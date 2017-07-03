using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] Button startButton;
    [SerializeField] Button quitButton;
    [SerializeField] Rigidbody2D Egg;

    void Start()
    {
        startButton.Changed += StartGame;
        quitButton.Changed += QuitGame;
    }

    private void QuitGame(Button b, bool pressed)
    {
        if (pressed)
            StartCoroutine(GameAction(false));
        Egg.AddForce(Vector2.up * 2000);
    }

    private void StartGame(Button b, bool pressed)
    {
        if (pressed)
            StartCoroutine(GameAction(true));
        Egg.AddForce(Vector2.up * 2000);
    }


    /// <summary>
    /// Quit or start the game.
    /// </summary>
    /// <param name="start"></param>
    /// <returns></returns>
    IEnumerator GameAction(bool start)
    {
        yield return new WaitForSeconds(2.0f);
        if (start)
            UnityEngine.SceneManagement.SceneManager.LoadScene(1);
        else
            Application.Quit();
    }
}
