using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class DialogueUI : MonoBehaviour
{
    [Header("UI References (assign in prefab)")]
    [SerializeField] private GameObject panel;
    [SerializeField] private TextMeshProUGUI speakerText;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private Transform choicesContainer;
    [SerializeField] private GameObject choiceButtonPrefab;



    private DialogueData currentDialogue;
    private int currentLineIndex;
    private Coroutine typingCoroutine;
    private bool isTyping = false;

    [SerializeField] private AudioSource typeSoundSource; // shared AudioSource
    [SerializeField] private int charsPerSound = 2;
    [SerializeField] private List<SpeakerSound> speakerSounds = new List<SpeakerSound>();
    [SerializeField] private GameObject inputPanel;        // container with input UI
    [SerializeField] private TMP_InputField inputField;    // actual input box
    [SerializeField] private Button confirmButton;

    private AudioClip currentSpeakerClip;

    private string pendingInputKey;

    [System.Serializable]
    public class SpeakerSound
    {
        public string speakerName;
        public AudioClip soundClip;
    }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        if (panel != null)
        {
            panel.SetActive(false);
            Debug.Log($"[DialogueUI] Hiding panel {panel.name} on Awake");
        }

        if (inputPanel != null)
        {
            inputPanel.SetActive(false);
            Debug.Log($"[DialogueUI] Hiding inputPanel {inputPanel.name} on Awake");
        }
    }

    public void StartDialogue(DialogueData dialogue)
    {
        currentDialogue = dialogue;
        currentLineIndex = 0;

        if (!panel) return;  // safe guard if panel was not assigned
        panel.SetActive(true);
        ShowCurrentLine();
    }

    private void ShowCurrentLine()
    {
        if (currentDialogue == null || currentLineIndex >= currentDialogue.Lines.Count)
        {
            EndDialogue();
            return;
        }

        DialogueLine line = currentDialogue.Lines[currentLineIndex];
        speakerText.text = line.Speaker;

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        // pick speaker sound
        currentSpeakerClip = null;
        foreach (var mapping in speakerSounds)
        {
            if (mapping.speakerName == line.Speaker)
            {
                currentSpeakerClip = mapping.soundClip;
                break;
            }
        }

        float speed = (line.TypingSpeed > 0)
            ? line.TypingSpeed
            : (currentDialogue.DefaultSpeed > 0 ? currentDialogue.DefaultSpeed : 0.02f);

        typingCoroutine = StartCoroutine(TypeLine(line, speed));
    }

    private IEnumerator TypeLine(DialogueLine line, float speed)
    {
        isTyping = true;
        dialogueText.text = "";

        string processedText = ReplacePlaceholders(line.Text);
        int charCount = 0;

        foreach (char c in processedText)
        {
            dialogueText.text += c;
            charCount++;

            if (currentSpeakerClip != null && charCount % charsPerSound == 0)
            {
                typeSoundSource.PlayOneShot(currentSpeakerClip);
            }

            yield return new WaitForSeconds(speed);
        }

        isTyping = false;
        SpawnChoices(line);

        if (!string.IsNullOrEmpty(line.InputKey))
        {
            ShowInputPrompt(line.InputKey);
        }
    }


    private void SpawnChoices(DialogueLine line)
    {
        foreach (Transform child in choicesContainer)
            Destroy(child.gameObject);

        if (line.Choices != null && line.Choices.Count > 0)
        {
            foreach (var choice in line.Choices)
            {
                var choiceGO = Instantiate(choiceButtonPrefab, choicesContainer);
                choiceGO.GetComponentInChildren<TextMeshProUGUI>().text = choice.Text;

                choiceGO.GetComponent<Button>().onClick.AddListener(() =>
                {
                    var nextDialogue = DialogueLoader.Instance.GetDialogue(choice.Next);
                    StartDialogue(nextDialogue);
                });
            }
        }
    }

    public void NextLine()
    {
        currentLineIndex++;
        ShowCurrentLine();
    }

    private void EndDialogue()
    {
        if (panel) panel.SetActive(false);

        if (!string.IsNullOrEmpty(currentDialogue.NextScene))
        {
            Debug.Log("Loading next scene: " + currentDialogue.NextScene);
            SceneManager.LoadScene(currentDialogue.NextScene);
        }
    }

    private void Update()
    {
        if (!panel || !panel.activeInHierarchy) return;

        if (inputPanel != null && inputPanel.activeSelf) return;

        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (isTyping)
            {
                if (typingCoroutine != null) StopCoroutine(typingCoroutine);
                dialogueText.text = currentDialogue.Lines[currentLineIndex].Text;
                isTyping = false;
                SpawnChoices(currentDialogue.Lines[currentLineIndex]);
            }
            else if (choicesContainer.childCount == 0)
            {
                NextLine();
            }
        }
    }
    private void ShowInputPrompt(string key)
    {
        pendingInputKey = key;
        inputPanel.SetActive(true);
        inputField.text = "";

        // disable confirm until text entered
        if (confirmButton != null) confirmButton.interactable = false;

        inputField.onValueChanged.RemoveAllListeners();
        inputField.onValueChanged.AddListener((text) =>
        {
            if (confirmButton != null)
                confirmButton.interactable = !string.IsNullOrEmpty(text.Trim());
        });
    }

    public void ConfirmInput()
    {
        string value = inputField.text.Trim();
        if (string.IsNullOrEmpty(value))
        {
            Debug.Log("Name cannot be empty!");
            return;
        }

        PlayerPrefs.SetString(pendingInputKey, value);
        PlayerPrefs.Save();

        inputPanel.SetActive(false);
        NextLine();
    }

    private string ReplacePlaceholders(string text)
    {
        if (text.Contains("{PlayerName}"))
        {
            string name = PlayerPrefs.GetString("PlayerName", "Player");
            text = text.Replace("{PlayerName}", name);
        }
        return text;
    }

}
