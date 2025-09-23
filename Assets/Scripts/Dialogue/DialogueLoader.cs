using UnityEngine;
using System.Collections.Generic;
using System.IO;
using YamlDotNet.Serialization;

public class DialogueLoader : MonoBehaviour
{
    public static DialogueLoader Instance { get; private set; }

    public TextAsset yamlFile;   // drag your YAML file here

    private Dictionary<string, DialogueData> dialogueDatabase;

    private void Awake()
    {
        // Singleton setup
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // keep between scenes
            LoadDialogue();
        }
        else
        {
            Destroy(gameObject); // only one loader allowed
        }
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
    }

    public DialogueData GetDialogue(string id)
    {
        if (dialogueDatabase.TryGetValue(id, out var data))
            return data;

        Debug.LogError($"Dialogue with id {id} not found!");
        return null;
    }
}
