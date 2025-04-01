using System;

using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;
using LabApi.Loader.Features.Plugins;

using LabExtended.API;
using LabExtended.API.Hints;
using LabExtended.API.Settings;

using LabExtended.Core;

using ScpVoiceChat.Overlay;
using ScpVoiceChat.Settings;
using ScpVoiceChat.Voice;

namespace ScpVoiceChat
{
    public class ScpVoicePlugin : Plugin<ScpVoiceConfig>
    {
        public override string Name { get; } = "Scp Voice";
        public override string Author { get; } = "marchellcx";
        public override string Description { get; } = "Allows SCPs to use spatial proximity voice chat.";

        public override Version Version { get; } = new Version(1, 0 ,0);
        public override Version RequiredApiVersion { get; } = null;
        
        public override void Enable()
        {
            ScpVoiceConfig.Instance = Config;
            
            SettingsManager.AddBuilder(new SettingsBuilder("scpVoiceBuilder").WithMenu(() => new ScpVoiceMenu()));
            
            PlayerEvents.Joined += OnPlayerJoined;
            
            ApiLog.Info("Scp Voice", "Enabled");
        }

        public override void Disable() { }

        private static void OnPlayerJoined(PlayerJoinedEventArgs ev)
        {
            ExPlayer player = ev.Player as ExPlayer;

            player.Voice.AddProfile<ScpVoiceProfile>(player.Role.IsScp);
            player.AddHintElement<ScpVoiceOverlay>();
        }
    }
}