using System.Collections;
using UnityEngine;

public class IntroScene : MonoBehaviour
{
    [Header("Cutscene Objects")]
    public GameObject fadeScreenIn;
    public GameObject deitySprite;

    [Header("Dialogue")]
    public DialogueUI dialogueUI;          // assign the DialogueUI instance in Inspector
    public string startingDialogueId = "intro_first_scene";

    private DialogueLoader loader;

    private void Start()
    {
        if (deitySprite != null)
            deitySprite.SetActive(false);

        // grab the DialogueLoader from the scene
        loader = Object.FindFirstObjectByType<DialogueLoader>();
        if (loader == null)
        {
            Debug.LogError("IntroScene: No DialogueLoader found in this scene!");
            return;
        }

        StartCoroutine(EventStarter());
    }

    private IEnumerator EventStarter()
    {
        Debug.Log("Starting cutscene...");

        // Load dialogue from YAML
        var dialogue = loader.GetDialogue(startingDialogueId);
        if (dialogue == null)
        {
            Debug.LogError($"IntroScene: Dialogue with id '{startingDialogueId}' not found!");
            yield break;
        }

        // Small delay before fade
        yield return new WaitForSeconds(1f / 0.06f);

        if (fadeScreenIn != null)
            fadeScreenIn.SetActive(false);

        if (deitySprite != null)
            deitySprite.SetActive(true);

        yield return new WaitForSeconds(1f);

        // Start dialogue
        if (dialogueUI != null)
        {
            dialogueUI.StartDialogue(dialogue);
        }
        else
        {
            Debug.LogError("IntroScene: DialogueUI is not assigned in the Inspector!");
        }
    }
}
