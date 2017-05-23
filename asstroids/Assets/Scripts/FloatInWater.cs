using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class FloatInWater : MonoBehaviour {

    const float viscosity = 5f;

    Water[] waters;
    new Collider2D collider;
    new Rigidbody2D rigidbody;

    void Start()
    {
        FindWater();
        collider = GetComponent<Collider2D>();
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

    void FixedUpdate()
    {
        if (waters.Length == 0)
            return;

        foreach(var water in waters)
        {
            if (transform.position.x < water.transform.position.x ||
            transform.position.x > water.transform.position.x + water.Width ||
            transform.position.y < water.transform.position.y + water.Depth ||
            transform.position.y > water.transform.position.y)
                continue;

            int index = water.GetClosestPoint(transform.position.x);
            if (index == -1)
                continue;
            var pos = water.transform.TransformPoint(water.Positions[index]);
            
            float dist = (pos.y - transform.position.y) / -water.Depth;

            if (transform.position.y < pos.y)
            {
                rigidbody.AddForce(Vector2.up * 22, ForceMode2D.Force);
                Vector2 force = rigidbody.velocity * -1 * viscosity;
                rigidbody.AddForce(force, ForceMode2D.Force);
            }
        }
    }
}
