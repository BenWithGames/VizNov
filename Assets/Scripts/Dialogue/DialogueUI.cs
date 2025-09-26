using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using TMPro;

public class DialogueUI : MonoBehaviour
{
    [Header("UI References (assign in prefab)")]
    [SerializeField] private GameObject panel;               // Dialogue panel
    [SerializeField] private TextMeshProUGUI speakerText;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private Transform choicesContainer;
    [SerializeField] private GameObject choiceButtonPrefab;
    [SerializeField] private GameObject narrationPanel;      // Narration panel
    [SerializeField] private TextMeshProUGUI narrationText;

    private DialogueData currentDialogue;
    private int currentLineIndex;
    private Coroutine typingCoroutine;
    private bool isTyping = false;

    [Header("Audio Typing Settings")]
    [SerializeField] private AudioSource typeSoundSource; // shared AudioSource
    [SerializeField] private int charsPerSound = 2;
    [SerializeField] private List<SpeakerSound> speakerSounds = new List<SpeakerSound>();
    [SerializeField] private AudioClip narrationClip;

    [Header("Input Prompt")]
    [SerializeField] private GameObject inputPanel;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private Button confirmButton;

    [Header("Dialogue Loader (scene-local)")]
    [SerializeField] private DialogueLoader loader; // assign in Inspector or auto-find

    private AudioClip currentSpeakerClip;
    private string pendingInputKey;

    public event System.Action OnDialogueEnd;

    [System.Serializable]
    public class SpeakerSound
    {
        public string speakerName;
        public AudioClip soundClip;
    }

    private void Awake()
    {
        if (panel != null) panel.SetActive(false);
        if (narrationPanel != null) narrationPanel.SetActive(false);
        if (inputPanel != null) inputPanel.SetActive(false);

        // Auto-find DialogueLoader if not assigned
        if (loader == null)
            loader = Object.FindFirstObjectByType<DialogueLoader>();
    }

    public void StartDialogue(DialogueData dialogue)
    {
        currentDialogue = dialogue;
        currentLineIndex = 0;
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

        // Hide both before deciding
        if (panel != null) panel.SetActive(false);
        if (narrationPanel != null) narrationPanel.SetActive(false);

        if (!string.IsNullOrEmpty(line.Speaker))
        {
            // Character dialogue
            if (panel != null) panel.SetActive(true);
            if (speakerText != null) speakerText.text = line.Speaker;

            if (typingCoroutine != null) StopCoroutine(typingCoroutine);

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
        else
        {
            // Narration
            if (narrationPanel != null) narrationPanel.SetActive(true);

            if (typingCoroutine != null) StopCoroutine(typingCoroutine);

            float speed = (line.TypingSpeed > 0)
                ? line.TypingSpeed
                : (currentDialogue.DefaultSpeed > 0 ? currentDialogue.DefaultSpeed : 0.02f);

            typingCoroutine = StartCoroutine(TypeNarration(line, speed));
        }
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

    private IEnumerator TypeNarration(DialogueLine line, float speed)
    {
        isTyping = true;
        narrationText.text = "";

        string processedText = ReplacePlaceholders(line.Text);
        int charCount = 0;

        foreach (char c in processedText)
        {
            narrationText.text += c;
            charCount++;

            if (narrationClip != null && charsPerSound > 0 && charCount % charsPerSound == 0)
            {
                typeSoundSource.PlayOneShot(narrationClip);
            }

            yield return new WaitForSeconds(speed);
        }

        isTyping = false;
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

                // Replace placeholders here 👇
                var label = choiceGO.GetComponentInChildren<TextMeshProUGUI>();
                if (label != null)
                    label.text = ReplacePlaceholders(choice.Text);

                var btn = choiceGO.GetComponent<Button>();
                if (btn != null)
                {
                    string nextId = choice.Next; // capture
                    btn.onClick.AddListener(() =>
                    {
                        var nextDialogue = FindFirstObjectByType<DialogueLoader>()?.GetDialogue(nextId);
                        if (nextDialogue != null)
                            StartDialogue(nextDialogue);
                    });
                }
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
        if (narrationPanel) narrationPanel.SetActive(false);

        if (!string.IsNullOrEmpty(currentDialogue.NextScene))
        {
            SceneManager.LoadScene(currentDialogue.NextScene);
        }

        OnDialogueEnd?.Invoke();
    }

    private void Update()
    {
        if ((panel == null || !panel.activeInHierarchy) &&
            (narrationPanel == null || !narrationPanel.activeInHierarchy))
            return;

        if (inputPanel != null && inputPanel.activeSelf) return;

        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (isTyping)
            {
                if (typingCoroutine != null) StopCoroutine(typingCoroutine);

                DialogueLine line = currentDialogue.Lines[currentLineIndex];

                if (!string.IsNullOrEmpty(line.Speaker))
                {
                    dialogueText.text = ReplacePlaceholders(line.Text);
                    SpawnChoices(line);
                }
                else
                {
                    narrationText.text = ReplacePlaceholders(line.Text);
                }

                isTyping = false;
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
        EventSystem.current.SetSelectedGameObject(inputField.gameObject);

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
