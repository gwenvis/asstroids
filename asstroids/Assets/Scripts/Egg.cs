﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Egg : MonoBehaviour
{
    [Header("constant prefab options")]
    [SerializeField] private BrokenEgg breakEggPrefab;
    [SerializeField] private bool canBreak = true;
    [SerializeField] private bool canBounce = false;
    [SerializeField] private int EggUses = 1;
    

    [SerializeField] private EggType eggType = EggType.SINK;

    private int usesLeft;
    public bool wasGrabbed = false;
    new private Rigidbody2D rigidbody;

	void Start ()
    {
        usesLeft = EggUses + 1;

        if (eggType == EggType.FLOAT)
            gameObject.AddComponent<FloatInWater>();

        rigidbody = GetComponent<Rigidbody2D>();

        if(canBounce)
        {
            var mat = new PhysicsMaterial2D();
            mat.friction = rigidbody.sharedMaterial.friction;
            mat.bounciness = 0.8f;
            rigidbody.sharedMaterial = mat;
        }
	}
	
	void Update ()
    {
		
	}

    void OnCollisionEnter2D(Collision2D col)
    {
        if(wasGrabbed)
        {
            if(--usesLeft <= 0)
            {

                if(breakEggPrefab)
                {
                    BrokenEgg obj = Instantiate(breakEggPrefab, transform.position, transform.rotation) as BrokenEgg;
                    obj.ApplyVelocities(rigidbody.velocity);
                }
                Destroy(gameObject);
            }

            wasGrabbed = false;
        }
            
    }
    
    private enum EggType
    {
        FLOAT,
        SINK
    }
}