using System.Collections.Generic;
using YamlDotNet.Serialization;

[System.Serializable]
public class DialogueData
{
    [YamlMember(Alias = "id")]
    public string Id { get; set; }

    [YamlMember(Alias = "defaultSpeed")]
    public float DefaultSpeed { get; set; } = 0.02f;

    [YamlMember(Alias = "lines")]
    public List<DialogueLine> Lines { get; set; }

    [YamlMember(Alias = "nextScene")]
    public string NextScene { get; set; }
}

[System.Serializable]
public class DialogueLine
{
    [YamlMember(Alias = "speaker")]
    public string Speaker { get; set; }

    [YamlMember(Alias = "text")]
    public string Text { get; set; }

    [YamlMember(Alias = "choices")]
    public List<DialogueChoice> Choices { get; set; }

    [YamlMember(Alias = "speed")]
    public float TypingSpeed { get; set; } = -1f;

    [YamlMember(Alias = "input")]
    public string InputKey;
}

[System.Serializable]
public class DialogueChoice
{
    [YamlMember(Alias = "text")]
    public string Text { get; set; }

    [YamlMember(Alias = "next")]
    public string Next { get; set; }
}
