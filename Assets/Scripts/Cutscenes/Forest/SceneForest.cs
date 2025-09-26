using System.Collections;
using UnityEngine;

public class SceneForest : MonoBehaviour
{
    [Header("Cutscene Objects")]
    public GameObject fadeScreen;
    public GameObject patrolGuard;

    [Header("Audio Clips")]
    public AudioSource swordFight;

    [Header("Dialogue")]
    public DialogueUI dialogueUI;   // assign the DialogueUI instance in the Inspector
    public string startingDialogueId1 = "scene_forest_1";
    public string startingDialogueId2 = "scene_forest_2";

    private DialogueLoader loader;

    private void Start()
    {
        patrolGuard.SetActive(false);
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
        yield return new WaitForSeconds(.5f);
        patrolGuard.SetActive(true);
        yield return new WaitForSeconds(1.3f);

        yield return StartCoroutine(PlayDialogue(startingDialogueId2));
        swordFight.Play();

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
