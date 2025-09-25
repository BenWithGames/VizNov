using System.Collections;
using UnityEngine;

public class SceneForest : MonoBehaviour
{
    [Header("Cutscene Objects")]
    public GameObject fadeScreen;

    [Header("Dialogue")]
    public DialogueUI dialogueUI;
    public string startingDialogueId1 = "scene_forest_1";
    public string startingDialogueId2 = "scene_forest_2";
    public string startingDialogueId3 = "scene_forest_3";
    public string startingDialogueId4 = "scene_forest_4";

    void Start()
    {
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
        var dialogue = DialogueLoader.Instance?.GetDialogue(dialogueId);
        if (dialogue == null)
        {
            Debug.LogError($"Dialogue with id '{dialogueId}' not found!");
            yield break;
        }

        DialogueUI ui = DialogueUI.Instance;
        if (ui == null)
        {
            Debug.LogError("DialogueUI.Instance is missing!");
            yield break;
        }

        bool finished = false;
        System.Action endHandler = () => finished = true;

        ui.OnDialogueEnd += endHandler;
        ui.StartDialogue(dialogue);

        yield return new WaitUntil(() => finished);

        ui.OnDialogueEnd -= endHandler;
    }
}
