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
    private Vector3 startPosition;
    private bool open;

    void Start()
    {
        startPosition = transform.position;

        if (button.Length == 0)
            return;

        foreach(var b in button)
        {
            b.Changed += ButtonStateChanged;
        }
    }

    void Update()
    {
        var wantedPos = startPosition;
        
        if(open)
        {
            if (vertical)
                wantedPos.y += inverted ? -moveDistance : moveDistance;
            else
                wantedPos.x += inverted ? -moveDistance : moveDistance;
        }

        transform.position = Vector3.Lerp(transform.position, wantedPos, lerpTime * Time.deltaTime);
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
