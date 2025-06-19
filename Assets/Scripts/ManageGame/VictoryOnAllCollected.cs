using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryOnAllCollected : MonoBehaviour
{
    [SerializeField] private string sceneToLoad = "Victory";

    private void Start()
    {
        if (FragmentManager.Instance != null)
            FragmentManager.Instance.OnAllFragmentsCollected += LoadVictoryScene;
    }


    private void OnDisable()
    {
        if (FragmentManager.Instance != null)
            FragmentManager.Instance.OnAllFragmentsCollected -= LoadVictoryScene;
    }


    private void LoadVictoryScene()
    {
        SceneManager.LoadScene(sceneToLoad);
    }
}
