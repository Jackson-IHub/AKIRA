using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using System.Collections;
public class DialogueManager : MonoBehaviour
{
    private bool onStart = false;
    public bool dialogueFinished = false;
    public TextAsset textFileStart;

    public TextMeshProUGUI adultTextMesh;
    public TextMeshProUGUI childTextMesh;

    public GameObject adultTextBubble;
    public GameObject childTextBubble;

    private Queue<string> dialogue = new Queue<string>();
    private List<int> whoIsSpeaking = new List<int>();

    private int lineNumber = 0;

    string constructedLine;

    public bool finishedRound = false;

    private bool didDialogueJustStart = false;

    private bool isTyping = false;

    public Animator adultAnimator;
    public Animator childAnimator;
    string animationTag = "";

    private bool animationPlaying;
    private void PrintDialogue()
    {
        if (dialogue.Count == 0)
        {
            Debug.Log("end of dialogue");
            EndDialogue();
            return;
        }
        adultTextMesh.text = "";
        childTextMesh.text = "";
        StartCoroutine(PrintOutText());
        if (lineNumber == 0)
        {
            childTextBubble.SetActive(false);

        }
        else
        {
            adultTextBubble.SetActive(false);
        }
    }

    private IEnumerator PrintOutText()
    {
        int numberOfCharacters = dialogue.Peek().Length;
        isTyping = true;
        bool foundTag = false;

        for (int i = 0; i < numberOfCharacters; i++)
        {
            if (dialogue.Peek()[i] == '<' || foundTag == true)
            {
                foundTag = true;
                if (dialogue.Peek()[i] == '>')
                {
                    foundTag = false;
                    animationTag = animationTag.Remove(0,1);
                    Debug.Log("Final animation tag " + animationTag);
                }
                else
                {
                    animationTag += dialogue.Peek()[i];
                }
                if(foundTag == false)
                {
                    i++;
                }
                yield return new WaitForSeconds(0f);
            }
            yield return new WaitForSeconds(0.005f);
            if (whoIsSpeaking[lineNumber] == 1 && foundTag == false)
            {
                if (animationTag == "Anger" && animationPlaying == false)
                {
                    animationPlaying = true;
                    adultAnimator.Play("Anger");
                }
                adultTextMesh.text += dialogue.Peek()[i];
                adultTextBubble.SetActive(true);
                childTextBubble.SetActive(false);
            }
            else if(whoIsSpeaking[lineNumber] != 1 && foundTag == false)
            {
                if (animationTag == "Sadness")
                {
                    childAnimator.Play("Sadness");
                }
                childTextMesh.text += dialogue.Peek()[i];
                childTextBubble.SetActive(true);
                adultTextBubble.SetActive(false);
            }
        }
        lineNumber++;
        dialogue.Dequeue();
        animationTag = "";
        isTyping = false;
        animationPlaying = false;
    }


    private void EndDialogue()
    {
        adultTextMesh.text = "";
        childTextMesh.text = "";
        adultTextBubble.SetActive(false);
        childTextBubble.SetActive(false);
        dialogue.Clear();
        dialogueFinished = true;
    }

    public void AdvanceDialogue() // call when a player presses a button in Dialogue Trigger
    {
        PrintDialogue();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space) && onStart == false)
        {
            onStart = true;
            ReadTextFile();
        }
        if (Input.GetKeyUp(KeyCode.Space) && dialogueFinished == false && isTyping == false)
        {
            AdvanceDialogue();
        }

    }

    private void ReadTextFile() // skip // 
    {
        didDialogueJustStart = true;
        dialogueFinished = false;
        lineNumber = 0;

        string txt;
        txt = textFileStart.text;
        

        string[] lines = txt.Split(System.Environment.NewLine.ToCharArray()); // Split dialogue lines by newline

        foreach (string line in lines) // for every line of dialogue
        {
            if (!string.IsNullOrEmpty(line))// ignore empty lines of dialogue
            {
                if (line.StartsWith("Dad: ")) 
                {
                    string curr = line.Substring(line.IndexOf(':') + 1); 
                    dialogue.Enqueue(curr);
                    whoIsSpeaking.Add(1);
                }
                else if (line.StartsWith("Son: "))
                {
                    string curr = line.Substring(line.IndexOf(':') + 1); 
                    dialogue.Enqueue(curr);
                    whoIsSpeaking.Add(0);
                }
                else
                {
                    dialogue.Enqueue(line);
                    whoIsSpeaking.Add(whoIsSpeaking[whoIsSpeaking.Count - 1]);
                }
            }
        }

        AdvanceDialogue();

    }




}