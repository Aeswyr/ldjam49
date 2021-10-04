using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unstable
{
    /// <summary>
    /// Rotates the Board according to horizontal / vertical inputs
    /// Prevents over-rotation
    /// </summary>
    public class BoardController : MonoBehaviour
    {
        #region Inspector 

        [SerializeField]
        private float m_rotationSpeed;

        #endregion

        #region Magic Numbers

        private static int ROTATION_LIMIT = 30;

        #endregion

        #region Unity Callbacks

        /// <summary>
        /// Update is called once per frame
        /// </summary>
        private void FixedUpdate()
        {
            RotateBoard();
        }

        #endregion

        #region Member Functions

        /// <summary>
        /// Rotates the board
        /// </summary>
        private void RotateBoard()
        {
            // initialize rotation
            var rotation = new Vector3(0, 0, 0);

            // get inputs
            float horizontalTilt = Input.GetAxis("Horizontal");
            float verticalTilt = Input.GetAxis("Vertical");

            if (horizontalTilt == 0 && verticalTilt == 0)
            {
                // no rotation input
                return;
            }

            // rotate left / right
            if (horizontalTilt != 0)
            {
                rotation += new Vector3(0, 0, 1f) * horizontalTilt;
            }

            // rotate up/down
            if (verticalTilt != 0)
            {
                rotation += new Vector3(1f, 0, 0) * -verticalTilt; // invert y-axis
            }

            // perform preliminary rotation
            transform.Rotate(rotation * m_rotationSpeed * Time.deltaTime, Space.World);            

            // this.GetComponent<Rigidbody>().MoveRotation(Quaternion.Euler(rotation * m_rotationSpeed * Time.deltaTime));

            // Apply rotation limits
            ApplyRotationLimits();
        }

        private void ApplyRotationLimits()
        {
            Vector3 angles = transform.rotation.eulerAngles;

            // disallow y rotation
            transform.rotation = Quaternion.Euler(new Vector3(angles.x, 0, angles.z));

            // x
            if ((angles.x < 360 - ROTATION_LIMIT)
                && (angles.x > ROTATION_LIMIT))
            {
                // not within acceptable range

                if (Mathf.Abs((360 - ROTATION_LIMIT) - angles.x) > Mathf.Abs(angles.x - ROTATION_LIMIT))
                {
                    // closer to ROTATION_LIMIT
                    transform.rotation = Quaternion.Euler(new Vector3(ROTATION_LIMIT, 0, angles.z));
                }
                else
                {
                    // closer to 360 - ROTATION_LIMIT
                    transform.rotation = Quaternion.Euler(new Vector3(360 - ROTATION_LIMIT, 0, angles.z));
                }
            }

            angles = transform.rotation.eulerAngles;

            // z
            if ((angles.z < 360 - ROTATION_LIMIT)
                && (angles.z > ROTATION_LIMIT))
            {
                // not within acceptable range

                if (Mathf.Abs((360 - ROTATION_LIMIT) - angles.z) > Mathf.Abs(angles.z - ROTATION_LIMIT))
                {
                    // closer to ROTATION_LIMIT
                    transform.rotation = Quaternion.Euler(new Vector3(angles.x, 0, ROTATION_LIMIT));
                }
                else
                {
                    // closer to 360 - ROTATION_LIMIT
                    transform.rotation = Quaternion.Euler(new Vector3(angles.x, 0, 360 - ROTATION_LIMIT));
                }
            }
        }

        #endregion

        #region Accessors

        public int GetRotationLimit()
        {
            return ROTATION_LIMIT;
        }

        #endregion
    }
}