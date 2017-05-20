using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class PlatformerCollision : MonoBehaviour
{
    [SerializeField] private float skinWidth = 0.03f;
    [SerializeField] private float maxStepHeight = 0.01f;
    [SerializeField] private float maxAngle = 45f;
    [SerializeField] private LayerMask mask;
    private BoxCollider2D boxCollider;
    private RaycastOrigins origins;
    private CollisionInfo collisionInfo;
    private float hspace, vspace;

    public bool Grounded
    {
        get { return collisionInfo.bottom; }
    }

    private const int ROWS = 4;
    private const int COLS = 4;

    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        collisionInfo = new CollisionInfo();
        UpdateSpace();
    }

    public void VerticalCollision(ref Vector2 velocity)
    {
        float sign = Mathf.Sign(velocity.y);
        float rayLength = Mathf.Abs(velocity.y) + skinWidth;
        
        for (int i = 0; i < COLS+1; i++)
        {
            
            Vector3 position = sign == -1 ? origins.topleft : origins.bottomleft;
            position.x += hspace * i + velocity.x;

            Debug.DrawRay(position, Vector2.up * rayLength * sign, Color.red);
            RaycastHit2D hit = Physics2D.Raycast(position, Vector2.up * sign, rayLength, mask);
            if (hit)
            {
                collisionInfo.bottom = sign == -1;
                collisionInfo.top = !collisionInfo.bottom;
                velocity.y = (hit.distance - skinWidth) * sign;
            }
        }
     }
    
    public void HandleSlope(ref Vector2 velocity, float angle)
    {
        float moveDistance = Mathf.Abs(velocity.x);
        float climbVelY = Mathf.Sin(angle * Mathf.Deg2Rad) * moveDistance;
        
        if(velocity.y <= climbVelY)
        {
            velocity.y = climbVelY;
            velocity.x = Mathf.Cos(angle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(velocity.x);
            collisionInfo.bottom = true;
            collisionInfo.slopeAngle = angle;
            collisionInfo.climbingSlope = true;
        }
    }

    public void DescendSlope(ref Vector2 velocity)
    {
        float direction = Mathf.Sign(velocity.x);
        Vector2 rayOrgin = direction == -1 ? origins.topright : origins.topleft;
        var hit = Physics2D.Raycast(rayOrgin, Vector2.down, Mathf.Infinity, mask);

        if (hit)
        {
            float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
            if (slopeAngle != 0 && slopeAngle <= maxAngle)
            {
                if(Mathf.Sign(hit.normal.x) == direction && 
                    hit.distance - skinWidth <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x))
                {
                    float moveDistance = Mathf.Abs(velocity.x);
                    float desvel = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
                    velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * direction;
                    velocity.y -= desvel;

                    collisionInfo.slopeAngle = slopeAngle;
                    collisionInfo.bottom = true;
                    collisionInfo.descendingSlope = true;
                }
            }
        }
    }

    public void HorizontalCollision(ref Vector2 velocity)
    {
        float sign = Mathf.Sign(velocity.x);
        float rayLength = Mathf.Abs(velocity.x) + skinWidth;
        
        for (int i = 0; i < ROWS + 1; i++)
        {
            Vector2 position = sign == 1 ? origins.topright : origins.topleft;
            position.y += vspace * i;

            Debug.DrawRay(position, Vector2.right * velocity.x, Color.red);
            RaycastHit2D hit = Physics2D.Raycast(position, Vector2.right * sign, rayLength, mask);

            if (hit)
            {
                float angle = Vector3.Angle(hit.normal, Vector3.up);

                vector = hit.normal;
                point = hit.point;

                if(angle <= maxAngle && i == 0)
                {
                    HandleSlope(ref velocity, angle);
                }

                if (!collisionInfo.climbingSlope || angle > maxAngle)
                {
                    collisionInfo.left = sign < 0;
                    collisionInfo.right = !collisionInfo.left;

                    //StepHeight(ref velocity, sign);
                    velocity.x = (hit.distance - skinWidth) * sign;
                }
            }
        }
    }

    Vector3 vector;
    Vector3 point;
    void OnDrawGizmos()
    {
        Gizmos.DrawLine(point, point + vector);
    }

    public CollisionInfo Move(Vector2 velocity)
    {
        collisionInfo.Reset();
        UpdateRaycastPositions();

        if (velocity.y < 0)
            DescendSlope(ref velocity);

        if(velocity.x != 0)
            HorizontalCollision(ref velocity);
        if(velocity.y != 0)
            VerticalCollision(ref velocity);

        collisionInfo.velocity = velocity;

        transform.position += new Vector3(velocity.x, velocity.y);
        return collisionInfo;
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

    public struct CollisionInfo
    {
        public bool left, right, top, bottom,
            climbingSlope, descendingSlope;
        public float slopeAngle, oldSlopeAngle;
        public Vector2 velocity;

        public void Reset()
        {
            left = right = top = bottom =
                climbingSlope = descendingSlope = false;
            oldSlopeAngle = slopeAngle;
            slopeAngle = 0;
            velocity = Vector2.zero;
        }
    }

}
