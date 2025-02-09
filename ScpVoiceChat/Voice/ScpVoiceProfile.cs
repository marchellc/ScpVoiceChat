using System.Collections.Generic;
using System.Text;
using AudioAPI;

using LabExtended.API;
using LabExtended.API.CustomVoice.Profiles;

using LabExtended.Extensions;
using NorthwoodLib.Pools;
using PlayerRoles;

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

    public override VoiceProfileResult Receive(ref VoiceMessage message)
    {
        if (IsActive && (SendToProximity || SendToScp))
        {
            if (SendToProximity)
                Handler?.Send(message.Data, message.DataLength);

            if (SendToScp)
            {
                message.Channel = VoiceChatChannel.ScpChat;
                
                foreach (var player in ExPlayer.Players)
                {
                    if (player == Player)
                        continue;
                    
                    if (!player.Role.IsScp)
                        continue;
                    
                    player.Send(message);
                }    
            }
            
            return VoiceProfileResult.SkipAndDontSend;
        }
        
        return VoiceProfileResult.None;
    }
}