using System.Collections.Generic;
using AudioAPI;
using LabApi.Features.Wrappers;
using LabExtended.API;
using LabExtended.API.CustomVoice.Profiles;
using LabExtended.Extensions;
using PlayerRoles;
using ScpVoiceChat.Voice.Proximity;
using VoiceChat;
using VoiceChat.Networking;

namespace ScpVoiceChat.Voice;

public class ScpVoiceProfile : VoiceProfile
{
    public static Dictionary<ExPlayer, ScpVoiceProfile> Profiles { get; } = new Dictionary<ExPlayer, ScpVoiceProfile>();

    public bool SendToProximity { get; private set; } = false;
    public bool SendToScp { get; private set; } = true;
    
    public AudioHandler Handler { get; private set; }
    
    public ScpVoicePersonalSettings Settings { get; } = new ScpVoicePersonalSettings();

    public void ResetState()
    {
        SendToScp = true;
        SendToProximity = false;
    }

    public void ToggleState()
    {
        if (SendToProximity && SendToScp)
        {
            SendToProximity = false;
            SendToScp = true;
        }
        else if (SendToProximity && !SendToScp)
        {
            SendToProximity = true;
            SendToScp = true;
        }
        else
        {
            SendToProximity = true;
            SendToScp = false;
        }
    }

    public string GetCurrentMode()
    {
        if (SendToScp && SendToProximity)
            return "SCP & Proximity";

        if (!SendToScp && SendToProximity)
            return "Proximity";

        return "SCP";
    }

    public override void Start()
    {
        base.Start();
        
        Handler = AudioHandler.GetOrAdd($"{Player.UserId}_ScpVoice");
        Handler.ParentTransform = Player.Transform;

        Handler.AddSpeaker("ScpVoice", x =>
        {
            x.NetworkVolume = ScpVoiceConfig.Instance.SpeakerVolume;
            
            x.NetworkMinDistance = ScpVoiceConfig.Instance.MinSpeakerDistance;
            x.NetworkMaxDistance = ScpVoiceConfig.Instance.MaxSpeakerDistance;
            
            x.NetworkIsSpatial = ScpVoiceConfig.Instance.UseSpatialAudio;
            x.NetworkIsStatic = false;

            x.transform.parent = Player.Transform;
        });
        
        
        Profiles.Add(Player, this);
    }

    public override void Stop()
    {
        base.Stop();
        
        Profiles.Remove(Player);
        
        Handler!.ParentTransform = null;
        Handler?.Dispose();
        Handler = null;
    }

    public override bool EnabledOnRoleChange(RoleTypeId newRoleType)
    {
        if (newRoleType.IsScp(countZombies: true) && !ScpVoiceConfig.Instance.BlacklistedRoles.Contains(newRoleType))
        {
            ResetState();
            return true;
        }
        return false;
    }

    public override VoiceProfileResult ReceiveFrom(ref VoiceMessage message)
    {
        void PacketHandler(ProximityChatPacket packet)
        {
            Handler.Send(packet.Data, packet.Length, player =>
            {
                if (!player) 
                    return false;

                if (player == Player && !player.Toggles.CanHearSelf) 
                    return false;

                return !player.IsSCP || !SendToScp;
            });
        }

        ProximityChatPacket PacketFactory()
        {
            var packet = new ProximityChatPacket();

            packet.Volume = ScpVoiceConfig.Instance.VolumeMultiplier;
            return packet;
        }
        
        if (message.Channel == VoiceChatChannel.Mimicry || Round.IsRoundEnded) {
            return VoiceProfileResult.None;
        }

        if (SendToProximity)
        {
            Player.Voice.Thread.ProcessCustom(message.Data, message.DataLength, ProximityChatProcessor.Instance, PacketHandler, PacketFactory);
        }

        return SendToScp ? VoiceProfileResult.None : VoiceProfileResult.SkipAndDontSend;
    }

    public override VoiceProfileResult SendTo(ref VoiceMessage message, ExPlayer player) {
        return VoiceProfileResult.None;
    }
}