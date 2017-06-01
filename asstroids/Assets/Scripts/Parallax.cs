using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 0.1f;
    [SerializeField] private float lerpTime = 5.0f;
    [SerializeField] private Transform target;
    private float depth;

    private Vector3 lastPos;
    private Vector3 offset;
    private Vector3 staticOffset;

    void Start()
    {
        depth = transform.position.z;
        staticOffset = transform.position - target.position;
        staticOffset.z = 0;
    }

    void LateUpdate()
    {

        offset += (lastPos - target.position) * moveSpeed / 100;

        var wantedpos = target.position + offset + staticOffset;
        wantedpos.z = depth;

        transform.position = wantedpos;

        lastPos = target.position;
    }
}
