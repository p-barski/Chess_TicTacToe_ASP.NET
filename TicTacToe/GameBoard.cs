namespace TicTacToe
{
	public class GameBoard
	{
		private readonly XO_Enum[,] board;
		public int Size { get; }
		public GameBoard(int size)
		{
			Size = size;
			board = new XO_Enum[size, size];
		}
		public XO_Enum Winner()
		{
			var hor = Horizontal();
			if (hor == XO_Enum.X || hor == XO_Enum.O)
				return hor;
			var vert = Vertical();
			if (vert == XO_Enum.X || vert == XO_Enum.O)
				return vert;
			var diag1 = Diagonal1();
			if (diag1 == XO_Enum.X || diag1 == XO_Enum.O)
				return diag1;
			var diag2 = Diagonal2();
			if (diag2 == XO_Enum.X || diag2 == XO_Enum.O)
				return diag2;
			if (hor == XO_Enum.Draw && vert == XO_Enum.Draw &&
			diag1 == XO_Enum.Draw && diag2 == XO_Enum.Draw)
				return XO_Enum.Draw;
			return XO_Enum.Empty;
		}
		public XO_Enum this[int x, int y]
		{
			get => board[x, y];
			set
			{
				if (board[x, y] != XO_Enum.Empty)
					throw new System.InvalidOperationException(
						$"Position ({x}, {y}) is not empty.");
				board[x, y] = value;
			}
		}
		private XO_Enum Vertical()
		{
			int x_counter;
			int o_counter;
			int draw_counter = 0;
			for (int x = 0; x < Size; x++)
			{
				x_counter = 0;
				o_counter = 0;
				for (int y = 0; y < Size; y++)
				{
					if (board[x, y] == XO_Enum.X)
						x_counter++;
					if (board[x, y] == XO_Enum.O)
						o_counter++;
				}
				if (x_counter == Size)
					return XO_Enum.X;
				if (o_counter == Size)
					return XO_Enum.O;
				if (x_counter > 0 && o_counter > 0)
					draw_counter++;
			}
			if (draw_counter == Size)
				return XO_Enum.Draw;
			return XO_Enum.Empty;
		}
		private XO_Enum Horizontal()
		{
			int x_counter;
			int o_counter;
			int draw_counter = 0;
			for (int y = 0; y < Size; y++)
			{
				x_counter = 0;
				o_counter = 0;
				for (int x = 0; x < Size; x++)
				{
					if (board[x, y] == XO_Enum.X)
						x_counter++;
					if (board[x, y] == XO_Enum.O)
						o_counter++;
				}
				if (x_counter == Size)
					return XO_Enum.X;
				if (o_counter == Size)
					return XO_Enum.O;
				if (x_counter > 0 && o_counter > 0)
					draw_counter++;
			}
			if (draw_counter == Size)
				return XO_Enum.Draw;
			return XO_Enum.Empty;
		}
		private XO_Enum Diagonal1()
		{
			int x_counter = 0;
			int o_counter = 0;
			for (int x = 0, y = 0; x < Size; x++, y++)
			{
				if (board[x, y] == XO_Enum.X)
					x_counter++;
				if (board[x, y] == XO_Enum.O)
					o_counter++;
			}
			if (x_counter == Size)
				return XO_Enum.X;
			if (o_counter == Size)
				return XO_Enum.O;
			if (x_counter > 0 && o_counter > 0)
				return XO_Enum.Draw;
			return XO_Enum.Empty;
		}
		private XO_Enum Diagonal2()
		{
			int x_counter = 0;
			int o_counter = 0;
			for (int x = 0, y = Size - 1; x < Size; x++, y--)
			{
				if (board[x, y] == XO_Enum.X)
					x_counter++;
				if (board[x, y] == XO_Enum.O)
					o_counter++;
			}
			if (x_counter == Size)
				return XO_Enum.X;
			if (o_counter == Size)
				return XO_Enum.O;
			if (x_counter > 0 && o_counter > 0)
				return XO_Enum.Draw;
			return XO_Enum.Empty;
		}
	}
}