using LabExtended.API.Enums;
using LabExtended.API.Hints.Elements.Personal;

using ScpVoiceChat.Voice;

namespace ScpVoiceChat.Overlay;

public class ScpVoiceOverlay : PersonalHintElement
{
    public ScpVoicePersonalSettings Settings { get; private set; }
    public ScpVoiceProfile Profile { get; private set; }

    public override HintAlign Alignment => Settings?.Align ?? base.Alignment;
    public override float VerticalOffset => Settings?.VerticalOffset ?? base.VerticalOffset;

    public override void OnEnabled()
    {
        base.OnEnabled();

        if (!Player.Voice.HasProfile<ScpVoiceProfile>(out var profile))
            return;

        Profile = profile;
        Settings = profile.Settings;
    }

    public override void OnDisabled()
    {
        base.OnDisabled();

        Profile = null;
        Settings = null;
    }

    public override bool OnDraw()
    {
        if (Settings is null || Profile is null || !Profile.IsActive)
            return false;

        if (Settings.Size != 0)
        {
            Builder.Append("<size=");
            Builder.Append(Settings.Size);
            Builder.Append(">");
        }

        Builder.AppendFormat(ScpVoiceConfig.Instance.DefaultSettings.Template, Profile.GetCurrentMode());

        if (Settings.Size != 0)
            Builder.Append("</size>");

        Builder.AppendLine();
        return true;
    }
}