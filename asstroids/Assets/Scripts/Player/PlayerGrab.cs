using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGrab : MonoBehaviour {

    public bool GrabbedObject { get; private set; }
    [SerializeField] private float maxdistance = 2;
    [SerializeField] private float throwStrength = 10;
    [SerializeField] private LayerMask grabLayer;
    [SerializeField] private LayerMask predictionLayer;
    [SerializeField] private int predictionAccuracy = 10;
    [SerializeField] private Transform grabTransform;
    [SerializeField] private Material lineRendererMat;
    
    public GameObject CurrentGrabbed { get; private set; }
    public Egg CurrentGrabbedEggObject { get; private set; }
    private Rigidbody2D CurrentRigid;
    private BoxCollider2D col;
    private LineRenderer lineRenderer;
    private PlayerMovement plyMovement;
    private PlayerSound pls;
    private bool justGrabbed = false;

    public void Start()
    {
        if (!grabTransform)
            grabTransform = transform;
        col = GetComponent<BoxCollider2D>();
        plyMovement = GetComponent<PlayerMovement>();
        pls = GetComponent<PlayerSound>();
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.material = lineRendererMat;
        lineRenderer.positionCount = predictionAccuracy;
        lineRenderer.endWidth = 0.07f;
        lineRenderer.startWidth = 0.15f;
        lineRenderer.enabled = false;
    }

	public void Grab()
    {
        var e = CheckNearbyCollision();

        if (e)
        {
            var rigid = e.GetComponent<Rigidbody2D>();
            CurrentGrabbedEggObject = e.GetComponent<Egg>();
            rigid.simulated = false;
            e.gameObject.transform.parent = transform;
            var pos = grabTransform.position;
            pos.z += -1;
            e.gameObject.transform.position = pos;
            CurrentGrabbed = e.gameObject;
            CurrentRigid = rigid;
            GrabbedObject = true;
            CurrentGrabbed.layer = LayerMask.NameToLayer("Grabbed");
            justGrabbed = true;
            var egg = e.GetComponent<Egg>();
            pls.PlayGrabSound();
            if(egg)
                e.GetComponent<Egg>().wasGrabbed = true;
        }
    }

    public Collider2D CheckNearbyCollision()
    {
        var hit = Physics2D.CircleCast(transform.position, maxdistance, Vector2.zero, maxdistance, grabLayer);
        return hit.collider;
    }

    void LateUpdate()
    {
        if (Input.GetMouseButtonUp(0))
            justGrabbed = false;
        if (!Input.GetMouseButton(0))
            lineRenderer.enabled = false;
    }

    public void Throw()
    {
        if (GrabbedObject && !justGrabbed)
        {
            var m = new Vector3(Input.mousePosition.x, Input.mousePosition.y, transform.position.z);
            var n = Camera.main.ScreenToWorldPoint(m);
            var dir = n - transform.position;
            var direction = new Vector2(dir.x, dir.y);
            var plvel = plyMovement.velocity;
            if (Mathf.Abs(plvel.y) < 2f)
                plvel.y = 0;
            CurrentRigid.simulated = true;
            CurrentRigid.velocity = (direction * throwStrength) + plvel;
            CurrentGrabbed.transform.parent = null;
            CurrentGrabbedEggObject = null;
            pls.PlayThrowSound();
            StartCoroutine(ChangeLayer(CurrentGrabbed));
            GrabbedObject = false;
        }
    }

    public void PredictTrajectory()
    {
        if (justGrabbed)
            return;

        var m = new Vector3(Input.mousePosition.x, Input.mousePosition.y, transform.position.z);
        var n = Camera.main.ScreenToWorldPoint(m);
        var dir = n - transform.position;
        var direction = new Vector2(dir.x, dir.y);
        var plvel = plyMovement.velocity;
        if (Mathf.Abs(plvel.y) < 2f)
            plvel.y = 0;
        Vector2 velocity = (direction * throwStrength) + plvel;
        Vector2[] v = Plot(CurrentRigid, CurrentRigid.transform.position, velocity, 1500);
        int diff = v.Length / predictionAccuracy;
        int length = v.Length / diff;

        lineRenderer.positionCount = predictionAccuracy;

        var originray = transform.position;
        for(int i = 0; i < length; i++)
        {
            
            lineRenderer.SetPosition(i, new Vector3(v[i * diff].x, v[i * diff].y, transform.position.z));
            var p = lineRenderer.GetPosition(i);
            var hit = Physics2D.Raycast(originray, p - originray, Vector2.Distance(originray, p), predictionLayer);
            if (i != length - 1 && hit)
            {
                lineRenderer.positionCount = i + 1;
                lineRenderer.SetPosition(i, hit.point);
                break;
            }
            originray = p;
        }
    
        lineRenderer.enabled = true;
    }

    private Vector2[] Plot(Rigidbody2D rigidbody, Vector2 pos, Vector2 velocity, int steps)
    {
        Vector2[] results = new Vector2[steps];

        float timestep = Time.fixedDeltaTime / Physics2D.velocityIterations;
        Vector2 gravityAccel = Physics2D.gravity * rigidbody.gravityScale * timestep * timestep;
        float drag = 1f - timestep * rigidbody.drag;
        Vector2 moveStep = velocity * timestep;

        for (int i = 0; i < steps; ++i)
        {
            moveStep += gravityAccel;
            moveStep *= drag;
            pos += moveStep;
            results[i] = pos;
        }

        return results;
    }

    public void DestroyCurrentEgg()
    {
        GrabbedObject = false;
        Destroy(CurrentGrabbed);
        CurrentGrabbed = null;
    }

    public void Drop()
    {
        Debug.Log("Drop");

        if (GrabbedObject && !justGrabbed)
        {
            CurrentRigid.simulated = true;
            CurrentRigid.velocity = plyMovement.velocity;
            CurrentGrabbed.transform.parent = null;
            CurrentGrabbedEggObject = null;
            StartCoroutine( ChangeLayer(CurrentGrabbed) ) ;
            GrabbedObject = false;
        }
    }

    IEnumerator ChangeLayer(GameObject g)
    {
        yield return new WaitForSeconds(1.0f);
        if (g)
        {
            var gcol = g.GetComponent<Collider2D>();
            bool touchingPlayer = true;

            while (touchingPlayer)
            {
                Collider2D[] cols = new Collider2D[1];
                gcol.OverlapCollider(new ContactFilter2D(),
                    cols);

                Debug.Log(cols.Length);

                touchingPlayer = false;
                foreach (var c in cols)
                {
                    if (c == col)
                    {
                        touchingPlayer = true;
                        break;
                    }
                }

                yield return new WaitForEndOfFrame();
            }

            g.layer = LayerMask.NameToLayer("Grabbable");
        }
    }
}
