using LabExtended.API.CustomVoice.Threading;

namespace ScpVoiceChat.Voice.Proximity;

public class ProximityChatPacket : VoiceThreadPacket
{
    public volatile int Volume;
}