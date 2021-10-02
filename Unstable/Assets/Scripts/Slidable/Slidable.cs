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

        // Debugging
        [SerializeField]
        private GameObject m_projectionPrefab;

        #endregion

        private CapsuleCollider m_collider;
        private GameObject m_projection;

        #region Unity Callbacks

        private void Awake()
        {
            m_collider = this.GetComponent<CapsuleCollider>();
            m_projection = Instantiate(m_projectionPrefab, this.transform.parent);
        }

        /// <summary>
        /// Update is called once per frame
        /// </summary>
        private void Update()
        {
            Vector3 boardAngles = m_board.transform.rotation.eulerAngles;

            float xSteepness = CalcSteepness(boardAngles.x);
            float zSteepness = CalcSteepness(boardAngles.z);

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

            Debug.DrawLine(transform.position, m_projection.transform.position + dir, Color.blue);

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

        #endregion
    }

}