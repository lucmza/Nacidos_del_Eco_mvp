using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class IntroController : MonoBehaviour
{
    [Header("Referencias Video")]
    public VideoPlayer videoPlayer;
    public RawImage videoScreen;

    [Header("Referencias HUD")]
    public CanvasGroup hudGroup;      
    public TextMeshProUGUI infoText;             
    public Button nextButton;         

    [Header("Texto de páginas")]
    [TextArea] public string[] pages; 
    private int currentPage = 0;
    [Header("Botón Saltar")]
    public Button skipButton;
    [Header("Escena destino")]
    public string mainMenuSceneName = "SceneStart";

    void Start()
    {
        hudGroup.alpha = 0f;
        hudGroup.interactable = false;
        hudGroup.blocksRaycasts = false;

        skipButton.gameObject.SetActive(true);


        videoPlayer.Play();
        videoPlayer.loopPointReached += OnVideoFinished;

        nextButton.onClick.AddListener(OnNextClicked);
        skipButton.onClick.AddListener(SkipVideo);
    }
    public void SkipVideo()
    {
        // Detener el video inmediatamente
        if (videoPlayer.isPlaying)
            videoPlayer.Stop();

        EndIntroSequence();
    }

    private void EndIntroSequence()
    {
       
        if (videoScreen != null)
            videoScreen.gameObject.SetActive(false);

       
        skipButton.gameObject.SetActive(false);

        
        ShowHUDPage(0);
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        // Ocultamos la RawImage del video
        if (videoScreen != null)
            videoScreen.gameObject.SetActive(false);

        ShowHUDPage(0);
    }

   
    private void ShowHUDPage(int pageIndex)
    {
        if (pageIndex < 0 || pageIndex >= pages.Length) return;

        currentPage = pageIndex;
        infoText.text = pages[pageIndex];

        hudGroup.alpha = 1f;
        hudGroup.interactable = true;
        hudGroup.blocksRaycasts = true;
    }

    
    private void HideHUD()
    {
        hudGroup.alpha = 0f;
        hudGroup.interactable = false;
        hudGroup.blocksRaycasts = false;
    }

   
    private void OnNextClicked()
    {
        int nextPage = currentPage + 1;
        if (nextPage < pages.Length)
        {
            ShowHUDPage(nextPage);
        }
        else
        {
            
            HideHUD();
            StartCoroutine(LoadMainMenuAfterFrame());
        }
    }

    private IEnumerator LoadMainMenuAfterFrame()
    {
        
        yield return null;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    private void OnDestroy()
    {
        videoPlayer.loopPointReached -= OnVideoFinished;
        nextButton.onClick.RemoveListener(OnNextClicked);
    }
}