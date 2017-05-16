using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] private bool lerp;
    [SerializeField] private bool autoOffset;
    [SerializeField] private Vector3 offset;

    void Start()
    {
        if (autoOffset)
        {
            offset = transform.position - target.position;
        }
    }

    void LateUpdate()
    {
        if(lerp)
        {

        }
        else
        {
            transform.position = target.position + offset;
        }
    }

}
