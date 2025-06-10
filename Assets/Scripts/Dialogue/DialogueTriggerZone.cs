using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Collections;

public class DialogueTriggerZone : MonoBehaviour
{
    [Header("Diálogo")]
    public List<DialogueLine> lines;
    [Header("UI")]
    public DialogueUI dialogueUI;
    [Header("Trigger")]
    public bool autoTrigger = false;
    public bool oneTimeOnly = true;
    [Header("Submit Action (Nuevo Input System)")]
    public InputActionReference submitActionReference;

    private InputAction submitAction;
    private PlayerInput playerInput;
    private PlayerMovement playerMovement;
    private bool playerInRange;
    private bool hasBeenTriggered;
    private int currentLineIndex;
   

    void OnEnable()
    {
        submitAction = submitActionReference?.action;
        submitAction?.Enable();
    }

    void OnDisable()
    {
        submitAction?.Disable();
    }

    void Update()
    {
        if (!playerInRange) return;

        if (dialogueUI.IsDialogueActive())
        {
            if (submitAction.triggered)
            {
                currentLineIndex++;
                if (currentLineIndex >= lines.Count) EndDialogue();
                else ShowLine();
            }
            return;
        }

        if (hasBeenTriggered) return;

        if (autoTrigger || submitAction.triggered)
            StartDialogue();
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        playerInRange = true;

        
       // playerMovement = other.GetComponent<PlayerMovement>();
      
        //if (playerMovement == null)
           // playerMovement = other.GetComponentInParent<PlayerMovement>();

        playerInput = other.GetComponent<PlayerInput>();

        if (autoTrigger && !hasBeenTriggered)
        { 

        StartDialogue();
            Destroy(gameObject, 10f);

        }
    }


    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInRange = false;
    }

    void StartDialogue()
    {
        hasBeenTriggered = oneTimeOnly;
        currentLineIndex = 0;

       // if (playerMovement != null)
        //{
          //  playerMovement.enabled = false;
        //}

        dialogueUI.ShowPanel(true);
        ShowLine();
        

    }
    

    void ShowLine()
    {
        var line = lines[currentLineIndex];
        dialogueUI.SetDialogue(line.speakerName, line.text);
    }

    void EndDialogue()
    {
        dialogueUI.ShowPanel(false);

        if (playerMovement != null)
        {
            playerMovement.enabled = true;
        }

        if (autoTrigger && oneTimeOnly)
            Destroy(gameObject);
    }

   
}
