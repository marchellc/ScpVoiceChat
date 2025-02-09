using System.ComponentModel;

using LabExtended.API.Enums;

using YamlDotNet.Serialization;

namespace ScpVoiceChat;

public class ScpVoicePersonalSettings
{
    [Description("Sets the hint alignment.")]
    public HintAlign Align { get; set; } = HintAlign.Center;

    [Description("Sets the hint's size.")]
    public int Size { get; set; } = 0;

    [Description("Sets the hint's vertical offset (height).")]
    public float VerticalOffset { get; set; } = 0f;

    [Description("Sets the text to display ({0} will be replaced with player's current mode).")]
    public string Template { get; set; } = "Your current SCP voice mode is {0}";
}