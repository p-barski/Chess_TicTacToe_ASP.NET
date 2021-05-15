namespace Server.Games.Chess
{
	public class PromotionMove : IGameMove
	{
		public string PromotionPiece { get; }
		public PromotionMove(string promotionPiece)
		{
			PromotionPiece = promotionPiece;
		}
		public static bool operator ==(PromotionMove left, PromotionMove right)
		{
			return left.Equals(right);
		}
		public static bool operator !=(PromotionMove left, PromotionMove right)
		{
			return !left.Equals(right);
		}
		public override bool Equals(object obj)
		{
			if (!(obj is PromotionMove))
			{
				return false;
			}
			return Equals((PromotionMove)obj);
		}
		public bool Equals(PromotionMove other)
		{
			return PromotionPiece == other.PromotionPiece;
		}
		public override int GetHashCode()
		{
			return PromotionPiece.GetHashCode();
		}
		public override string ToString()
		{
			return $"PromotionMove({PromotionPiece})";
		}
	}
}