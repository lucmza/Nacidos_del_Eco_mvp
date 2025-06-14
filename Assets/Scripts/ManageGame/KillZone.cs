using UnityEngine;
using UnityEngine.SceneManagement;


public class KillZone : MonoBehaviour
{
    [Tooltip("Nombre de la escena Game Over")]
    public string gameOverScene = "GameOver";

    void Reset()
    {
      
        Collider c = GetComponent<Collider>();
        c.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
         
            var pm = other.GetComponent<PlayerMovement>();
            if (pm != null) pm.enabled = false;

           
            SceneManager.LoadScene(gameOverScene);
        }
    }
}
