using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardController : MonoBehaviour
{
    #region Inspector 

    [SerializeField]
    private float m_rotationSpeed;

    #endregion

    #region Unity Callbacks

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    private void Update()
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

        // perform rotation
        transform.Rotate(rotation * m_rotationSpeed * Time.deltaTime, Space.World);
    }

    #endregion
}
