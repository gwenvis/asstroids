using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder : MonoBehaviour
{
    public const float LADDER_WIDTH = 0.75f;


    [SerializeField] private float height = 2.0f;
    [SerializeField] private LadderSide side = LadderSide.RIGHT;

    public LadderSide Side { get { return side; } }

    public float Height { get { return height; } }

    void Start()
    {
        var boxcol = gameObject.AddComponent<BoxCollider2D>();
        boxcol.size = new Vector2(LADDER_WIDTH, height);
        boxcol.offset = new Vector2(LADDER_WIDTH / 2, height / 2);
        boxcol.isTrigger = true;
    }

    void OnDrawGizmos()
    {
        Vector3 pos = transform.position;
        pos.x += LADDER_WIDTH / 2;
        pos.y += height / 2;

        Vector3 size = new Vector3(LADDER_WIDTH, height, 1);
        Gizmos.color = new Color(0, 1, 0, 0.4f);
        Gizmos.DrawCube(pos, size);
    }

    public enum LadderSide
    {
        RIGHT,
        LEFT
    }
}
