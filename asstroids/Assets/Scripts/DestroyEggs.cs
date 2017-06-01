using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyEggs : MonoBehaviour
{
    [SerializeField] private GameObject[] EggsToDestroy;
    [SerializeField] private bool DestroyAll = true;

    PlayerGrab p;

    void Start()
    {
        var x = GameObject.FindGameObjectWithTag("Player");
        if (x)
            p = x.GetComponent<PlayerGrab>();
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (DestroyAll && col.tag == "Player")
        {
            foreach(var egg in EggsToDestroy)
            {
                Destroy(egg);
            }
        }

        foreach(var egg in EggsToDestroy)
        {
            if (col.gameObject == egg)
                Destroy(egg);

            //Also check the egg the player is holding.
            if (p.GrabbedObject && p.CurrentGrabbed == egg)
                p.DestroyCurrentEgg();
        }
    }	
}
