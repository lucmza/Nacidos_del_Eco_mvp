using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CountdownTimer : MonoBehaviour
{
    [SerializeField] private float startTimeInSeconds = 300f; 
    [SerializeField] private TextMeshProUGUI timerText; 
    [SerializeField] private string gameOverSceneName = "GameOver"; 

    private float currentTime;
    private bool isRunning = true;

    void Start()
    {
        currentTime = startTimeInSeconds;
        UpdateTimerUI();
    }

    void Update()
    {
        if (!isRunning) return;

        currentTime -= Time.deltaTime;
        UpdateTimerUI();

        if (currentTime <= 0)
        {
            isRunning = false;
            currentTime = 0;
            UpdateTimerUI();
            SceneManager.LoadScene(gameOverSceneName);
        }
    }

    private void UpdateTimerUI()
    {
        int minutes = Mathf.FloorToInt(currentTime / 60f);
        int seconds = Mathf.FloorToInt(currentTime % 60f);
        timerText.text = $"{minutes:00}:{seconds:00}";
    }

   
    public void PauseTimer() => isRunning = false;

    public void ResumeTimer() => isRunning = true;
}
