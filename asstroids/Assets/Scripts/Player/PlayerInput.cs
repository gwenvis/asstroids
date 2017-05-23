﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    private PlayerMovement playerMovement;
    private PlatformerCollision plat;
    private PlayerGrab plag;
    

    public void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        plat = GetComponent<PlatformerCollision>();
        plag = GetComponent<PlayerGrab>();
    }

    public void Update()
    {
        if (Input.GetKey(KeyCode.A))
            playerMovement.MoveHorizontal(-1);
        else if (Input.GetKey(KeyCode.D))
            playerMovement.MoveHorizontal(1);
        else
            playerMovement.MoveHorizontal(0);

        if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Space)) && (plat.Grounded || playerMovement.InWater))
            playerMovement.Jump();
        else
            playerMovement.ApplyGravity();

        if (Input.GetMouseButtonDown(0))
        {
            // grab or throw
            if(plag.GrabbedObject)
                plag.Throw();
            else
                plag.Grab();
        }
        else if (Input.GetMouseButton(1))
        {
            if (plag.GrabbedObject)
                plag.Drop();
        }

        var info = plat.Move(playerMovement.velocity * Time.deltaTime);
        if(!info.climbingSlope)
        {
            if (info.bottom || info.top)
                playerMovement.velocity.y = 0;
            if (info.left || info.right)
                playerMovement.velocity.x = 0;
        }
        
        playerMovement.MoveAnimation(info.velocity);
    }
}
