using System.Threading.Tasks;
using Server.Games;

namespace Server.Database
{
	public interface IChessSessionCanceler
	{
		Task TryCancel(IGameSession session, IPlayer cancelingPlayer);
	}
}