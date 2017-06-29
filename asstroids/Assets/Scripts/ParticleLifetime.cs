using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleLifetime : MonoBehaviour {

    [SerializeField] private float lifetime = 3.0f;
    private float time;

    void Start()
    {
        time = Time.time;
    }

    void Update()
    {
        if (time + lifetime < Time.time)
            DestroyImmediate(gameObject);
    }
}
