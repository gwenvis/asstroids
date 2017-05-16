using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlatformerCollision))]
public class PlayerMovement : MonoBehaviour
{
    public const float MOVEMENT_SPEED = 25.0f;
    public const float MAX_MOVEMENT_SPEED = 10.0f;
    public const float DEC_SPEED = 30.0f;
    public const float GRAVITY = 20.0f;
    public const float MAX_GRAVITY = 16.0f;
    public const float JUMP_POWER = 15.0f;

    PlatformerCollision plat;
    public Vector2 velocity;

    void Start()
    {
        plat = GetComponent<PlatformerCollision>();
    }

    public void MoveHorizontal(int dir)
    {
        if(dir == 0)
        {
            velocity.x = Slowdown(velocity.x);
            return;
        }

        velocity.x += dir * MOVEMENT_SPEED * Time.deltaTime;
        if(Mathf.Abs(velocity.x) > MAX_MOVEMENT_SPEED)
        {
            velocity.x = Mathf.Sign(velocity.x) * MAX_MOVEMENT_SPEED;
        }
    }

    public void Jump()
    {
        velocity.y = JUMP_POWER;
    }

    public void ApplyGravity()
    {
        if (plat.Grounded)
            velocity.y = 0;

        velocity.y -= GRAVITY * Time.deltaTime;
        if (velocity.y < -MAX_GRAVITY)
            velocity.y = -MAX_GRAVITY;
    }

    float Slowdown(float value)
    {
        float sign = Mathf.Sign(value);
        float abs = Mathf.Abs(value);
        abs -= DEC_SPEED * Time.deltaTime;
        if (abs < 0)
            abs = 0;
        return abs * sign;
    }
}
