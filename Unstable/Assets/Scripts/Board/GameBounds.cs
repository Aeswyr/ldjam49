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
        LevelController controller;
        private void Start()
        {
            controller = GameObject.FindObjectOfType<LevelController>();    
        }

        private void OnTriggerExit(Collider other)
        {
            Customer newCustomer = other.GetComponent<Customer>();
            if (newCustomer != null)
            {
                if (newCustomer.status != Customer.customerStatus.doneEating)
                    controller.CustomerFed(0);
            }

            Destroy(other.gameObject);
        }
    }

}