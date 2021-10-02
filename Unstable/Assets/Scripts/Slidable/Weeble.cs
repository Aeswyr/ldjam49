using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unstable
{
    /// <summary>
    /// An object that slides across the board
    /// </summary>
    [RequireComponent(typeof(CapsuleCollider))]
    public class Weeble : MonoBehaviour
    {
        #region Inspector

        [SerializeField]
        private BoardController m_board;
        [SerializeField]
        private float m_slideSpeed;
        [SerializeField]
        private float m_flatBuffer; // how far tilted the board must be before this slides
        [SerializeField]
        private float m_skinWidth;

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

            if (xSteepness > zSteepness)
            {
                SlideVertical(ref boardAngles);
            }
            else
            {
                SlideHorizontal(ref boardAngles);
            }
        }

        #endregion

        #region Member Functions

        private void SlideVertical(ref Vector3 boardAngles)
        {
            // x rotation correlates to z movement

            Vector3 rawMovement = new Vector3(0, 0, 0);
            Vector3 rawDir = new Vector3(0, 0, 0);

            if (boardAngles.x >= 360 - m_board.GetRotationLimit())
            {
                // tilt up

                if (boardAngles.x <= 360 - m_flatBuffer)
                {
                    rawDir = new Vector3(0, 0, -1);
                    // rawMovement = -transform.forward * Time.deltaTime;
                    rawMovement = rawDir * m_slideSpeed * Time.deltaTime;
                    // transform.localPosition += new Vector3(0, 0, 1) * m_slideSpeed * Time.deltaTime;
                    //Debug.DrawLine(transform.position, transform.position + -transform.forward * Time.deltaTime);
                }
            }
            else if (boardAngles.x >= 0 + m_flatBuffer)
            {
                // tilt down

                rawDir = new Vector3(0, 0, 1);
                //rawMovement = transform.forward * Time.deltaTime;
                rawMovement = rawDir * m_slideSpeed * Time.deltaTime;
                //transform.localPosition += new Vector3(0, 0, -1) * m_slideSpeed * Time.deltaTime;
                //Debug.DrawLine(transform.position, transform.position + transform.forward * Time.deltaTime);
            }

            // project the new transform.position
            m_projection.transform.position = this.transform.position;
            m_projection.transform.localPosition += rawMovement + rawDir * m_collider.bounds.extents.z;

            // raycast for obstacles
            Vector3 adjustedMovement = rawMovement;

            Vector3 dir = m_projection.transform.position - this.transform.position;
            Debug.DrawLine(transform.position, m_projection.transform.position + dir, Color.blue);

            RaycastHit[] hits;

            hits = Physics.RaycastAll(transform.position, dir, dir.magnitude);

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

            if (closestHit.collider != null)
            {
                Debug.Log("Collided");
                Vector3 newDest = transform.position; //closestHit.collider.ClosestPoint(m_projection.transform.position) + -dir.normalized * m_skinWidth;
                adjustedMovement = newDest - transform.position;
            }

            transform.localPosition += adjustedMovement;
        }

        private void SlideHorizontal(ref Vector3 boardAngles)
        {
            // z rotation correlates to x movement

            if (boardAngles.z >= 360 - m_board.GetRotationLimit())
            {
                if (boardAngles.z <= 360 - m_flatBuffer)
                {
                    transform.localPosition += new Vector3(1, 0, 0) * m_slideSpeed * Time.deltaTime;
                }
            }
            else if (boardAngles.z >= 0 + m_flatBuffer)
            {
                transform.localPosition += new Vector3(-1, 0, 0) * m_slideSpeed * Time.deltaTime;
            }
        }

        private float CalcSteepness(float angle)
        {
            float steepest = 0;

            int limit = m_board.GetRotationLimit();

            if (angle < limit)
            {
                steepest = angle;
            }
            else
            {
                steepest = 360 - angle;
            }

            return steepest;
        }

        #endregion
    }

}