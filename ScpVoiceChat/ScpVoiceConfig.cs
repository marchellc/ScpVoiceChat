using System.Collections.Generic;
using System.ComponentModel;

using LabExtended.API.Enums;

using PlayerRoles;

using UnityEngine;

namespace ScpVoiceChat
{
    public class ScpVoiceConfig
    {
        public static ScpVoiceConfig Instance { get; internal set; }

        [Description("The default personal voice settings.")]
        public ScpVoicePersonalSettings DefaultSettings { get; set; } = new ScpVoicePersonalSettings();

        [Description("Translations for hint alignment options.")]
        public Dictionary<HintAlign, string> Translations { get; set; } = new Dictionary<HintAlign, string>()
        {
            [HintAlign.Left] = "Left",
            [HintAlign.Center] = "Center",
            [HintAlign.Right] = "Right",
            [HintAlign.FullLeft] = "Full Left"
        };

        [Description("A list of roles that will not be able to use SCP voice chat.")]
        public List<RoleTypeId> BlacklistedRoles { get; set; } = new List<RoleTypeId>() { RoleTypeId.Scp079 };

        [Description("The label for the settings menu.")]
        public string MenuLabel { get; set; } = "Scp Voice Chat Settings";

        [Description("The label for the offset slider.")]
        public string OffsetSliderLabel { get; set; } = "Vertical Offset";

        [Description("The hint to show when hovering over the offset slider.")]
        public string OffsetSliderHint { get; set; } = "Controls the vertical offset (height) of the displayed hint.";
        
        [Description("The label for the size slider.")]
        public string SizeSliderLabel { get; set; } = "Size";

        [Description("The hint to show when hovering over the size slider.")]
        public string SizeSliderHint { get; set; } = "Controls the size of the displayed hint.";

        [Description("The label for the alignment dropdown.")]
        public string AlignDropdownLabel { get; set; } = "Alignment";

        [Description("The hint to show when hovering over the alignment dropdown.")]
        public string AlignDropdownHint { get; set; } = "Sets the hint's alignment.";

        [Description("The label for the key bind.")]
        public string KeyBindLabel { get; set; } = "Key Bind";

        [Description("The hint to show when hovering over the key bind.")]
        public string KeyBindHint { get; set; } = "The button you want to use to toggle between different voice chat modes.";

        [Description("The key code to suggest for the voice key bind.")]
        public KeyCode KeyBindSuggestKey { get; set; } = KeyCode.LeftAlt;
        
        [Description("The minimum distance from the speaker.")]
        public float MinSpeakerDistance { get; set; } = 5f;
        
        [Description("The maximum distance from the speaker.")]
        public float MaxSpeakerDistance { get; set; } = 20f;
        
        [Description("Volume of the speaker.")]
        public float SpeakerVolume { get; set; } = 1f;

        [Description("Volume multiplier.")]
        public int VolumeMultiplier { get; set; } = 20;

        [Description("Whether or not to use spatial audio.")]
        public bool UseSpatialAudio { get; set; } = true;
    }
}