// using System.Collections;
// using UnityEngine;

// public class FirstScene : MonoBehaviour
// {

//     [Header("Cutscene Objects")]
//     public GameObject fadeScreen;
//     public GameObject dietySprite;

//     [Header("Dialogue")]
//     public DialogueUI dialogueUI;
//     public string startingDialogueId = "intro_first_scene";
//     // Start is called once before the first execution of Update after the MonoBehaviour is created
//     void Start()
//     {
//         dietySprite.SetActive(false);
//         StartCoroutine(EventStarter());
//     }

//     private IEnumerator EventStarter()
//     {
//         Debug.Log("Starting First Scene");
//         var dialogue = DialogueLoader.Instance?.GetDialogue(startingDialogueId);
//         if (dialogue == null)
//         {
//             Debug.LogError($"IntroScene: Dialogue with id '{startingDialogueId}' not found!");
//             yield break;
//         }
//         yield return new WaitForSeconds(1f / 0.06f);

//         if (fadeScreen != null)
//             fadeScreen.SetActive(false);
//         dietySprite.SetActive(true);

//         yield return new WaitForSeconds(1f);

//         if (dialogueUI != null)
//         {
//             dialogueUI.StartDialogue(dialogue);
//         }
//         else
//         {
//             Debug.LogError("IntroScene: DialogueUI is not assigned in the Inspector!");
//         }

//     }

// }
