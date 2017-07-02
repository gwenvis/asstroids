using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlatformerCollision))]
public class PlayerMovement : MonoBehaviour
{
    public const float MOVEMENT_SPEED = 18.0f;
    public const float MAX_MOVEMENT_SPEED = 10.0f;
    public const float HEAVY_MAX_MOVEMENT_SPEED = 4.0f;
    public const float HOLDING_MAX_MOVEMENT_SPEED = 5.5f;
    public const float DEC_SPEED = 30.0f;
    public const float GRAVITY = 30.0f;
    public const float UNDERWATER_GRAVITY = 5.0f;
    public const float UNDERWATER_MAX_GRAVITY = 5.0f;
    public const float MAX_GRAVITY = 70.0f;
    public const float JUMP_POWER = 12.0f;
    public const float JETPACK_MAX_SPEED = 20.0f;
    public const float JETPACK_ACCELERATION = 20.0f;
    public const float FUEL_USAGE_RATE = 34f;
    public const float FUEL_RECHARGE_RATE = 50f;

    [SerializeField] private float maxStepHeight = 0.1f;
    [SerializeField] private ParticleSystem walkParticle;
    [SerializeField] private ParticleSystem landParticle;
    [SerializeField] private ParticleSystem jetpackParticle;
    [SerializeField] private Transform walkParticlePosition;
    private float fuel = 100;

    Animator anim;
    PlatformerCollision plat;
    PlayerGrab plygrab;
    PlayerSound pls;
    Vector2 oldVelocity;

    public Vector2 velocity;
    public bool InWater = false;
    private bool oldGrounded;

    public bool OnLadder { get; private set; }
    private Ladder onLadderObject;

    private Water[] waters;

    void Start()
    {
        anim = GetComponent<Animator>();
        plat = GetComponent<PlatformerCollision>();
        pls = GetComponent<PlayerSound>();
        plygrab = GetComponent<PlayerGrab>();
        FindWaters();
    }

    public void MoveHorizontal(int dir)
    {
        if(OnLadder)
        {
            velocity.x = 0;
            return;
        }
        else if (dir == 0)
        {
            velocity.x = Slowdown(velocity.x);
            return;
        }

        velocity.x += dir * MOVEMENT_SPEED * Time.deltaTime;

        if(plygrab.GrabbedObject && plygrab.CurrentGrabbedEggObject)
        {
            if (plygrab.CurrentGrabbedEggObject.EType == Egg.EggType.SINK && Mathf.Abs(velocity.x) > HEAVY_MAX_MOVEMENT_SPEED)
                velocity.x = Mathf.Sign(velocity.x) * HEAVY_MAX_MOVEMENT_SPEED;
            else if (Mathf.Abs(velocity.x) > HOLDING_MAX_MOVEMENT_SPEED)
                velocity.x = Mathf.Sign(velocity.x) * HOLDING_MAX_MOVEMENT_SPEED;
        }
        else if (Mathf.Abs(velocity.x) > MAX_MOVEMENT_SPEED)
        {
            velocity.x = Mathf.Sign(velocity.x) * MAX_MOVEMENT_SPEED;
        }
    }

    public void FireWalkParticle()
    {
        if (walkParticle)
        {
            Destroy(Instantiate(walkParticle, walkParticlePosition.position, walkParticlePosition.rotation), 2f);
        }
    }

    public void MoveAnimation(Vector3 velocity)
    {
        if (OnLadder)
            return;

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
        if (plat.Grounded)
        {
            anim.SetInteger("jump", 0);
            if (fuel < 100)
            {
                fuel += FUEL_RECHARGE_RATE * Time.deltaTime;
                pls.PlayJetpackRefillingSound();
                if (fuel > 100)
                {
                    pls.PlayJetpackRefilledSound();
                    pls.PauseJetpackRefillingSound();
                    fuel = 100;
                }
            }
        }
        else
            pls.PauseJetpackRefillingSound();

        if (Input.GetKeyUp(KeyCode.Space))
        {
            pls.PauseJetpackUsingSound();
            jetpackParticle.Stop();
        }

        if (InWater)
            anim.SetInteger("inwater", 1);
        else
            anim.SetInteger("inwater", 0);

        if(plat.Grounded != oldGrounded)
        {
            if (plat.Grounded && oldVelocity.y < -2f)
            {
                pls.PlayLandSound();
                var p = Instantiate(landParticle, new Vector3(
                    walkParticlePosition.position.x,
                    walkParticlePosition.position.y + 0.2f,
                    walkParticlePosition.position.z), Quaternion.identity);
            }
        }

        oldGrounded = plat.Grounded;
        oldVelocity = velocity;
            
        var cols = Physics2D.BoxCastAll(transform.position, new Vector2(1, 1), 0, Vector2.zero);
        foreach (var col in cols)
        {
            
            if (col.transform.tag == "Ladder" &&
                !OnLadder && (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.W)))
            {
                //Get on the ladder
                onLadderObject = col.transform.GetComponent<Ladder>();
                OnLadder = true;

                var pos = transform.position;
                pos.x = onLadderObject.transform.position.x + (onLadderObject.Side == Ladder.LadderSide.RIGHT ? Ladder.LADDER_WIDTH : 0);
                transform.position = pos;

                var scale = transform.localScale;
                scale.x = onLadderObject.Side == Ladder.LadderSide.RIGHT ? -1 : 1;
                transform.localScale = scale;

                anim.SetBool("onladder", true);

                plygrab.Drop();
            }
        }
    }

    public void Jump()
    {
        if (plat.Grounded || InWater || OnLadder)
        {
            velocity.y = JUMP_POWER;
            anim.SetInteger("jump", 1);
            pls.PlayJumpSound();

            if (OnLadder)
                ExitLadder();
        }
        else if(fuel > 0 && velocity.y < JETPACK_MAX_SPEED + 1)
        {
            fuel -= FUEL_USAGE_RATE * Time.deltaTime;
            pls.PlayJetpackUsingSound();
            jetpackParticle.Play();
            if (fuel < 0)
            {
                fuel = 0;
                pls.PlayJetpackEmpty();
                pls.PauseJetpackUsingSound();
                jetpackParticle.Stop();
            }
            if (velocity.y < 0)
                velocity.y += GRAVITY * Time.deltaTime;
            velocity.y += JETPACK_ACCELERATION * Time.deltaTime;
            if (velocity.y > JETPACK_MAX_SPEED)
                velocity.y = JETPACK_MAX_SPEED;
        }
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
        if (OnLadder)
        {
            LadderMovement();
            return;
        }

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

    void LadderMovement()
    {
        if (plygrab.CurrentGrabbed)
            plygrab.Drop();

        if (Input.GetKey(KeyCode.W))
        {
            velocity.y = MAX_MOVEMENT_SPEED;
            anim.speed = 1;
            Debug.Log(plat.BoxCollider.bounds.min.y);
            if (plat.BoxCollider.bounds.min.y > onLadderObject.transform.position.y + onLadderObject.Height)
                ExitLadder();
        }
        else if(Input.GetKey(KeyCode.S))
        {
            velocity.y = -MAX_MOVEMENT_SPEED;
            anim.speed = 1;
            if (plat.BoxCollider.bounds.min.y < onLadderObject.transform.position.y)
                ExitLadder();
        }
        else
        {
            velocity.y = 0;
            anim.speed = 0;
        }
    }

    void ExitLadder()
    {
        OnLadder = false;
        onLadderObject = null;
        anim.SetBool("onladder", false);
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

    void OnGUI()
    {
        GUI.Label(new Rect(0, 0, 100, 100), fuel.ToString());
    }
}
