using System;
using UnityEngine;

using LabExtended.API.CustomVoice.Threading;
using VoiceChat;

namespace ScpVoiceChat.Voice.Proximity;

public class ProximityChatProcessor : IVoiceThreadAction
{
    public static volatile ProximityChatProcessor Instance = new();

    public void Modify(ref VoiceThreadPacket packet) {
        if (packet is not ProximityChatPacket proximityChatPacket)
            throw new Exception("Expected ProximityChatPacket");

        float[] buffer = new float[VoiceChatSettings.SampleRate];
        packet.Decoder.Decode(packet.Data, packet.Length, buffer);

        for (int i = 0; i < buffer.Length; i++)
            buffer[i] = Mathf.Clamp(buffer[i] * proximityChatPacket.Volume, -1f, 1f);

        packet.Length = packet.Encoder.Encode(buffer, packet.Data, VoiceChatSettings.PacketSizePerChannel);
    }
}