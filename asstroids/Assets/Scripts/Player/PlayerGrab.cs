using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGrab : MonoBehaviour {

    public bool GrabbedObject { get; private set; }
    [SerializeField] private float maxdistance = 2;
    [SerializeField] private float throwStrength = 10;
    [SerializeField] private LayerMask grabLayer;
    [SerializeField] private Transform grabTransform;
    
    private GameObject CurrentGrabbed;
    private Rigidbody2D CurrentRigid;
    private BoxCollider2D col;
    private PlayerMovement plyMovement;
    private bool justGrabbed = false;

    public void Start()
    {
        if (!grabTransform)
            grabTransform = transform;
        col = GetComponent<BoxCollider2D>();
        plyMovement = GetComponent<PlayerMovement>();
    }

	public void Grab()
    {
        var e = CheckCollisionOnMouse();
        if (e && Vector3.Distance(transform.position, e.gameObject.transform.position) < maxdistance)
        {
            var rigid = e.GetComponent<Rigidbody2D>();
            rigid.simulated = false;
            e.gameObject.transform.parent = transform;
            e.gameObject.transform.position = grabTransform.position;
            CurrentGrabbed = e.gameObject;
            CurrentRigid = rigid;
            GrabbedObject = true;
            CurrentGrabbed.layer = LayerMask.NameToLayer("Grabbed");
            justGrabbed = true;
        }
    }

    public Collider2D CheckCollisionOnMouse()
    {
        var mousepos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, transform.position.z);
        var point = Camera.main.ScreenToWorldPoint(mousepos);

        return Physics2D.OverlapPoint(point, grabLayer);
    }

    void Update()
    {
        if (Input.GetMouseButtonUp(0))
            justGrabbed = false;
    }

    public void Throw()
    {

        Debug.Log("Throw");

        if (GrabbedObject && !justGrabbed)
        {
            var m = new Vector3(Input.mousePosition.x, Input.mousePosition.y, transform.position.z);
            var n = Camera.main.ScreenToWorldPoint(m);
            var dir = n - transform.position;
            var direction = new Vector2(dir.x, dir.y);

            CurrentRigid.simulated = true;
            CurrentRigid.velocity = (direction * throwStrength) + plyMovement.velocity;
            CurrentGrabbed.transform.parent = null;

            StartCoroutine(ChangeLayer(CurrentGrabbed));
            GrabbedObject = false;
        }
    }
    

    public void Drop()
    {
        Debug.Log("Drop");

        if (GrabbedObject && !justGrabbed)
        {
            CurrentRigid.simulated = true;
            CurrentRigid.velocity = plyMovement.velocity;
            CurrentGrabbed.transform.parent = null;
            StartCoroutine( ChangeLayer(CurrentGrabbed) ) ;
            GrabbedObject = false;
        }
    }

    IEnumerator ChangeLayer(GameObject g)
    {
        yield return new WaitForSeconds(1.0f);
        var gcol = g.GetComponent<Collider2D>();
        bool touchingPlayer = true;

        while(touchingPlayer)
        {
            Collider2D[] cols = new Collider2D[1];
            gcol.OverlapCollider(new ContactFilter2D(),
                cols);

            Debug.Log(cols.Length);

            touchingPlayer = false;
            foreach(var c in cols)
            {
                if (c == col) {
                    touchingPlayer = true;
                    break;
                }
            }

            yield return new WaitForEndOfFrame();
        }

        g.layer = LayerMask.NameToLayer("Grabbable");
    }
}
