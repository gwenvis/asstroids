using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public delegate void ButtonStateChanged(bool pressed);

    public class Button : MonoBehaviour
    {
        [SerializeField] private float pressDistance = 0.25f;
        [SerializeField] private float lerpTime = 5.0f;
        private Vector3 startPos;
        private BoxCollider2D boxCollider;
        private bool pressed = false;
        private bool lastPressed = false;

        public event ButtonStateChanged Changed;

        void Start()
        {
            startPos = transform.position;
            boxCollider = GetComponent<BoxCollider2D>();
        }

        void Update()
        {
            Collider2D[] c = new Collider2D[1];
            if (boxCollider.OverlapCollider(new ContactFilter2D(), c) > 0)
            {
                pressed = true;
            }
            else
                pressed = false;

            if(lastPressed != pressed)
            {
                Changed(pressed);
            }

            Vector3 wantedPos = startPos;
            if (pressed)
                wantedPos.y -= pressDistance;

            transform.position = Vector3.Lerp(transform.position, wantedPos, lerpTime * Time.deltaTime);

            lastPressed = pressed;
        }
    }
}