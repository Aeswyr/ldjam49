﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBounds : MonoBehaviour
{
    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Something dropped out of bounds!");

        Destroy(other.gameObject);
    }
}