using LabExtended.API.Settings.Entries;
using LabExtended.Core;
using ScpVoiceChat.Voice;
using UnityEngine;

namespace ScpVoiceChat.Settings;

public class ScpVoiceSizeSlider : SettingsSlider
{
    public ScpVoiceSizeSlider() : base("scpVoiceMenu.sizeSlider",
        ScpVoiceConfig.Instance.SizeSliderLabel,
        10f,
        30f,
        
        0f,

        true,

        "0.##",
        "{0}",

        ScpVoiceConfig.Instance.SizeSliderHint)
    {
        ShouldSyncDrag = false;
    }

    public override void HandleMove(float previousValue, float newValue)
    {
        base.HandleMove(previousValue, newValue);

        if (!Player || !ScpVoiceProfile.Profiles.TryGetValue(Player, out var profile))
            return;

        profile.Settings.Size = Mathf.CeilToInt(newValue);
        
        ApiLog.Debug("Scp Voice", $"Player &1{Player.Name}&r (&6{Player.UserId}&r) changed their hint size to &1{profile.Settings.Size}&r");
    }
}