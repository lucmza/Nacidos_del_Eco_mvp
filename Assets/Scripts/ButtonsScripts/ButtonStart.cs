using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonStart : MonoBehaviour
{
    [SerializeField] private int sceneStart = 1;
    [SerializeField] private int lvlGame = 2;

    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("Salir del juego");
    }
    public void SceneStart()
    {
        SceneManager.LoadScene(sceneStart);
    }

    public void LevelGame()
    {
        SceneManager.LoadScene(lvlGame);
    }
}
