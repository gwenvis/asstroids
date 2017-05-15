using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    private PlayerMovement playerMovement;
    PlatformerCollision plat;

    public void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        plat = GetComponent<PlatformerCollision>();
    }

    public void Update()
    {
        if (Input.GetKey(KeyCode.A))
            playerMovement.MoveHorizontal(-1);
        else if (Input.GetKey(KeyCode.D))
            playerMovement.MoveHorizontal(1);
        else
            playerMovement.MoveHorizontal(0);

        if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Space)) && plat.Grounded)
            playerMovement.Jump();
        else
            playerMovement.ApplyGravity();

        playerMovement.velocity = plat.Move(playerMovement.velocity * Time.deltaTime) / Time.deltaTime;
    }
}
