using System;
using Chess;
using Server.Games.Chess;
using Server.Games.TicTacToe;

namespace Server.Games
{
	public class GameSessionFactory : IGameSessionFactory
	{
		private readonly IChessGameFactory chessGameFactory;
		public GameSessionFactory(IChessGameFactory chessGameFactory)
		{
			this.chessGameFactory = chessGameFactory;
		}
		public IGameSession Create(IPlayer playerOne, IPlayer playerTwo,
			IExpectedGame expectedGame)
		{
			if (expectedGame is ExpectedTicTacToe)
			{
				int size = ((ExpectedTicTacToe)expectedGame).Size;
				return new TicTacToeGameSession(playerOne, playerTwo, size);
			}
			if (expectedGame is ExpectedChess)
			{
				return new ChessGameSession(playerOne, playerTwo, chessGameFactory);
			}
			throw new NotImplementedException();
		}
	}
}