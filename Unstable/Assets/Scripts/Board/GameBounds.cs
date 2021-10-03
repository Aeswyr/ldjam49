using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unstable
{
    /// <summary>
    /// Affectionately known as the Emmolation Sphere
    /// </summary>
    public class GameBounds : MonoBehaviour
    {
        private void OnTriggerExit(Collider other)
        {
            Destroy(other.gameObject);
        }
    }

}