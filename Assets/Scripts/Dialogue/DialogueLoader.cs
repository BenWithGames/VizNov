using UnityEngine;
using System.Collections.Generic;
using System.IO;
using YamlDotNet.Serialization;

public class DialogueLoader : MonoBehaviour
{
    [Header("YAML File")]
    [Tooltip("Assign a YAML file for this scene's cutscene/dialogue")]
    public TextAsset yamlFile;

    private Dictionary<string, DialogueData> dialogueDatabase;

    private void Awake()
    {
        if (yamlFile == null)
        {
            Debug.LogError("DialogueLoader: No YAML file assigned!");
            return;
        }

        LoadDialogue();
    }

    private void LoadDialogue()
    {
        var deserializer = new DeserializerBuilder().Build();

        using (StringReader reader = new StringReader(yamlFile.text))
        {
            var dialogues = deserializer.Deserialize<List<DialogueData>>(reader);

            dialogueDatabase = new Dictionary<string, DialogueData>();
            foreach (var dialogue in dialogues)
            {
                dialogueDatabase[dialogue.Id] = dialogue;
            }
        }

        Debug.Log($"DialogueLoader: Loaded {dialogueDatabase.Count} dialogues from {yamlFile.name}");
    }

    public DialogueData GetDialogue(string id)
    {
        if (dialogueDatabase == null)
        {
            Debug.LogError("DialogueLoader: Database not loaded.");
            return null;
        }

        if (dialogueDatabase.TryGetValue(id, out var data))
            return data;

        Debug.LogError($"DialogueLoader: Dialogue with id '{id}' not found!");
        return null;
    }
}
