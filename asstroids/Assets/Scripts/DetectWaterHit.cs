using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectWaterHit : MonoBehaviour
{
    Water water;
    PlayerMovement player;

    void Start()
    {
        water = transform.parent.GetComponent<Water>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>() ;
    }

    void OnTriggerEnter2D(Collider2D other)
    {

        if(other.tag == "Player")
        {
            Debug.Log(player.velocity.y);
            water.Splash(transform.position.x, player.velocity.y / 50);

            return;
        }

        var rigid = other.gameObject.GetComponent<Rigidbody2D>();
        
        if (rigid)
        {
            Debug.Log(rigid);
            water.Splash(transform.position.x,
                rigid.velocity.y * rigid.mass / 50);
        }
    }
}
