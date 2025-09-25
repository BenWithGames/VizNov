using System.Collections;
using UnityEngine;

public class IntroScene : MonoBehaviour
{
    [Header("Cutscene Objects")]
    public GameObject fadeScreenIn;
    public GameObject charAnimeChick;

    [Header("Dialogue")]
    public DialogueUI dialogueUI;          // assign in Inspector
    public string startingDialogueId = "intro_01";

    void Start()
    {
        if (charAnimeChick != null)
            charAnimeChick.SetActive(false);

        StartCoroutine(EventStarter());
    }

    private IEnumerator EventStarter()
    {
        Debug.Log("Starting cutscene...");

        // Get dialogue from loader
        var dialogue = DialogueLoader.Instance?.GetDialogue(startingDialogueId);
        if (dialogue == null)
        {
            Debug.LogError($"IntroScene: Dialogue with id '{startingDialogueId}' not found!");
            yield break;
        }

        // Run sequence
        yield return new WaitForSeconds(1f);

        if (fadeScreenIn != null)
            fadeScreenIn.SetActive(false);

        if (charAnimeChick != null)
            charAnimeChick.SetActive(true);

        yield return new WaitForSeconds(0.2f);

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
