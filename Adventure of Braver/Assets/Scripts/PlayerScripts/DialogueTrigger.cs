using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour {

    public Dialogue dialogue;

    private void OnTriggerEnter(Collider other)
    {
        if( other.tag == "Player")
        {
            TriggerDialogue();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if( other.tag == "Player")
        {
            StopDialogue();
        }
    }

    public void TriggerDialogue()
    {
        FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
    }

    public void StopDialogue()
    {
        FindObjectOfType<DialogueManager>().EndDialogue();
    }
}
