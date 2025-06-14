using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonStart : MonoBehaviour
{
    [Header("Índices de Escena")]
    [Tooltip("Escena de Menú Principal")]
    [SerializeField] private int sceneMainMenu = 1;
    [Tooltip("Escena de Nivel de Juego")]
    [SerializeField] private int sceneLevelGame = 2;

 

    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("Salir del juego");
    }

 

    public void StartNewGame()
    {
        Time.timeScale = 1f;
       
        SceneManager.LoadScene(sceneLevelGame);
    }
}

