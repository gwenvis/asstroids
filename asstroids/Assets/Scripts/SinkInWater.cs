using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinkInWater : MonoBehaviour {

    Water[] waters;
    float maxySpeed = 3f;
    new Rigidbody2D rigidbody;

    void Start()
    {
        FindWater();
        rigidbody = GetComponent<Rigidbody2D>();
    }

    void FindWater()
    {
        var watersObjects = GameObject.FindGameObjectsWithTag("Water");
        waters = new Water[watersObjects.Length];

        for (int i = 0; i < watersObjects.Length; i++)
        {
            waters[i] = watersObjects[i].GetComponent<Water>();
        }
    }

    void Update()
    {
        foreach(var water in waters)
        {
            if (water.PointUnderwater(transform.position))
            {
                var velocity = rigidbody.velocity;
                if (velocity.y < -maxySpeed)
                    velocity.y = -maxySpeed;
                rigidbody.velocity = velocity;
                break;
            }
        }
    }
}
