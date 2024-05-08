namespace GNMS.MatchThree
{
	using System.Collections.Generic;
	using UnityEngine;

	public class LightBall : PowerUpItem
	{
		public override bool CheckBoardPlacement(Board board)
		{
			MovingItem otherItem = board.PrimarySwapItem == this ? board.SecondarySwapItem : board.PrimarySwapItem;
			if (otherItem is MatchItem matchItem)
			{
				this.DestroyItemsWithColorOnBoard(board, matchItem.ItemColor);
				return true;
			}
			// TODO: Implement combination of power-ups
			return false;
		}

		public override void Combine(PowerUpItem other)
		{
			if (other is LightBall lightball)
			{
				this.CombineWithLightBall(lightball);
			}
			else if (other is Propeller propeller)
			{
				this.CombineWithPropeller(propeller);
			}
			else if (other is Rocket rocket)
			{
				this.CombineWithRocket(rocket);
			}
			else if (other is Tnt tnt)
			{
				this.CombineWithTnt(tnt);
			}
		}

		void CombineWithLightBall(LightBall lightBall)
		{

		}

		void CombineWithPropeller(Propeller propeller)
		{

		}

		void CombineWithRocket(Rocket rocket)
		{

		}

		void CombineWithTnt(Tnt tnt)
		{

		}

		void DestroyItemsWithColorOnBoard(Board board, MatchItemColor matchItemColor)
		{
			List<MatchItem> matchItemsWithColor = new List<MatchItem>();
			for (int x = 0; x < board.Size.x; x++)
			{
				for (int y = 0; y < board.Size.y; y++)
				{
					Item item = board.GetItemAtPosition(new Vector2Int(x, y));
					if (item is MatchItem matchItem && matchItem.ItemColor == matchItemColor)
					{
						matchItemsWithColor.Add(matchItem);
					}
				}
			}
			foreach (MatchItem matchItem in matchItemsWithColor)
			{
				Destroy(matchItem.gameObject);
			}
		}
	}
}