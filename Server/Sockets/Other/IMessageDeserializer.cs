using Server.Sockets.Messages;

namespace Server.Sockets.Other
{
	public interface IMessageDeserializer
	{
		IMessage Deserialize(byte[] buffer, int byteCount);
		byte[] SerializeToBuffer<T>(T message);
	}
}