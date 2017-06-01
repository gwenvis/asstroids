using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Water : MonoBehaviour
{
    const float springconstant = 0.02f;
    const float damping = 0.04f;//0.04f;
    const float spread = 0.05f;//0.05f;
    const float z = -5f;

    [SerializeField] private Material lineMat;
    [SerializeField] private GameObject splash;
    [SerializeField] private GameObject waterMesh;

    [Header("Water size settings")]
    [SerializeField] float xOffset;
    [SerializeField] float waterWidth = 10;
    [SerializeField] float waterDepth = -10;
    [SerializeField] float topOffset = 0;
    [SerializeField] float maxSplashVelocity = 4;

    [Header("Water Sound")]
    [SerializeField] private AudioClip[] waterSplash;
    AudioSource audioSource;

    Vector3[] positions;
    GameObject[] colliders;
    Mesh[] meshes;
    GameObject[] meshObjects;
    float[] velocities, accelerations;

    LineRenderer lines;
    float baseHeight,
        bottom,
        left;

    public Vector3[] Positions
    {
        get { return positions; }
    }

    public float Depth
    {
        get { return waterDepth; }
    }

    public float Width
    {
        get { return waterWidth; }
    }
    
	void Start ()
    {
        if (!Application.isPlaying)
            return;
        var wantedpos = transform.position;
        transform.position = Vector3.zero;
        CreateWater(-xOffset, waterWidth, waterDepth, topOffset);
        transform.position = wantedpos;
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.spatialBlend = 1.0f;
    }

    public bool PointUnderwater(Vector3 point)
    {
        if (point.x < transform.position.x ||
            point.x > transform.position.x + Width ||
            point.y < transform.position.y + Depth ||
            point.y > transform.position.y)
            return false;

        int index = GetClosestPoint(point.x);
        if (index == -1)
            return false;

        var pos = transform.TransformPoint(positions[index]);

        if (point.y < pos.y)
            return true;

        

        return false;
    }
    
    void CreateWater(float left, float width, float bottom, float top)
    {
        int edgeCount = Mathf.RoundToInt(width) * 5;
        int nodeCount = edgeCount + 1;

        lines = gameObject.AddComponent<LineRenderer>();
        lines.material = lineMat;
        lines.material.renderQueue = 1000;
        lines.startWidth = 0.2f;
        lines.endWidth = 0.2f;
        lines.positionCount = nodeCount;

        positions = new Vector3[nodeCount];
        colliders = new GameObject[edgeCount];
        meshes = new Mesh[edgeCount];
        meshObjects = new GameObject[edgeCount];

        velocities = new float[nodeCount];
        accelerations = new float[nodeCount];

        baseHeight = top;
        this.bottom = bottom;
        this.left = left;

        for(int i = 0; i < nodeCount; i++)
        {
            positions[i] = new Vector3(left + width * i / edgeCount, top, z);
            accelerations[i] = 0;
            velocities[i] = 0;
            lines.SetPosition(i, positions[i]);
        }

        for(int i = 0; i < edgeCount; i++)
        {
            meshes[i] = new Mesh();
            Vector3[] vertices = new Vector3[4];
            vertices[0] = new Vector3(positions[i].x, positions[i].y, z);
            vertices[1] = new Vector3(positions[i + 1].x, positions[i + 1].y, z);
            vertices[2] = new Vector3(positions[i].x, bottom, z);
            vertices[3] = new Vector3(positions[i + 1].x, bottom, z);

            Vector2[] UVs = new Vector2[4];
            UVs[0] = new Vector2(0, 1);
            UVs[1] = new Vector2(1, 1);
            UVs[2] = new Vector2(0, 0);
            UVs[3] = new Vector2(1, 0);

            int[] tris = new int[] { 0,1,3,3,2,0 };

            meshes[i].vertices = vertices;
            meshes[i].uv = UVs;
            meshes[i].triangles = tris;

            meshObjects[i] = Instantiate(waterMesh, Vector3.zero, Quaternion.identity);
            meshObjects[i].GetComponent<MeshFilter>().mesh = meshes[i];
            meshObjects[i].transform.parent = transform;

            var col = new GameObject();
            col.name = "Trigger";
            col.AddComponent<BoxCollider2D>();
            col.transform.parent = transform;
            col.transform.localPosition = new Vector3(left + width * (i + 0.5f) / edgeCount, top - 0.25f, 0);
            col.transform.localScale = new Vector3(width / edgeCount, 0.5f, 1);
            col.GetComponent<BoxCollider2D>().isTrigger = true;
            col.layer = LayerMask.NameToLayer("Water");
            col.AddComponent<DetectWaterHit>();
            colliders[i] = col;

            lines.useWorldSpace = false;
        }
    }

    void UpdateMeshes()
    {
            for (int i = 0; i < meshes.Length; i++)
            {

                Vector3[] Vertices = new Vector3[4];
                Vertices[0] = new Vector3(positions[i].x, positions[i].y, z);
                Vertices[1] = new Vector3(positions[i + 1].x, positions[i + 1].y, z);
                Vertices[2] = new Vector3(positions[i].x, bottom, z);
                Vertices[3] = new Vector3(positions[i + 1].x, bottom, z);

                meshes[i].vertices = Vertices;
            }
    }

    void FixedUpdate()
    {
        if (!Application.isPlaying)
            return;
        for (int i = 0; i < positions.Length; i++)
        {
            float force = springconstant * (positions[i].y - baseHeight) + velocities[i] * damping;
            accelerations[i] = -force;
            positions[i].y += velocities[i];
            velocities[i] += accelerations[i];
            lines.SetPosition(i, positions[i]);
        }

        float[] leftDeltas = new float[positions.Length];
        float[] rightDeltas = new float[positions.Length];

        for (int j = 0; j < 8; j++)
        {
            for (int i = 0; i < positions.Length; i++)
            {
                if (i > 0)
                {
                    leftDeltas[i] = spread * (positions[i].y - positions[i - 1].y);
                    velocities[i - 1] += leftDeltas[i];
                }
                if (i < positions.Length - 1)
                {
                    rightDeltas[i] = spread * (positions[i].y - positions[i + 1].y);
                    velocities[i + 1] += rightDeltas[i];
                }
            }
        }

        for (int i = 0; i < positions.Length; i++)
        {
            if (i > 0)
            {
                positions[i - 1].y += leftDeltas[i];
            }
            if (i < positions.Length - 1)
            {
                positions[i + 1].y += rightDeltas[i];
            }
        }

        UpdateMeshes();
    }

    private float lastSplashTime;
    private float minSplashTime = 0.08f;

    public void Splash(float xpos, float velocity)
    {
        if(Mathf.Abs(velocity) > maxSplashVelocity)
        {
            velocity = maxSplashVelocity * Mathf.Sign(velocity);
        }

        Debug.Log(velocity);

        int index = GetClosestPoint(xpos);
        if (index == -1)
            return;
        velocities[index] += velocity;

        if (Time.time > lastSplashTime + minSplashTime)
        {
            AudioSource.PlayClipAtPoint(waterSplash[Random.Range(0, waterSplash.Length)],
                new Vector3(xpos, transform.position.y));
            lastSplashTime = Time.time;
        }
    }

    public int GetClosestPoint(float xpos)
    {
        if(xpos > positions[0].x + transform.position.x && xpos < positions[positions.Length - 1].x + transform.position.x)
        {
            xpos -= positions[0].x + transform.position.x;

            return Mathf.RoundToInt((positions.Length - 1) * (xpos / (positions[positions.Length - 1].x - positions[0].x)));
        }

        return -1;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0.1f, 0.1f, 1f, 0.2f);
        Vector3 waterCenter = transform.position;
        waterCenter.x += waterWidth / 2;
        waterCenter.y += waterDepth / 2;

        Vector3 waterSize = new Vector3(waterWidth, waterDepth, 1);
        Gizmos.DrawCube(waterCenter, waterSize);
    }
}
