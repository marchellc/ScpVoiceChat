using System.Collections.Generic;

using LabExtended.API.Settings;
using LabExtended.API.Settings.Menus;
using LabExtended.API.Settings.Entries;

using ScpVoiceChat.Voice;

namespace ScpVoiceChat.Settings;

public class ScpVoiceMenu : SettingsMenu
{
    public override string CustomId { get; } = "scpVoiceMenu";
    public override string Header => ScpVoiceConfig.Instance.MenuLabel;

    public ScpVoiceSizeSlider SizeSlider => new ScpVoiceSizeSlider();
    public ScpVoiceOffsetSlider OffsetSlider => new ScpVoiceOffsetSlider();
    public ScpVoiceAlignDropdown AlignDropdown => new ScpVoiceAlignDropdown();
    
    public override void BuildMenu(List<SettingsEntry> settings)
    {
        settings
            .WithEntry(OffsetSlider)
            .WithEntry(SizeSlider)
            .WithEntry(AlignDropdown)
            
            .WithEntry(SettingsKeyBind.Create("scpVoiceMenu.bindVoice", 
                ScpVoiceConfig.Instance.KeyBindLabel,
                ScpVoiceConfig.Instance.KeyBindSuggestKey,
                
                true,
                
                ScpVoiceConfig.Instance.KeyBindHint));
    }

    public override void OnKeyBindPressed(SettingsKeyBind keyBind)
    {
        base.OnKeyBindPressed(keyBind);

        if (!Player)
            return;

        if (keyBind.CustomId != "scpVoiceMenu.bindVoice" || !keyBind.IsPressed)
            return;

        var profile = Player.Voice.GetProfile<ScpVoiceProfile>();

        if (profile is null)
            return;
        
        profile.ToggleState();
    }
}