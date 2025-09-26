// using System.Collections;
// using UnityEngine;

// public class NextScene : MonoBehaviour
// {
//     [Header("Cutscene Objects")]
//     public GameObject fadeScreenIn;
//     public GameObject charAnimeChick;

//     [Header("Dialogue")]
//     public DialogueUI dialogueUI;          // assign in Inspector
//     public string startingDialogueId = "intro_02"; 

//     void Start()
//     {
//         if (charAnimeChick != null)
//             charAnimeChick.SetActive(false);

//         StartCoroutine(EventStarter());
//     }

//     private IEnumerator EventStarter()
//     {
//         Debug.Log("NextScene cutscene starting...");

//         var dialogue = DialogueLoader.Instance?.GetDialogue(startingDialogueId);
//         if (dialogue == null)
//         {
//             Debug.LogError($"NextScene: Dialogue with id '{startingDialogueId}' not found!");
//             yield break;
//         }

//         yield return new WaitForSeconds(1f);

//         if (fadeScreenIn != null)
//             fadeScreenIn.SetActive(false);

//         if (charAnimeChick != null)
//             charAnimeChick.SetActive(true);

//         yield return new WaitForSeconds(0.2f);

//         if (dialogueUI != null)
//         {
//             dialogueUI.StartDialogue(dialogue);
//         }
//         else
//         {
//             Debug.LogError("NextScene: DialogueUI is not assigned in the Inspector!");
//         }
//     }
// }
