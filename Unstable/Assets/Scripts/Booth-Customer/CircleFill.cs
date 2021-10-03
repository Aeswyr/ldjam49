using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleFill : MonoBehaviour
{
    [SerializeField] private Transform rightCover;
    [SerializeField] private Transform leftCover;
    [Range(0.0f, 1f)]
    [SerializeField] private float testPercent;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //UpdateFill(testPercent);
    }

    public void UpdateFill(float fillPercent)
    {
        Debug.Log("Fill updated to: " + fillPercent);

        rightCover.localEulerAngles = new Vector3(0, 0, Mathf.Max(-180, fillPercent * -360));

        leftCover.localEulerAngles = new Vector3(0, 0, Mathf.Min(0, fillPercent * -360 + 180));
    }
}
