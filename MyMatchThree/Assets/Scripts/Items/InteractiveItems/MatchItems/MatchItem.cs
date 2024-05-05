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

	public class MatchItem : InteractiveSlidableItem
	{
		[SerializeField]
		MatchItemColor itemColor = MatchItemColor.Red;
	}
}