using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuCamera : MonoBehaviour
{
    Vector3 endPosition = new Vector3(0, 0, -10);
    Quaternion endRotation = Quaternion.identity;
    float wantedSize = 7.54f;
    bool move = false;

    public void Start()
    {
        StartCoroutine(CameraAnimation());
    }

    void Update()
    {
        if (!move)
            return;
        float lerpSpeed = 1;
        transform.position = Vector3.Lerp(transform.position, endPosition, lerpSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, endRotation, lerpSpeed * Time.deltaTime);
        Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, wantedSize, lerpSpeed * Time.deltaTime);
    }

    IEnumerator CameraAnimation()
    {
        yield return new WaitForSeconds(2.0f);
        move = true;
    }
}
