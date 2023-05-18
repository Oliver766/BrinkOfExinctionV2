using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Dialog : MonoBehaviour
{
    public DialogText[] warpDialog;
    public DialogText[] gameDialog;

    public TextMeshProUGUI text;
    public GameObject next;
    public float speed = 0.03f;

    private bool isReading = false;
    private Coroutine coroutine;

    private int currentParagraphCount;

    public Animator anim;
    public GameController gameController;

    private bool dialogActive = false;
    private bool isWarpDialog;
    private DialogText currentDialog;

    public IEnumerator TriggerDialog(int selection, bool isWarp, float delay)
    {
        text.text = null;

        yield return new WaitForSeconds(delay);

        anim.Play("Open");

        yield return new WaitForSeconds(2f);

        dialogActive = true;

        currentParagraphCount = 0;

        isWarpDialog = isWarp;
        currentDialog = isWarp ? warpDialog[selection] : gameDialog[selection];

        coroutine = StartCoroutine(DisplayText());
    }

    private IEnumerator DisplayText()
    {
        isReading = true;
        next.SetActive(false);

        for (int i = 0; i < currentDialog.paragraphs[currentParagraphCount].Length; i++)
        {
            text.text += currentDialog.paragraphs[currentParagraphCount][i];

            yield return new WaitForSeconds(speed);
        }


        isReading = false;
        next.SetActive(true);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && dialogActive)
        {
            // Skip to end of text if dialogue is being read
            if (isReading)
            {
                StopCoroutine(coroutine);
                text.text = currentDialog.paragraphs[currentParagraphCount];
                isReading = false;
                next.SetActive(true);
            }
            // Proceed to next paragraph
            else if (currentParagraphCount < currentDialog.paragraphs.Length - 1)
            {
                currentParagraphCount++;
                text.text = null;
                coroutine = StartCoroutine(DisplayText());
            }
            // If the end of the dialogue is reached, close the diaglogue and trigger an event (if applicable)
            else
            {
                dialogActive = false;
                next.SetActive(false);
                anim.Play("Close");

                if (isWarpDialog)
                {
                    Invoke(nameof(StartGame), 2f);
                }
            }
        }
    }

    private void StartGame()
    {
        gameController.StartGame();
    }
}

