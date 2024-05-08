namespace GNMS.MatchThree
{
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;

	public class BoardMatchCheckerOperative : MonoBehaviour
	{
		Board board;

		List<(MatchPatternProvider matchPatternProvider, System.Type powerUpType)>
			matchPatternProvidersAndPowerUpsToProduceSortedByPrecedence = new()
			{
				(new QuintupleLinearMatchPatternProvider(), typeof(LightBall)),
				(new TShapedMatchPatternProvider(), typeof(Tnt)),
				(new LShapedMatchPatternProvider(), typeof(Tnt)),
				(new QuadrupleLinearMatchPatternProvider(), typeof(Rocket)),
				(new TwoUnitSquareMatchPatternProvider(), typeof(Propeller)),
				(new TripleLinearMatchPatternProvider(), null),
			};
		Dictionary<MatchItem, MatchedItemCollection> matchItemToMatchedItemCollectionMapping = new();
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

		public bool ItemIsRegisteredForMatch(MatchItem matchItem)
		{
			return this.matchItemToMatchedItemCollectionMapping.ContainsKey(matchItem);
		}

		void PopRegisteredMatchItems()
		{
			foreach (MatchedItemCollection matchedItemCollection in this.matchItemToMatchedItemCollectionMapping.Values)
			{
				matchedItemCollection.Match();
			}
			this.ClearRegisteredMatchItems();
			this.alreadyCheckedDuringCurrentUpdate = false;
		}

		void DetectMatches()
		{
			foreach (var matchPatterProviderAndTargetItem in this.matchPatternProvidersAndPowerUpsToProduceSortedByPrecedence)
			{
				MatchPatternProvider matchPatternProvider = matchPatterProviderAndTargetItem.matchPatternProvider;
				System.Type powerUpType = matchPatterProviderAndTargetItem.powerUpType;
				MatchPattern[] acceptableMatchPatterns = matchPatternProvider.AcceptableMatchPatterns;
				foreach (MatchPattern acceptableMatchPattern in acceptableMatchPatterns)
				{
					this.DetectMatchPattern(acceptableMatchPattern, powerUpType);
				}
			}
		}

		void DetectMatchPattern(MatchPattern matchPattern, System.Type powerUpType)
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
						MatchedItemCollection matchedItemCollection = new MatchedItemCollection(board, matchedItems, powerUpType);
						foreach (MatchItem matchedItem in matchedItems)
						{
							this.matchItemToMatchedItemCollectionMapping.Add(matchedItem, matchedItemCollection);
						}
						continue;
					}
					// some are registered, some are not
					MatchItem firstRegisteredItem = matchedItems.First(matchedItem => this.ItemIsRegisteredForMatch(matchedItem));
					MatchedItemCollection registeredItemCollection = this.matchItemToMatchedItemCollectionMapping[firstRegisteredItem];
					List<MatchItem> unregisteredMatchItems = matchedItems
						.Where(matchedItem => !this.ItemIsRegisteredForMatch(matchedItem))
						.ToList();
					registeredItemCollection.AddMatchItems(unregisteredMatchItems);
					foreach (MatchItem unregisteredMatchItem in unregisteredMatchItems)
					{
						this.matchItemToMatchedItemCollectionMapping.Add(unregisteredMatchItem, registeredItemCollection);
					}
				}
			}
		}

		void ClearRegisteredMatchItems()
		{
			this.matchItemToMatchedItemCollectionMapping.Clear();
		}
	}
}