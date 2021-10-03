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
        private float m_skinWidth = 0.5f;
        [SerializeField]
        private float m_steepMod = 15;
        [SerializeField]
        private float m_fallBuffer = 0.1f;
        [SerializeField]
        private float m_fallSpeed = 20f;
        [SerializeField]
        private float m_shuntSlideSpeed = 20;
        [SerializeField]
        private float m_shuntMinTime = 1f;

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
            z
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

            // project the new transform.position
            m_projection.transform.position = this.transform.position;

            float extents;
            switch (angleDir)
            {
                case AngleDir.x:
                    extents = m_collider.bounds.extents.z;
                    break;
                case AngleDir.z:
                    extents = m_collider.bounds.extents.x;
                    break;
                default:
                    extents = 0;
                    break;
            }

            m_projection.transform.localPosition += m_skinWidth * rawDir * extents;

            // raycast for obstacles
            Vector3 adjustedMovement = rawMovement;

            Vector3 dir = m_projection.transform.position - this.transform.position;

            // Debug.DrawLine(transform.position, m_projection.transform.position + dir, Color.blue);

            RaycastHit[] hits;

            hits = Physics.RaycastAll(m_projection.transform.position, dir, dir.magnitude);

            RaycastHit closestHit = new RaycastHit();

            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.gameObject.layer != LayerMask.NameToLayer("Barrier"))
                {
                    continue;
                }

                if (hit.Equals(hits[0]))
                {
                    closestHit = hit;
                }
                else if (hit.distance < closestHit.distance)
                {
                    closestHit = hit;
                }
            }

            // adjust movement if obstacle would be hit
            if (closestHit.collider != null)
            {
                Vector3 buffer = -dir.normalized * m_skinWidth;
                Vector3 newDest = closestHit.collider.ClosestPoint(transform.position) + buffer;
                adjustedMovement = newDest - transform.position;
                adjustedMovement.y = 0;
            }

            transform.localPosition += adjustedMovement;
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
            Vector3 rawDir = m_shuntDir;


            // x rotation correlates to z movement and vice versa

            rawMovement = rawDir * m_shuntSlideSpeed * Time.deltaTime;

            // project the new transform.position
            m_projection.transform.position = this.transform.position;

            Vector3 extents = m_collider.bounds.extents;

            m_projection.transform.localPosition += m_skinWidth * rawDir * ((extents.x + extents.z) / 2);

            // raycast for obstacles
            Vector3 adjustedMovement = rawMovement;

            Vector3 dir = m_projection.transform.position - this.transform.position;

            Debug.DrawLine(transform.position, m_projection.transform.position + dir * 50, Color.blue);

            RaycastHit[] hits;

            hits = Physics.RaycastAll(m_projection.transform.position, dir, dir.magnitude);

            RaycastHit closestHit = new RaycastHit();

            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.gameObject.layer != LayerMask.NameToLayer("Barrier"))
                {
                    continue;
                }

                if (hit.Equals(hits[0]))
                {
                    closestHit = hit;
                }
                else if (hit.distance < closestHit.distance)
                {
                    closestHit = hit;
                }
            }

            // adjust movement if obstacle would be hit
            if (closestHit.collider != null)
            {
                Vector3 buffer = -dir.normalized * m_skinWidth;
                Vector3 newDest = closestHit.collider.ClosestPoint(transform.position) + buffer;
                adjustedMovement = newDest - transform.position;
                adjustedMovement.y = 0;

                // end shunt when collide with a barrier
                EndShunt();
            }

            // TODO: make this Translate()
            transform.localPosition += adjustedMovement;
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

        private bool CheckFall()
        {
            RaycastHit hit;
            Physics.SphereCast(transform.position, m_collider.bounds.extents.z + m_fallBuffer, Vector3.down, out hit);

            if (hit.collider == null
                || (hit.collider.gameObject.layer == LayerMask.NameToLayer("Bounds")))
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
            Physics.SphereCast(transform.position, m_collider.bounds.extents.z / 2, Vector3.down, out hit);

            if (hit.collider != null
                && (hit.collider.gameObject.layer == LayerMask.NameToLayer("Tile")))
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