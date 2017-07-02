using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Egg : MonoBehaviour
{
    [Header("constant prefab options")]
    [SerializeField] private BrokenEgg breakEggPrefab;
    [SerializeField] private bool canBreak = true;
    [SerializeField] private bool canBounce = false;
    [SerializeField] private int EggUses = 1;
    [SerializeField] private AudioClip[] eggImpacts;
    private float minMagniutedForImpact = 1.0f;
    private bool destroying = false;

    [SerializeField] private EggType _eggType = EggType.SINK;

    private int usesLeft;
    public bool wasGrabbed = false;
    new private Rigidbody2D rigidbody;
    SpriteRenderer spriteRenderer;

    public EggType EType { get { return _eggType; } }

	void Start ()
    {
        usesLeft = EggUses + 1;

        if (_eggType == EggType.FLOAT)
            gameObject.AddComponent<FloatInWater>();
        else
            gameObject.AddComponent<SinkInWater>();

        rigidbody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

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
        if (destroying)
        {
            float rotateSpeed = 80;
            float moveSpeed = 3;
            float scaleSpeed = 1f;
            float colorSpeed = 2f;

            var rot = transform.eulerAngles;
            rot.z += rotateSpeed * Time.deltaTime;
            transform.eulerAngles = rot;

            transform.Translate(0, moveSpeed * Time.deltaTime, 0, Space.World);
            var scale = transform.localScale;
            scale.x -= scaleSpeed * Time.deltaTime;
            scale.y -= scaleSpeed * Time.deltaTime;
            spriteRenderer.color = new Color(1, 1, 1, spriteRenderer.color.a - colorSpeed * Time.deltaTime);
            if (spriteRenderer.color.a <= 0)
                Destroy(gameObject);
            transform.localScale = scale;
        }
	}

    public void DestroyEgg()
    {
        if (destroying)
            return;

        rigidbody.simulated = false;
        gameObject.GetComponent<Collider2D>().enabled = false ;
        destroying = true;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if(wasGrabbed)
        {
            if(--usesLeft <= 0 && canBreak)
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

        if (rigidbody.velocity.magnitude > minMagniutedForImpact)
            AudioSource.PlayClipAtPoint(eggImpacts[Random.Range(0, eggImpacts.Length)],
                transform.position);
            
    }
    
    public enum EggType
    {
        FLOAT,
        SINK
    }
}
