using LabExtended.API.Settings.Entries;
using LabExtended.Core;
using ScpVoiceChat.Voice;

namespace ScpVoiceChat.Settings;

public class ScpVoiceOffsetSlider : SettingsSlider
{
    public ScpVoiceOffsetSlider() : base("scpVoiceMenu.offsetSlider",
        ScpVoiceConfig.Instance.OffsetSliderLabel,
        -15f,
        15f,
        
        0f,

        true,

        "0.##",
        "{0}",

        ScpVoiceConfig.Instance.OffsetSliderHint)
    {
        ShouldSyncDrag = false;
    }

    public override void HandleMove(float previousValue, float newValue)
    {
        base.HandleMove(previousValue, newValue);

        if (!Player || !ScpVoiceProfile.Profiles.TryGetValue(Player, out var profile))
            return;

        profile.Settings.VerticalOffset = newValue;
        
        ApiLog.Debug("Scp Voice", $"Player &1{Player.Nickname}&r (&6{Player.UserId}&r) changed their vertical offset to &1{profile.Settings.VerticalOffset}&r");
    }
}