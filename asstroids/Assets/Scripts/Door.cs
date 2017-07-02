using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] Player.Button[] button;
    [SerializeField] private float moveDistance = 5.0f;
    [SerializeField] private float lerpTime = 5.0f;
    [SerializeField] private bool vertical = false;
    [SerializeField] private bool inverted = false;
    [SerializeField] private AudioClip doorOpenClip;
    [SerializeField] private AudioClip doorCloseClip;
    private Vector3 startPosition;
    private Animator animator;
    private BoxCollider2D boxCollider;
    private bool open;
    private bool oldState;

    private AudioSource audioSource;

    void Start()
    {
        startPosition = transform.position;

        if (button.Length == 0)
            return;

        foreach(var b in button)
        {
            if(b)
            b.Changed += ButtonStateChanged;
        }

        animator = GetComponentInChildren<Animator>();
        boxCollider = GetComponentInChildren<BoxCollider2D>();
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.spatialBlend = 1;
        audioSource.loop = false;

        oldState = open;
    }

    void Update()
    {
        var wantedPos = startPosition;
        
        boxCollider.enabled = !open;
        
        animator.SetBool("open", open);

        if(oldState != open)
        {
            if(open)
            {
                audioSource.Stop();
                audioSource.clip = doorOpenClip;
                audioSource.Play();
            }
            else
            {
                audioSource.Stop();
                audioSource.clip = doorCloseClip;
                audioSource.Play();
            }
        }

        transform.position = Vector3.Lerp(transform.position, wantedPos, lerpTime * Time.deltaTime);

        oldState = open;
    }

    private void ButtonStateChanged(Player.Button sender, bool pressed)
    {
        bool opendoor = true;
        foreach(var b in button)
        {
            if (!b.Pressed) opendoor = false;
        }
        open = opendoor;
    }
}
