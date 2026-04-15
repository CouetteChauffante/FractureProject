using System;
using System.Collections.Generic;
using UnityEngine;

public class DialogManager : MonoBehaviour
{
    private Queue<string> sentences;
    public TriggerEvent TriggerEvent;
    public static DialogManager instance;
    public TMPro.TMP_Text nameText;
    public TMPro.TMP_Text phrasesText;

    void Start()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            Debug.Log("the canvas just fucking died");
        }
        instance = this;
        
        sentences = new Queue<string>();
        Debug.Log(sentences.Count);
    }

    public void StartDialogue(Dialogs dialogs)
    {
        Debug.Log("Starting to chat with " + dialogs.name);
        nameText.text = dialogs.name;
        sentences.Clear();

        foreach (string sentence in dialogs.sentences)
        {
            sentences.Enqueue(sentence);
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log("tkt ça marche");
                sentences.Dequeue();
                Debug.Log(sentence);
            }
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        string sentence = sentences.Dequeue();
        phrasesText.text = sentence;
        Debug.Log(sentence);
    }

    public void EndDialogue()
    {
        Debug.Log("Finished chatting.");
    }
}
