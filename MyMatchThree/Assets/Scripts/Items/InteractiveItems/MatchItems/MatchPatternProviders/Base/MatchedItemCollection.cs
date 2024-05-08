namespace GNMS.MatchThree
{
	using System.Collections;
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
		const float matchAnimationDuration = 0.25f;

		Board board;
		List<MatchItem> matchedItems;
		MatchItem activeMatchItem;
		PowerUpToProduce powerUpToProduce;

		bool isMatching = false;

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

		public void Match()
		{
			if (this.isMatching)
			{
				return;
			}

			this.isMatching = true;
			foreach (MatchItem matchItem in this.matchedItems)
			{
				matchItem.SetStabilized(false);
			}
			this.board.StartCoroutine(this.MatchCoroutine());
		}

		IEnumerator MatchCoroutine()
		{
			float timer = 0f;
			float duration = MatchedItemCollection.matchAnimationDuration;
			while (timer < duration)
			{
				yield return new WaitForEndOfFrame();
				float scale = Mathf.SmoothStep(1, 0, timer / duration);
				foreach (MatchItem matchItem in this.matchedItems)
				{
					matchItem.transform.localScale = scale * Vector3.one;
				}
				timer += Time.deltaTime;
			}
			foreach (MatchItem matchItem in this.matchedItems)
			{
				GameObject.Destroy(matchItem.gameObject);
			}
		}
	}
}