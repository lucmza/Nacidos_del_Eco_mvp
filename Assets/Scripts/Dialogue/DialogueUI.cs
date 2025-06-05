using UnityEngine;
using TMPro;
using System.Collections;


public class DialogueUI : MonoBehaviour
{
    public GameObject panel;
    public TextMeshProUGUI speakerText;
    public TextMeshProUGUI dialogueText;
    [SerializeField] private float delay = 5f;

    private void Start()
    {
        StartCoroutine(DisableAfterTime());
    }
    public void SetDialogue(string speaker, string text)
    {
        speakerText.text = speaker;
        dialogueText.text = text;
    }

    public void ShowPanel(bool show)
    {
        panel.SetActive(show);
    }

    public bool IsDialogueActive()
    {
        return panel.activeSelf;
    }

    private IEnumerator DisableAfterTime()
    {
        yield return new WaitForSeconds(delay);
        panel.SetActive(false);
    }
}
