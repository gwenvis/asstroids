using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlatformerCollision))]
public class PlayerMovement : MonoBehaviour
{
    public const float MOVEMENT_SPEED = 25.0f;
    public const float MAX_MOVEMENT_SPEED = 10.0f;
    public const float DEC_SPEED = 30.0f;
    public const float GRAVITY = 30.0f;
    public const float UNDERWATER_GRAVITY = 5.0f;
    public const float UNDERWATER_MAX_GRAVITY = 5.0f;
    public const float MAX_GRAVITY = 70.0f;
    public const float JUMP_POWER = 15.0f;

    [SerializeField] private float maxStepHeight = 0.1f;

    Animator anim;
    PlatformerCollision plat;
    PlayerSound pls;
    public Vector2 velocity;
    Vector2 oldVelocity;

    public bool InWater = false;
    private bool oldGrounded;

    private Water[] waters;

    void Start()
    {
        anim = GetComponent<Animator>();
        plat = GetComponent<PlatformerCollision>();
        pls = GetComponent<PlayerSound>();
        FindWaters();
    }

    public void MoveHorizontal(int dir)
    {
        if (dir == 0)
        {
            velocity.x = Slowdown(velocity.x);
            return;
        }

        velocity.x += dir * MOVEMENT_SPEED * Time.deltaTime;
        if (Mathf.Abs(velocity.x) > MAX_MOVEMENT_SPEED)
        {
            velocity.x = Mathf.Sign(velocity.x) * MAX_MOVEMENT_SPEED;
        }
    }

    public void MoveAnimation(Vector3 velocity)
    {
        if (Mathf.Abs(velocity.x) > 0)
        {
            anim.SetInteger("walking", 1);
            anim.speed = Mathf.Abs(velocity.x) * 5;

            var scale = transform.localScale;
            if (velocity.x < -0.01f)
                scale.x = -1;
            else if (velocity.x > 0.01f)
                scale.x = 1;
            transform.localScale = scale;
        }
        else
        {
            anim.SetInteger("walking", 0);
            anim.speed = 1;
        }
    }

    void Update()
    {
        if (plat.Grounded || InWater)
            anim.SetInteger("jump", 0);

        if(plat.Grounded != oldGrounded)
        {
            Debug.Log(oldVelocity);
            if (plat.Grounded && oldVelocity.y < -2f)
                pls.PlayLandSound();
        }

        oldGrounded = plat.Grounded;
        oldVelocity = velocity;
    }

    public void Jump()
    {
        velocity.y = JUMP_POWER;
        anim.SetInteger("jump", 1);
        pls.PlayJumpSound();
    }

    void CheckIfInWater()
    {
        InWater = false;

        foreach(var water in waters)
        {
            if(water.PointUnderwater(transform.position))
            {
                InWater = true;
                return;
            }
        }
    }

    public void ApplyGravity()
    {
        CheckIfInWater();
        bool inwater = InWater;
        var maxgrav = InWater ? UNDERWATER_MAX_GRAVITY : MAX_GRAVITY;
        var grav = InWater ? UNDERWATER_GRAVITY : GRAVITY;

        if (plat.Grounded)
            velocity.y = 0;

        velocity.y -= grav * Time.deltaTime;
        if (velocity.y < -maxgrav)
            velocity.y = -maxgrav;
    }

    void FindWaters()
    {
        var waterObjects = GameObject.FindGameObjectsWithTag("Water");
        waters = new Water[waterObjects.Length];

        for(int i = 0; i < waters.Length; i++)
        {
            waters[i] = waterObjects[i].GetComponent<Water>();
        }
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
