namespace GNMS.MatchThree
{
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;

	public class BoardMatchCheckerOperative : MonoBehaviour
	{
		Board board;

		List<(MatchPatternProvider matchPatternProvider, PowerUpToProduce powerUpToProduce)>
			matchPatternProvidersAndPowerUpsToProduceSortedByPrecedence = new()
			{
				(new QuintupleLinearMatchPatternProvider(), PowerUpToProduce.LightBall),
				(new TShapedMatchPatternProvider(), PowerUpToProduce.Tnt),
				(new LShapedMatchPatternProvider(), PowerUpToProduce.Tnt),
				(new QuadrupleLinearMatchPatternProvider(), PowerUpToProduce.Rocket),
				(new TwoUnitSquareMatchPatternProvider(), PowerUpToProduce.Propeller),
				(new TripleLinearMatchPatternProvider(), PowerUpToProduce.None),
			};
		Dictionary<MatchItem, MatchedItemCollection> matchItemToMatchItemCollectionMapping = new();
		bool alreadyCheckedDuringCurrentUpdate = false;

		private void Awake()
		{
			this.board = this.GetComponent<Board>();
		}

		private void LateUpdate()
		{
			this.PopRegisteredMatchItems();
		}

		public bool CheckAndRegisterForMatch(MatchItem matchItem)
		{
			if (this.alreadyCheckedDuringCurrentUpdate)
			{
				return this.ItemIsRegisteredForMatch(matchItem);
			}

			this.alreadyCheckedDuringCurrentUpdate = true;
			this.DetectMatches();

			return this.ItemIsRegisteredForMatch(matchItem);
		}

		void PopRegisteredMatchItems()
		{
			foreach (MatchItem matchItem in this.matchItemToMatchItemCollectionMapping.Keys)
			{
				Destroy(matchItem.gameObject);
			}
			this.ClearRegisteredMatchItems();
			this.alreadyCheckedDuringCurrentUpdate = false;
		}

		void DetectMatches()
		{
			foreach (var matchPatterProviderAndTargetItem in this.matchPatternProvidersAndPowerUpsToProduceSortedByPrecedence)
			{
				MatchPatternProvider matchPatternProvider = matchPatterProviderAndTargetItem.matchPatternProvider;
				PowerUpToProduce powerUpToProduce = matchPatterProviderAndTargetItem.powerUpToProduce;
				MatchPattern[] acceptableMatchPatterns = matchPatternProvider.AcceptableMatchPatterns;
				foreach (MatchPattern acceptableMatchPattern in acceptableMatchPatterns)
				{
					this.DetectMatchPattern(acceptableMatchPattern, powerUpToProduce);
				}
			}
		}

		void DetectMatchPattern(MatchPattern matchPattern, PowerUpToProduce powerUpToProduce)
		{
			Vector2Int boardStartPosition = -matchPattern.ExtentMin;
			Vector2Int boardEndPosition = this.board.Size - Vector2Int.one - matchPattern.ExtentMax;
			for (int x = boardStartPosition.x; x <= boardEndPosition.x; x++)
			{
				for (int y = boardStartPosition.y; y <= boardEndPosition.y; y++)
				{
					List<MatchItem> matchedItems = matchPattern.GetMatchedItems(this.board, new Vector2Int(x, y));
					if (matchedItems.Count <= 0 ||
						matchedItems.All(matchedItem => this.ItemIsRegisteredForMatch(matchedItem)))
					{
						continue;
					}
					if (matchedItems.All(matchedItem => !this.ItemIsRegisteredForMatch(matchedItem)))
					{
						MatchedItemCollection matchedItemCollection = new MatchedItemCollection(board, matchedItems, powerUpToProduce);
						foreach (MatchItem matchedItem in matchedItems)
						{
							this.matchItemToMatchItemCollectionMapping.Add(matchedItem, matchedItemCollection);
							if (this.matchItemToMatchItemCollectionMapping.Keys.Count() > 0)
							{
								Debug.Log($"mapping key count {this.matchItemToMatchItemCollectionMapping.Keys.Count()}");
							}
						}
						continue;
					}
					// some are registered, some are not
					MatchItem firstRegisteredItem = matchedItems.First(matchedItem => this.ItemIsRegisteredForMatch(matchedItem));
					MatchedItemCollection registeredItemCollection = this.matchItemToMatchItemCollectionMapping[firstRegisteredItem];
					registeredItemCollection.AddMatchItems(matchedItems
						.Where(matchedItem => !this.ItemIsRegisteredForMatch(matchedItem))
						.ToList());
				}
			}
		}

		bool ItemIsRegisteredForMatch(MatchItem matchItem)
		{
			return this.matchItemToMatchItemCollectionMapping.ContainsKey(matchItem);
		}

		void ClearRegisteredMatchItems()
		{
			this.matchItemToMatchItemCollectionMapping.Clear();
		}
	}
}