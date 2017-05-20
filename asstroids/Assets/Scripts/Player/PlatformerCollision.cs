using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class PlatformerCollision : MonoBehaviour
{
    [SerializeField] private float skinWidth = 0.03f;
    [SerializeField] private float maxStepHeight = 0.01f;
    [SerializeField] private LayerMask mask;
    private BoxCollider2D boxCollider;
    private RaycastOrigins origins;
    private float hspace, vspace;
    private bool grounded = false;

    public bool Grounded
    {
        get { return grounded; }
    }

    private const int ROWS = 4;
    private const int COLS = 4;

    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        UpdateSpace();
    }

    public void VerticalCollision(ref Vector2 velocity)
    {
        float sign = Mathf.Sign(velocity.y);
        float rayLength = Mathf.Abs(velocity.y) + skinWidth;

        grounded = false;
        for (int i = 0; i < COLS+1; i++)
        {
            
            Vector3 position = sign == -1 ? origins.topleft : origins.bottomleft;
            position.x += hspace * i + velocity.x;

            Debug.DrawRay(position, Vector2.up * rayLength * sign, Color.red);
            RaycastHit2D hit = Physics2D.Raycast(position, Vector2.up * sign, rayLength, mask);
            if (hit)
            {
                grounded = true;
                velocity.y = (hit.distance - skinWidth) * sign;
            }
        }
     }
    
    public void HandleSlope(ref Vector2 velocity, float direction)
    {
        Vector3 raycastorigin = direction == 1 ? origins.topright : origins.topleft;
        raycastorigin.x += velocity.x;
        raycastorigin.y += maxStepHeight;

        var hit = Physics2D.Raycast(raycastorigin, -Vector3.up, maxStepHeight * 2, mask);
        if (hit) Debug.Log(hit.normal.x);
        if (hit && Mathf.Abs(hit.normal.x) > 0.1f )
        {
            var pos = transform.position;
            pos.y += -hit.normal.x * Mathf.Abs(velocity.x) * (velocity.x - hit.normal.x > 0 ? 1 : -1);
            transform.position = pos;
        }
    }

    public void HorizontalCollision(ref Vector2 velocity)
    {
        float sign = Mathf.Sign(velocity.x);
        float rayLength = Mathf.Abs(velocity.x) + skinWidth;

        HandleSlope(ref velocity, sign);

        for (int i = 0; i < ROWS + 1; i++)
        {
            Vector2 position = sign == 1 ? origins.topright : origins.topleft;
            position.y += vspace * i;

            Debug.DrawRay(position, Vector2.right * velocity.x, Color.red);
            RaycastHit2D hit = Physics2D.Raycast(position, Vector2.right * sign, rayLength, mask);

            if (hit)
            {
                velocity.x = (hit.distance - skinWidth) * sign;
            }
        }
    }

    public Vector3 Move(Vector2 velocity)
    {
        UpdateRaycastPositions();
        if(velocity.x != 0)
            HorizontalCollision(ref velocity);
        if(velocity.y != 0)
            VerticalCollision(ref velocity);

        transform.position += new Vector3(velocity.x, velocity.y);
        return velocity;
    }

    void UpdateRaycastPositions()
    {
        Bounds bounds = boxCollider.bounds;
        bounds.Expand(skinWidth * -2);

        origins = new RaycastOrigins();
        origins.topleft = new Vector2(bounds.min.x, bounds.min.y);
        origins.topright = new Vector2(bounds.max.x, bounds.min.y);
        origins.bottomleft = new Vector2(bounds.min.x, bounds.max.y);
        origins.bottomright = new Vector2(bounds.max.x, bounds.max.y);

    }

    void UpdateSpace()
    {
        Bounds bounds = boxCollider.bounds;
        bounds.Expand(skinWidth * -2);
        hspace = bounds.size.x / COLS;
        vspace = bounds.size.y / ROWS;
    }

    struct RaycastOrigins
    {
        public Vector2 topleft, topright, bottomright, bottomleft;
    }
}
