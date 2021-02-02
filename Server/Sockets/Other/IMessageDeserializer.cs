using Server.Sockets.Messages;

namespace Server.Sockets.Other
{
	public interface IMessageDeserializer
	{
		IReceivedMessage Deserialize(byte[] buffer, int byteCount);
		byte[] SerializeToBuffer<T>(T message);
	}
}