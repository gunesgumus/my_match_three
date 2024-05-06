namespace GNMS.MatchThree
{
	using UnityEngine;

	public enum MatchItemColor
	{
		Red,
		Green,
		Blue,
		Yellow,
		Purple,
	}

	public class MatchItem : InteractiveSlidableItem, IBoardPlacementCheckServer
	{
		[SerializeField]
		MatchItemColor itemColor = MatchItemColor.Red;

		public MatchItemColor ItemColor => this.itemColor;

		public bool CheckBoardPlacement(Board board)
		{
			return board.CheckAndRegisterForMatch(this);
		}
	}
}