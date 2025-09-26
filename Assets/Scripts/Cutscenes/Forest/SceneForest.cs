using System.Collections;
using UnityEngine;

public class SceneForest : MonoBehaviour
{
    [Header("Cutscene Objects")]
    public GameObject fadeScreen;

    [Header("Dialogue")]
    public DialogueUI dialogueUI;   // assign the DialogueUI instance in the Inspector
    public string startingDialogueId1 = "scene_forest_1";
    public string startingDialogueId2 = "scene_forest_2";
    public string startingDialogueId3 = "scene_forest_3";
    public string startingDialogueId4 = "scene_forest_4";

    private DialogueLoader loader;

    private void Start()
    {
        loader = Object.FindFirstObjectByType<DialogueLoader>();
        if (loader == null)
        {
            Debug.LogError("SceneForest: No DialogueLoader found in this scene!");
            return;
        }

        StartCoroutine(EventStarter());
    }

    private IEnumerator EventStarter()
    {
        Debug.Log("Starting Scene");

        yield return new WaitForSeconds(2f);

        if (fadeScreen != null)
            fadeScreen.SetActive(false);

        // Play dialogues in sequence
        yield return StartCoroutine(PlayDialogue(startingDialogueId1));

        // do something else here (animations, fades, etc.)
        yield return new WaitForSeconds(1f);

        yield return StartCoroutine(PlayDialogue(startingDialogueId2));
        yield return StartCoroutine(PlayDialogue(startingDialogueId3));
        yield return StartCoroutine(PlayDialogue(startingDialogueId4));

        Debug.Log("All dialogues finished!");
    }

    private IEnumerator PlayDialogue(string dialogueId)
    {
        var dialogue = loader.GetDialogue(dialogueId);
        if (dialogue == null)
        {
            Debug.LogError($"SceneForest: Dialogue with id '{dialogueId}' not found!");
            yield break;
        }

        if (dialogueUI == null)
        {
            Debug.LogError("SceneForest: DialogueUI is not assigned in the Inspector!");
            yield break;
        }

        bool finished = false;
        System.Action endHandler = () => finished = true;

        dialogueUI.OnDialogueEnd += endHandler;
        dialogueUI.StartDialogue(dialogue);

        yield return new WaitUntil(() => finished);

        dialogueUI.OnDialogueEnd -= endHandler;
    }
}
