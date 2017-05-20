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
        private bool pressed = false;

        public event ButtonStateChanged Changed;

        void Start()
        {
            startPos = transform.position;
        }

        void Update()
        {
            Vector3 wantedPos = startPos;
            if (pressed)
                wantedPos.y -= pressDistance;
            
            transform.position = Vector3.Lerp(transform.position, wantedPos, lerpTime * Time.deltaTime);
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            pressed = true;
            if(Changed != null)
                Changed(pressed);
        }

        void OnTriggerExit2D(Collider2D other)
        {
            pressed = false;
            if(Changed != null)
                Changed(pressed);
        }
    }
}