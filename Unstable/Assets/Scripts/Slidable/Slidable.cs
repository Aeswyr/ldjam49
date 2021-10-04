using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unstable
{
    /// <summary>
    /// An object that slides across the board
    /// </summary>
    [RequireComponent(typeof(CapsuleCollider))]
    public class Slidable : MonoBehaviour
    {
        #region Inspector

        [SerializeField]
        private BoardController m_board;
        [SerializeField]
        private float m_slideSpeed = 3;
        [SerializeField]
        private float m_flatBuffer = 1.5f; // how far tilted the board must be before this slides
        [SerializeField]
        private float m_skinWidth = 0.4f;
        [SerializeField]
        private float m_steepMod = 15;
        [SerializeField]
        private float m_fallBuffer = 0.1f;
        [SerializeField]
        private float m_fallSpeed = 20f;
        [SerializeField]
        private float m_shuntSlideSpeed = 10;
        [SerializeField]
        private float m_shuntMinTime = 1f;
        [SerializeField]
        private int m_ray_dimension = 3;

        // Debugging
        [SerializeField]
        private GameObject m_projectionPrefab;

        #endregion

        private CapsuleCollider m_collider;
        private GameObject m_projection;
        private Slidable m_slidable;
        private Tile m_currTile;
        private Tile m_prevTile;
        private bool m_shunting;
        private Vector3 m_shuntDir;
        private float m_shuntCountdown;

        private bool m_locked;

        #region Unity Callbacks

        private void Awake()
        {
            m_collider = this.GetComponent<CapsuleCollider>();
            m_projection = Instantiate(m_projectionPrefab, this.transform.parent);
            m_slidable = this.GetComponent<Slidable>();
            m_locked = false;
            m_shunting = false;

            // m_board = BoardController.instance;

            if (m_board == null)
                m_board = GameObject.Find("Board").GetComponent<BoardController>();
        }

        /// <summary>
        /// Update is called once per frame
        /// </summary>
        private void Update()
        {
            if (m_locked) { return; }

            // check for falling

            if (CheckFall())
            {
                Fall();
                return;
            }

            // tile influence

            m_currTile = IdentifyCurrTile();

            if (m_currTile != null)
            {
                if (m_prevTile == null)
                {
                    EnterCurrTile();
                }
                else if (m_currTile != m_prevTile)
                {
                    ExitPrevTile();

                    EnterCurrTile();
                }

                m_prevTile = m_currTile;
            }

            if (m_shunting)
            {
                ShuntSlide();

                m_shuntCountdown -= Time.deltaTime;

                if (m_shuntCountdown <= 0)
                {
                    EndShunt();
                }

                // return; //todo: remove this to add complex movement functionality
            }

            // default slide behavior
            Vector3 boardAngles = m_board.transform.rotation.eulerAngles;

            SlideHorizontal(ref boardAngles);
            SlideVertical(ref boardAngles);
        }
        #endregion

        #region Member Functions

        private enum AngleDir
        {
            x,
            z,
            shunt
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="boardAngles"></param>
        /// <param name="angleDir"></param>
        private void Slide(ref Vector3 boardAngles, AngleDir angleDir)
        {
            Vector3 rawMovement = new Vector3(0, 0, 0);
            Vector3 rawDir = new Vector3(0, 0, 0);

            // x rotation correlates to z movement and vice versa

            float boardAngle;
            switch (angleDir)
            {
                case AngleDir.x:
                    boardAngle = boardAngles.x;
                    break;
                case AngleDir.z:
                    boardAngle = boardAngles.z;
                    break;
                default:
                    boardAngle = 0;
                    break;
            }

            float steepness = CalcSteepness(boardAngle);

            if (boardAngle >= 360 - m_board.GetRotationLimit())
            {
                // tilt up/left
                if (boardAngle <= 360 - m_flatBuffer)
                {
                    switch (angleDir)
                    {
                        case AngleDir.x:
                            rawDir = new Vector3(0, 0, -1);
                            break;
                        case AngleDir.z:
                            rawDir = new Vector3(1, 0, 0);
                            break;
                        default:
                            break;
                    }
                    rawMovement = rawDir * m_slideSpeed * (steepness / m_steepMod) * Time.deltaTime;
                }
            }
            else if (boardAngle >= 0 + m_flatBuffer)
            {
                // tilt down/right
                switch (angleDir)
                {
                    case AngleDir.x:
                        rawDir = new Vector3(0, 0, 1);
                        break;
                    case AngleDir.z:
                        rawDir = new Vector3(-1, 0, 0);
                        break;
                    default:
                        break;
                }
                rawMovement = rawDir * m_slideSpeed * (steepness / m_steepMod) * Time.deltaTime;
            }

            Vector3 adjustedMovement = rawMovement;
            Vector3 projectionDir = Vector3.zero;

            // raycast line as far as movement

            // raycast for obstacles
            RaycastHit closestHit = CollisionRaycast(angleDir, rawDir, rawMovement, ref projectionDir);

            // if raycast hits, trim movement down to collision point - skinWidth
            AdjustMovement(ref adjustedMovement, ref closestHit, false);

            // move to new position
            //transform.localPosition += adjustedMovement;
            transform.Translate(adjustedMovement);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="boardAngles"></param>
        private void SlideVertical(ref Vector3 boardAngles)
        {
            Slide(ref boardAngles, AngleDir.x);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="boardAngles"></param>
        private void SlideHorizontal(ref Vector3 boardAngles)
        {
            Slide(ref boardAngles, AngleDir.z);
        }

        private void ShuntSlide()
        {
            // slide toward ShuntDir

            Vector3 rawMovement = new Vector3(0, 0, 0);
            Vector3 rawDir = m_shuntDir.normalized;

            // x rotation correlates to z movement and vice versa

            rawMovement = rawDir * m_shuntSlideSpeed * Time.deltaTime;

            Vector3 adjustedMovement = rawMovement;
            Vector3 projectionVector = Vector3.zero;

            // raycast for obstacles
            RaycastHit closestHit = CollisionRaycast(AngleDir.shunt, rawDir, rawMovement, ref projectionVector);

            AdjustMovement(ref adjustedMovement, ref closestHit, true);

            // move to new position
            transform.localPosition += adjustedMovement;
        }

        private void AdjustMovement(ref Vector3 adjustedMovement, ref RaycastHit closestHit, bool shunting)
        {
            // adjust movement if obstacle would be hit
            if (closestHit.collider != null)
            {
                Debug.Log("collision");
                
                /*
                Vector3 newDest = // Physics.ClosestPoint(transform.position, closestHit.collider, closestHit.collider.transform.position, closestHit.collider.transform.rotation);
                    closestHit.collider.ClosestPointOnBounds(transform.position);
                Vector3 bufferDir = (this.transform.position - newDest).normalized;
                adjustedMovement = newDest - transform.position + (bufferDir * m_skinWidth);
                adjustedMovement.y = 0;
                */

                adjustedMovement = Vector3.zero;                

                if (shunting)
                {
                    //end shunt when collide with a barrier
                    EndShunt();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        private float CalcSteepness(float angle)
        {
            float steepness = 0;

            int limit = m_board.GetRotationLimit();

            if ((int)angle <= limit)
            {
                steepness = angle;
            }
            else
            {
                steepness = 360 - angle;
            }


            return steepness;
        }

        private RaycastHit CollisionRaycast(AngleDir angleDir, Vector3 rawDir, Vector3 rawMovement, ref Vector3 projectionVector)
        {
            // project the new transform.position
            m_projection.transform.position = this.transform.position;

            // determine how to place projection on outside of collider where facing
            Bounds bounds = m_collider.bounds;
            Vector3 extents;
            switch (angleDir)
            {
                case AngleDir.x:
                    extents = rawDir * bounds.extents.z;
                    break;
                case AngleDir.z:
                    extents = rawDir * bounds.extents.x;
                    break;
                case AngleDir.shunt:
                    extents = rawDir * (bounds.extents.z + bounds.extents.x) / 2;
                    break;
                default:
                    extents = Vector3.zero;
                    break;
            }

            // projection used to get absolute vector for local position change

            m_projection.transform.localPosition +=
                // m_skinWidth * // buffer ratio
                rawMovement // direction facing
                // + extents // distance from middle to edge of collider
                ;

            Vector3 start = transform.position;
            //start.y -= bounds.extents.y/2 - .2f;
            projectionVector = m_projection.transform.position - this.transform.position;

            Vector3 raySpacing = CalcRaySpacing();
            Vector3 startMod = new Vector3(
                bounds.extents.x * projectionVector.normalized.x,
                bounds.extents.y * projectionVector.normalized.y,
                bounds.extents.z * projectionVector.normalized.z
                );
            Vector3 rayStarts = start + startMod; // pushes all rays to lowest edge

            // cast a ray at each interval from lowest to highest edge

            RaycastHit closestHit = new RaycastHit();

            for (int i = 0; i < m_ray_dimension; ++i)
            {
                Ray collisionRay = new Ray(rayStarts + raySpacing * i, projectionVector.normalized);

                Debug.DrawLine(collisionRay.origin, collisionRay.origin + collisionRay.direction * projectionVector.magnitude * 10, Color.blue);

                RaycastHit hit;

                float maxDistance = (projectionVector.magnitude > m_skinWidth) ? projectionVector.magnitude : m_skinWidth;
                Physics.Raycast(collisionRay, out hit, maxDistance, LayerMask.GetMask("Barrier"));

                if (hit.collider != null)
                {
                    if (closestHit.collider == null)
                    {
                        closestHit = hit;
                    }
                    else if (hit.distance < closestHit.distance)
                    {
                        closestHit = hit;
                    }
                }
            }

            return closestHit;

            /*

            RaycastHit closestHit = new RaycastHit();

            foreach (RaycastHit hit in hits)
            {
                if (hit.Equals(hits[0]))
                {
                    closestHit = hit;
                }
                else if (hit.distance < closestHit.distance)
                {
                    closestHit = hit;
                }
            }

            return closestHit;

            */
        }

        private Vector3 CalcRaySpacing()
        {
            Vector3 extents = m_collider.bounds.extents;

            float verticalSpacing = extents.y / m_ray_dimension;

            // only vertical currently implemented
            return new Vector3(0, verticalSpacing, 0);
        }

        private bool CheckFall()
        {
            RaycastHit hit;
            Physics.SphereCast(transform.position, m_collider.bounds.extents.z + m_fallBuffer, Vector3.down, out hit, LayerMask.GetMask("Bounds"));

            if (hit.collider == null)
            {
                return true;
            }

            return false;
        }

        private void Fall()
        {
            Vector3 fallMovement = Vector3.down * m_fallSpeed * Time.deltaTime;
            transform.Translate(fallMovement);
        }

        private Tile IdentifyCurrTile()
        {
            RaycastHit hit;
            Physics.SphereCast(transform.position, m_collider.bounds.extents.z / 2, Vector3.down, out hit, LayerMask.GetMask("Tile"));

            if (hit.collider != null)
            {
                return hit.collider.gameObject.GetComponent<Tile>();
            }

            return null;
        }

        private void ExitPrevTile()
        {
            var prevType = m_prevTile.GetTileType();
            switch (prevType)
            {
                case (Tile.Type.wood):
                    // deregister wood effect
                    m_prevTile.GetComponent<WoodTile>().OnExit(ref m_slidable);
                    break;
                case (Tile.Type.ice):
                    // deregister ice effect
                    m_prevTile.GetComponent<IceTile>().OnExit(ref m_slidable);
                    break;
                case (Tile.Type.puddle):
                    // deregister puddle effect
                    m_prevTile.GetComponent<PuddleTile>().OnExit(ref m_slidable);
                    break;
                case (Tile.Type.none):
                    break;
                default:
                    break;
            }
        }

        private void EnterCurrTile()
        {
            var currType = m_currTile.GetTileType();
            switch (currType)
            {
                case (Tile.Type.wood):
                    // register wood effect
                    m_currTile.GetComponent<WoodTile>().OnEnter(ref m_slidable);
                    break;
                case (Tile.Type.ice):
                    // register ice effect
                    m_currTile.GetComponent<IceTile>().OnEnter(ref m_slidable);
                    break;
                case (Tile.Type.puddle):
                    // register puddle effect
                    m_currTile.GetComponent<PuddleTile>().OnEnter(ref m_slidable);
                    break;
                default:
                    break;
            }
        }

        public void MultiplySlideSpeed(float factor)
        {
            m_slideSpeed *= factor;
        }

        public void Shunt()
        {
            m_shunting = true;
            m_shuntCountdown = m_shuntMinTime;

            // randomly generate a direction
            float shuntX = Random.Range(-1f, 1f);
            float shuntZ = Random.Range(-1f, 1f);

            m_shuntDir = new Vector3(shuntX, 0, shuntZ);
        }

        public void EndShunt()
        {
            m_shunting = false;
            m_shuntDir = Vector3.zero;
            m_shuntCountdown = 0;
        }

        #endregion

        #region Getters and Setters

        public bool IsLocked()
        {
            return m_locked;
        }

        public void SetLocked(bool locked)
        {
            m_locked = locked;
        }

        #endregion
    }

}