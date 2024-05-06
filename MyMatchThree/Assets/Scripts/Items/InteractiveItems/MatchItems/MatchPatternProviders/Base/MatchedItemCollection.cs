namespace GNMS.MatchThree
{
	using System.Collections.Generic;
	using UnityEngine;

	public enum PowerUpToProduce
	{
		None,
		LightBall,
		Tnt,
		Rocket,
		Propeller,
	}

	public class MatchedItemCollection
	{
		Board board;
		List<MatchItem> matchedItems;
		MatchItem activeMatchItem;
		PowerUpToProduce powerUpToProduce;

		public List<MatchItem> MatchedItems => this.matchedItems;
		public MatchItem ActiveMatchItem => this.activeMatchItem;
		public PowerUpToProduce PowerUpToProduce => this.powerUpToProduce;

		public MatchedItemCollection(Board board, List<MatchItem> items, PowerUpToProduce powerUpToProduce)
		{
			this.board = board;
			this.matchedItems = items;
			this.activeMatchItem = (this.board.PrimarySwapItem is MatchItem primaryMatchItem && this.matchedItems.Contains(primaryMatchItem)) ?
				primaryMatchItem :
				((this.board.SecondarySwapItem is MatchItem secondaryMatchItem && this.matchedItems.Contains(secondaryMatchItem)) ?
					secondaryMatchItem :
					this.matchedItems[Random.Range(0, this.matchedItems.Count)]);
			this.powerUpToProduce = powerUpToProduce;
		}

		public void AddMatchItems(List<MatchItem> matchItems)
		{
			foreach (MatchItem matchItem in matchItems)
			{
				this.matchedItems?.Add(matchItem);
				if (this.board.PrimarySwapItem is MatchItem primaryMatchItem && primaryMatchItem == matchItem)
				{
					this.activeMatchItem = primaryMatchItem;
				}
				else if (this.board.SecondarySwapItem is MatchItem secondaryMatchItem && secondaryMatchItem == matchItem)
				{
					this.activeMatchItem = secondaryMatchItem;
				}
			}
		}
	}
}