using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHostage : MonoBehaviour
{
    FollowTarget ft;
    [SerializeField] private Transform newTarget;
    bool move = false;
    public Color color;

    public void Start()
    {
        ft = Camera.main.GetComponent<FollowTarget>();
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.tag == "Player" && !move)
        {
            ft.target = newTarget;
            move = true;
            StartCoroutine(GoToMainMenu());
        }
    }

    IEnumerator GoToMainMenu()
    {
        yield return new WaitForSeconds(8.0f);
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    void Update()
    {
        if (move)
        {
            newTarget.Translate(0, -10 * Time.deltaTime, 0);
            color.a += Time.deltaTime * 0.05f;
        }
    }

    void OnGUI()
    {
        GUIStyle guistyle = new GUIStyle();
        GUI.contentColor = color;
        guistyle.fontStyle = FontStyle.Bold;
        guistyle.fontSize = 20;
        guistyle.alignment = TextAnchor.MiddleCenter;
        GUI.Label(new Rect(Screen.width / 2, Screen.height / 2, 500, 500), "Dankjewel voor het spelen", guistyle);
    }
}
