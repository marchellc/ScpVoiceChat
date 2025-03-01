using System;

using LabExtended.API.CustomVoice.Threading;

using VoiceChat;

namespace ScpVoiceChat.Voice.Proximity;

public class ProximityChatProcessor : IVoiceThreadAction
{
    public static volatile ProximityChatProcessor Instance = new();
    
    public void Modify(ref VoiceThreadPacket packet)
    {
        if (packet is not ProximityChatPacket proximityChatPacket)
            throw new Exception("Expected ProximityChatPacket");

        var buffer = new float[VoiceChatSettings.PacketSizePerChannel];
        var encoded = new byte[VoiceChatSettings.MaxEncodedSize];
        
        packet.Decoder.Decode(packet.Data, packet.Length, buffer);

        for (int i = 0; i < buffer.Length; i++)
            buffer[i] *= proximityChatPacket.Volume;

        packet.Length = packet.Encoder.Encode(buffer, encoded, VoiceChatSettings.PacketSizePerChannel);
        packet.Data = encoded;
    }
}