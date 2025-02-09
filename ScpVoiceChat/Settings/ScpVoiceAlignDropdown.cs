using LabExtended.API.Enums;
using LabExtended.API.Settings.Entries.Dropdown;
using LabExtended.Core;
using ScpVoiceChat.Voice;
using UserSettings.ServerSpecific;

namespace ScpVoiceChat.Settings;

public class ScpVoiceAlignDropdown : SettingsDropdown
{
    public ScpVoiceAlignDropdown() : base("scpVoiceMenu.alignDropdown",
        ScpVoiceConfig.Instance.AlignDropdownLabel,
        0,
        SSDropdownSetting.DropdownEntryType.Regular,
        BuildDropdown,
        ScpVoiceConfig.Instance.AlignDropdownHint)
    { }

    public override void HandleSelection(SettingsDropdownOption previous, SettingsDropdownOption option)
    {
        base.HandleSelection(previous, option);

        if (!Player || !ScpVoiceProfile.Profiles.TryGetValue(Player, out var profile))
            return;

        profile.Settings.Align = (HintAlign)option.Data;
        
        ApiLog.Debug("Scp Voice", $"Player &1{Player.Name}&r (&6{Player.UserId}&r) changed their hint alignment to &1{profile.Settings.Align}&r");
    }

    private static void BuildDropdown(SettingsDropdown settingsDropdown)
    {
        var centerText = string.Empty;
        var leftText = string.Empty;
        var rightText = string.Empty;
        var fullLeftText = string.Empty;
        
        if (ScpVoiceConfig.Instance.Translations.TryGetValue(HintAlign.Center, out var centerTrans) && !string.IsNullOrWhiteSpace(centerTrans))
            centerText = centerTrans;
        else
            centerText = "Center";
        
        if (ScpVoiceConfig.Instance.Translations.TryGetValue(HintAlign.Left, out var leftTrans) && !string.IsNullOrWhiteSpace(leftTrans))
            leftText = leftTrans;
        else
            leftText = "Left";
        
        if (ScpVoiceConfig.Instance.Translations.TryGetValue(HintAlign.Right, out var rightTrans) && !string.IsNullOrWhiteSpace(rightTrans))
            rightText = rightTrans;
        else
            rightText = "Right";
        
        if (ScpVoiceConfig.Instance.Translations.TryGetValue(HintAlign.FullLeft, out var fullLeftTrans) && !string.IsNullOrWhiteSpace(fullLeftTrans))
            fullLeftText = fullLeftTrans;
        else
            fullLeftText = "Full Left";

        settingsDropdown.AddOption(HintAlign.Center, centerText);
        settingsDropdown.AddOption(HintAlign.Left, leftText);
        settingsDropdown.AddOption(HintAlign.Right, rightText);
        settingsDropdown.AddOption(HintAlign.FullLeft, fullLeftText);
    }
}