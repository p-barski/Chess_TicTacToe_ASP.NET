using System.Text;
using Newtonsoft.Json;
using Server.Sockets.Messages;

namespace Server.Sockets.Other
{
	public class MessageDeserializer : IMessageDeserializer
	{
		private readonly JsonSerializerSettings jsonSettings =
			new JsonSerializerSettings()
			{ MissingMemberHandling = MissingMemberHandling.Error };

		public IReceivedMessage Deserialize(byte[] buffer, int byteCount)
		{
			var json = Encoding.UTF8.GetString(buffer, 0, byteCount);
			try
			{
				return JsonConvert.DeserializeObject<FindGameMessage>(json, jsonSettings);
			}
			catch (JsonException) { }
			try
			{
				return JsonConvert.DeserializeObject<MakeMoveMessage>(json, jsonSettings);
			}
			catch (JsonException) { }
			try
			{
				return JsonConvert.DeserializeObject<FindChessGameMessage>(json, jsonSettings);
			}
			catch (JsonException) { }
			try
			{
				return JsonConvert.DeserializeObject<MakeChessMoveMessage>(json, jsonSettings);
			}
			catch (JsonException) { }
			try
			{
				return JsonConvert.DeserializeObject<PawnPromotionMessage>(json, jsonSettings);
			}
			catch (JsonException) { }
			try
			{
				return JsonConvert.DeserializeObject<CancelSessionMessage>(json, jsonSettings);
			}
			catch (JsonException) { }
			return new IncorrectMessage() { Json = json };
		}
		public byte[] SerializeToBuffer<T>(T message)
		{
			var json = JsonConvert.SerializeObject(message);
			return Encoding.UTF8.GetBytes(json);
		}
	}
}