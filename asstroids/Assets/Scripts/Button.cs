using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public delegate void ButtonStateChanged(Button b, bool pressed);

    public class Button : MonoBehaviour
    {
        [SerializeField] private float pressDistance = 0.25f;
        [SerializeField] private float lerpTime = 5.0f;
        [SerializeField] private bool oneTimeUse;
        private Vector3 startPos;
        private BoxCollider2D boxCollider;
        private CapsuleCollider2D capCollider;
        private bool pressed = false;
        private bool lastPressed = false;

        public event ButtonStateChanged Changed;
        public bool Pressed { get { return pressed; } }

        void Start()
        {
            startPos = transform.position;
            boxCollider = GetComponent<BoxCollider2D>();
            capCollider = GetComponent<CapsuleCollider2D>();
            capCollider.enabled = false;
            if (oneTimeUse)
                GetComponent<SpriteRenderer>().color = new Color(0.5f, 0, 0.5f);
        }

        void Update()
        {

            Collider2D[] c = new Collider2D[1];
            if (boxCollider.OverlapCollider(new ContactFilter2D(), c) > 0)
            {
                pressed = true;
            }
            else
            {
                if(pressed && oneTimeUse)
                {
                    Rigidbody2D rb = gameObject.AddComponent<Rigidbody2D>();
                    transform.position = new Vector3(
                        transform.position.x, transform.position.y + 0.2f, transform.position.z);
                    capCollider.enabled = true;
                    rb.AddForce(transform.up * 500);
                    rb.AddTorque(Random.Range(-10, 10));

                    Destroy(this);
                    if (Changed != null)
                        Changed = null;
                    return;
                }

                pressed = false;
            }

            if(lastPressed != pressed)
            {
                if(Changed != null)
                    Changed(this, pressed);
            }

            Vector3 wantedPos = startPos;
            if (pressed)
                wantedPos.y -= pressDistance;

            transform.position = Vector3.Lerp(transform.position, wantedPos, lerpTime * Time.deltaTime);

            lastPressed = pressed;
        }
    }
}