using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class MenuController : MonoBehaviour
{


    [Header("Referencias a paneles")]
    [Tooltip("Contenedor del HUD normal del juego")]
    public GameObject hudGameplayContainer;
    [Tooltip("Panel del menú principal")]
    public GameObject menuPrincipalPanel;
   

    [Header("Botones del Menú Principal")]
    public Button buttonBackToGame;  
    public Button buttonMisiones;    
    public Button buttonExitGame;    

    [Header("Botones del Sub-Menú Misiones")]
    public Button buttonBackFromMisiones;

    private bool isMenuOpen = false;  

    void Start()
    {
       
        hudGameplayContainer.SetActive(true);
        menuPrincipalPanel.SetActive(false);
        

        
        buttonBackToGame.onClick.AddListener(OnBackToGamePressed);
        buttonMisiones.onClick.AddListener(OnMisionesPressed);
        buttonExitGame.onClick.AddListener(OnExitGamePressed);
        buttonBackFromMisiones.onClick.AddListener(OnBackFromMisionesPressed);
    }

    void Update()
    {

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Debug.Log("ESC PRESSED!");
            if (isMenuOpen)
                CloseMenuPrincipal();
            else
                OpenMenuPrincipal();
        }
    }

   

    private void OpenMenuPrincipal()
    {
        isMenuOpen = true;

       
       hudGameplayContainer.SetActive(false);

      
        menuPrincipalPanel.SetActive(true);

      
       

       
        Time.timeScale = 0f;
    }

    private void CloseMenuPrincipal()
    {
        isMenuOpen = false;

       
        menuPrincipalPanel.SetActive(false);
        

      
        hudGameplayContainer.SetActive(true);

     
        Time.timeScale = 1f;
    }

    


    private void OnBackToGamePressed()
    {
        CloseMenuPrincipal();
    }

    
    private void OnMisionesPressed()
    {
        
        menuPrincipalPanel.SetActive(false);

     
    }

    
    private void OnExitGamePressed()
    {
        Debug.Log("Salir del juego");
        Application.Quit();

        
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

   
    private void OnBackFromMisionesPressed()
    {
      
        

      
        menuPrincipalPanel.SetActive(true);
    }

    private void OnDestroy()
    {
       
        buttonBackToGame.onClick.RemoveListener(OnBackToGamePressed);
        buttonMisiones.onClick.RemoveListener(OnMisionesPressed);
        buttonExitGame.onClick.RemoveListener(OnExitGamePressed);
        buttonBackFromMisiones.onClick.RemoveListener(OnBackFromMisionesPressed);
    }
}
