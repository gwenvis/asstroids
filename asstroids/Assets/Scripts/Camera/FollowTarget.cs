using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    public Transform target;
    [SerializeField] private bool lerp;
    [SerializeField] private float lerpTime = 2.0f;

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
            transform.position = Vector3.Lerp(transform.position, target.position + offset, lerpTime * Time.deltaTime);
        }
        else
        {
            transform.position = target.position + offset;
        }
    }

}
