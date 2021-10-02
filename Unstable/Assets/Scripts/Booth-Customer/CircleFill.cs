using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleFill : MonoBehaviour
{
    [SerializeField] private Transform rightCover;
    [SerializeField] private Transform leftCover;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateFill(float fillPercent)
    {
        rightCover.eulerAngles = new Vector3(0, 0, Mathf.Max(-180, fillPercent * -360));

        leftCover.eulerAngles = new Vector3(0, 0, Mathf.Min(0, fillPercent * -360 + 180));

    }
}
