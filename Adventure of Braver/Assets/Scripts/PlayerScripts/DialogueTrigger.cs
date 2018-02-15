using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour {

    public Dialogue dialogue;
    public bool isTalking;
    [SerializeField]Camera dialogueCamera;
    [SerializeField]Camera mainCamera;

    private void Start()
    {
        dialogueCamera.enabled = false;
        mainCamera.enabled = true;
    }

    private void OnTriggerStay(Collider other)
    {
        if( other.tag == "Player" && Input.GetAxis("Action") > 0)
        {
            isTalking = true;
            TriggerDialogue();
        }

        if( Input.GetAxis("Cancel") > 0){
            isTalking = false;
            StopDialogue();
        }
    }

    public void TriggerDialogue()
    {
        dialogueCamera.enabled = true;
        mainCamera.enabled = false;
        FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
    }

    public void StopDialogue()
    {
        dialogueCamera.enabled = false;
        mainCamera.enabled = true;
        FindObjectOfType<DialogueManager>().EndDialogue();
    }
}
