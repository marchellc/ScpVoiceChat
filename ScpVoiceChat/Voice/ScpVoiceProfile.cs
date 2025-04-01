using System.Collections.Generic;
using System.Text;
using AudioAPI;

using LabExtended.API;
using LabExtended.API.CustomVoice.Profiles;
using LabExtended.API.CustomVoice.Threading;
using LabExtended.Extensions;
using NorthwoodLib.Pools;
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
        
        Handler.AddSpeaker("ScpVoice", x =>
        {
            x.NetworkVolume = ScpVoiceConfig.Instance.SpeakerVolume;
            
            x.NetworkMinDistance = ScpVoiceConfig.Instance.MinSpeakerDistance;
            x.NetworkMaxDistance = ScpVoiceConfig.Instance.MaxSpeakerDistance;
            
            x.NetworkIsSpatial = ScpVoiceConfig.Instance.UseSpatialAudio;
            x.NetworkIsStatic = false;
        });
        
        Handler.ParentTransform = Player.Transform;
        
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

    public override bool OnChangingRole(RoleTypeId newRoleType)
    {
        if (newRoleType.IsScp() && !ScpVoiceConfig.Instance.BlacklistedRoles.Contains(newRoleType))
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
                
                if ((player.Role.IsSpectator || player.Role.IsOverwatch))
                {
                    var spectatedPlayer = player.SpectatedPlayer;

                    if (spectatedPlayer != null)
                    {
                        if (spectatedPlayer == Player)
                            return true;
                        
                        if (spectatedPlayer.Position.DistanceTo(Player) <= ScpVoiceConfig.Instance.MaxSpeakerDistance)
                            return true;
                    }

                    return false;
                }

                return true;
            });
        }

        ProximityChatPacket PacketFactory()
        {
            var packet = new ProximityChatPacket();

            packet.Volume = ScpVoiceConfig.Instance.VolumeMultiplier;
            return packet;
        }
        
        if (SendToProximity && ScpVoiceConfig.Instance.VolumeMultiplier != 1)
        {
            if (SendToScp)
            {
                for (int i = 0; i < ExPlayer.Players.Count; i++)
                {
                    var player = ExPlayer.Players[i];

                    if (!player || !player.Role.IsScp)
                        continue;

                    if (player == Player && !player.Toggles.CanHearSelf)
                        continue;

                    player.Send(message);
                }
            }
            
            Player.Voice.Thread.ProcessCustom(message.Data, message.DataLength, ProximityChatProcessor.Instance, PacketHandler, PacketFactory);
            return VoiceProfileResult.SkipAndDontSend;
        }

        return VoiceProfileResult.None;
    }

    public override VoiceProfileResult SendTo(ref VoiceMessage message, ExPlayer player)
    {
        if (player == Player && !player.Toggles.CanHearSelf)
            return VoiceProfileResult.SkipAndDontSend;

        var isHandled = false;
        
        if (SendToProximity)
        {
            if (ScpVoiceConfig.Instance.VolumeMultiplier == 1)
            {
                Handler.Send(player, message.Data, message.DataLength);

                isHandled = true;
            }
        }

        if (SendToScp)
        {
            message.Channel = VoiceChatChannel.ScpChat;
            
            player.Send(message);

            isHandled = true;
        }

        return isHandled ? VoiceProfileResult.SkipAndDontSend : VoiceProfileResult.None;
    }
}