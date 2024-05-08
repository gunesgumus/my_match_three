namespace GNMS.MatchThree
{
	public abstract class PowerUpItem : InteractiveSlidableItem, IBoardPlacementCheckServer
	{
		public abstract bool CheckBoardPlacement(Board board);
		public abstract void Combine(PowerUpItem other);
	}
}