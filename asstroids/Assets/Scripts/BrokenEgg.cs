using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrokenEgg : MonoBehaviour {

    [SerializeField] private Rigidbody2D[] rigidbodies;
    
    public void ApplyVelocities(Vector3 velocity)
    {
        foreach(var rigid in rigidbodies)
        {
            rigid.velocity = -velocity;
        }
    }
}
