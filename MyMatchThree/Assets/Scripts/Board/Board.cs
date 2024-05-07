namespace GNMS.MatchThree
{
	using GNMS.StateMachine;
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Unity.VisualScripting;
	using UnityEngine;

	public class Board : MonoBehaviour
	{
		[SerializeField]
		Vector2Int boardSize = new Vector2Int(5, 5);
		[SerializeField, Min(0f)]
		float minimumSlideDistance = 0.5f;
		[Header("Prefabs")]
		[SerializeField]
		MatchItem[] matchItemPrefabs;
		[SerializeField]
		PowerUpItem[] powerUpItemPrefabs;

		public SignalContainer OnItemSlideInput = new SignalContainer();
		public SignalContainer OnInvalidSlideComplete = new SignalContainer();
		public SignalContainer OnValidSlideComplete = new SignalContainer();

		BoardStateMachine boardStateMachine;

		BoardInteractionOperative boardInteractionOperative;
		BoardMatchCheckerOperative boardMatchCheckerOperative;

		MovingItem primarySwapItem;
		MovingItem secondarySwapItem;

		TileInfo[,] tilesInfo;
		Dictionary<MatchItemColor, MatchItem> colorToMatchItemMapping = new Dictionary<MatchItemColor, MatchItem>();

		public Vector2Int Size => this.boardSize;
		public MovingItem PrimarySwapItem => this.primarySwapItem;
		public MovingItem SecondarySwapItem => this.secondarySwapItem;

		// TODO: Implement this after adding LightBall, etc., since that stalls interaction while being destroyed
		public bool InteractionIsStalled => false;

		private void Awake()
		{
			this.InitializeData();
			this.InitializeBoardContent();
			this.InitializeComponents();
			this.SubscribeToEvents();
		}

		private void OnDestroy()
		{
			this.UnsubscribeFromEvents();
		}

		private void Update()
		{
			this.MoveItemsDownAndCreateNewItemsAtTop();
		}

		public Item GetItemAtPosition(Vector2Int position)
		{
			if (position.x < 0 || position.x >= this.boardSize.x || position.y < 0 || position.y >= this.boardSize.y)
			{
				throw new System.ArgumentOutOfRangeException(nameof(position), $"Position is outside the boundaries of the board.");
			}

			return this.tilesInfo[position.x, position.y].ownerItem;
		}

		public Item GetItemAtPosition(Vector3 position)
		{
			int x = Mathf.FloorToInt(position.x);
			int y = Mathf.FloorToInt(position.y);
			return (x >= 0 && x < this.boardSize.x && y >= 0 && y < this.boardSize.y) ?
				this.tilesInfo[x, y].ownerItem :
				null;
		}

		public void SwapItems(MovingItem firstItem, MovingItem secondItem)
		{
			Vector3 firstItemInitialPosition = primarySwapItem.transform.position;
			Vector3 secondItemInitialPosition = secondarySwapItem.transform.position;
			Vector2Int firstTilePosition = this.WorldPositionToTile(firstItemInitialPosition);
			Vector2Int secondTilePosition = this.WorldPositionToTile(secondItemInitialPosition);
			this.tilesInfo[firstTilePosition.x, firstTilePosition.y].ownerItem = secondItem;
			this.tilesInfo[secondTilePosition.x, secondTilePosition.y].ownerItem = firstItem;

			firstItem.MoveToPosition(secondItemInitialPosition);
			secondItem.MoveToPosition(firstItemInitialPosition);
		}

		public bool CheckItemBoardPlacement(MovingItem movingItem)
		{
			if (movingItem is IBoardPlacementCheckServer boardPlacementCheckServer)
			{
				return boardPlacementCheckServer.CheckBoardPlacement(this);
			}
			return false;
		}

		public bool CheckAndRegisterForMatch(MatchItem matchItem)
		{
			return this.boardMatchCheckerOperative.CheckAndRegisterForMatch(matchItem);
		}

		public void HandleValidSlide()
		{
			this.OnValidSlideComplete.EmitSignal();
		}

		public void HandleInvalidSlide()
		{
			this.OnInvalidSlideComplete.EmitSignal();
		}

		public void RemoveItem(Item item)
		{
			List<Vector2Int> tilesOccupiedByItem = this.GetTilePositionsOccupiedByItem(item);
			foreach (Vector2Int tileOccupiedByItem in tilesOccupiedByItem)
			{
				if (tileOccupiedByItem.y >= this.boardSize.y)
				{
					continue;
				}
				if (this.tilesInfo[tileOccupiedByItem.x, tileOccupiedByItem.y].ownerItem == item)
				{
					this.tilesInfo[tileOccupiedByItem.x, tileOccupiedByItem.y].ownerItem = null;
				}
			}
		}

		List<Vector2Int> GetTilePositionsOccupiedByItem(Item item)
		{
			Vector2 itemPosition = item.transform.position;
			Vector2 itemBottomLeft = itemPosition - item.ItemSize / 2;
			Vector2Int itemTileBottomLeft = new Vector2Int(Mathf.RoundToInt(itemBottomLeft.x), Mathf.RoundToInt(itemBottomLeft.y));
			List<Vector2Int> tilePositionsOccupied = new List<Vector2Int>();
			Vector2Int itemSizeInt = item.ItemSizeInt;
			for (int x = 0; x < itemSizeInt.x; x++)
			{
				for (int y = 0; y < itemSizeInt.y; y++)
				{
					tilePositionsOccupied.Add(itemTileBottomLeft + new Vector2Int(x, y));
				}
			}
			return tilePositionsOccupied;
		}

		void MoveItemsDownAndCreateNewItemsAtTop()
		{
			Dictionary<int, int> emptyTilesHeight = new Dictionary<int, int>();
			for (int x = 0; x < this.boardSize.x; x++)
			{
				int y = 0;
				while (y < this.boardSize.y)
				{
					if (this.tilesInfo[x, y].ownerItem == null)
					{
						break;
					}
					y++;
				}
				emptyTilesHeight.Add(x, y);
			}
			for (int x = 0; x < this.boardSize.x; x++)
			{
				int heightOfEmptyTile = emptyTilesHeight[x];
				int upperItemHeight = heightOfEmptyTile + 1;
				while (heightOfEmptyTile < this.boardSize.y && upperItemHeight < this.boardSize.y)
				{
					Item upperItem = this.GetItemAtPosition(new Vector2Int(x, upperItemHeight));
					if (upperItem == null)
					{
						upperItemHeight++;
						continue;
					}

					if (upperItem is MovingItem movingItem)
					{
						this.tilesInfo[x, heightOfEmptyTile].ownerItem = movingItem;
						this.tilesInfo[x, upperItemHeight].ownerItem = null;
						// TODO: Move upperItem to heightOfEmptyTile
						movingItem.DropToPosition(new Vector3(x, heightOfEmptyTile, 0f) + new Vector3(0.5f, 0.5f, 0f));
						heightOfEmptyTile++;
						upperItemHeight++;
						continue;
					}

					heightOfEmptyTile = upperItemHeight + 1;
					while (heightOfEmptyTile < this.boardSize.y)
					{
						if (this.tilesInfo[x, heightOfEmptyTile].ownerItem == null)
						{
							break;
						}
						heightOfEmptyTile++;
					}
					upperItemHeight = heightOfEmptyTile + 1;
				}
				int itemCountToCreateOnTop = Mathf.Max(0, this.boardSize.y - heightOfEmptyTile);
				for (int count = 0; count < itemCountToCreateOnTop; count++)
				{
					MatchItem instantiatedMatchItem = Instantiate(
						this.matchItemPrefabs[UnityEngine.Random.Range(0, this.matchItemPrefabs.Length)],
						this.transform);
					float itemCreationHeight = Mathf.Max(
						this.tilesInfo[x, heightOfEmptyTile - 1].ownerItem.transform.position.y + 1f,
						this.boardSize.y + 0.5f);
					Vector3 itemCreationPosition = new Vector3(x + 0.5f, itemCreationHeight, 0f);
					instantiatedMatchItem.transform.position = itemCreationPosition;
					this.tilesInfo[x, heightOfEmptyTile].ownerItem = instantiatedMatchItem;
					instantiatedMatchItem.DropToPosition(new Vector3(x, heightOfEmptyTile, 0f) + new Vector3(0.5f, 0.5f, 0f));
					heightOfEmptyTile++;
					upperItemHeight++;
				}
			}
		}

		void InitializeData()
		{
			this.tilesInfo = new TileInfo[this.boardSize.x, this.boardSize.y];

			foreach (MatchItem matchItemPrefab in this.matchItemPrefabs)
			{
				this.colorToMatchItemMapping.Add(matchItemPrefab.ItemColor, matchItemPrefab);
			}
		}

		void InitializeBoardContent()
		{
			// These must be enough to avoid all other currently existing match patterns as well
			MatchPattern[] matchPatternsToAvoid = new[]
			{
				// two unit square
				new MatchPattern(new[] { Vector2Int.zero, Vector2Int.right, Vector2Int.up, Vector2Int.right + Vector2Int.up }),
				// horizontal triple linear
				new MatchPattern(new[] { Vector2Int.zero, Vector2Int.right, 2 * Vector2Int.right }),
				// vertical triple linear
				new MatchPattern(new[] { Vector2Int.zero, Vector2Int.up, 2 * Vector2Int.up }),
			};

			for (int x = 0; x < this.boardSize.x; x++)
			{
				for (int y = 0; y < this.boardSize.y; y++)
				{
					Vector2Int position = new Vector2Int(x, y);
					List<MatchItemColor> possibleColors = Enum.GetValues(typeof(MatchItemColor)).Cast<MatchItemColor>().ToList();

					foreach (MatchPattern matchPatternToAvoid in matchPatternsToAvoid)
					{
						Vector2Int leftBottom = position - (matchPatternToAvoid.ExtentMax - matchPatternToAvoid.ExtentMin);
						if (leftBottom.x < 0 || leftBottom.y < 0)
						{
							continue;
						}
						Vector2Int matchLocation = position - matchPatternToAvoid.ExtentMax;
						MatchItemColor? colorToAvoid = matchPatternToAvoid.GetMatchColorToAvoidForTargetPosition(this, matchLocation, position);
						if (colorToAvoid.HasValue)
						{
							possibleColors.Remove(colorToAvoid.Value);
						}
					}

					MatchItem matchItemPrefab = this.colorToMatchItemMapping[possibleColors[UnityEngine.Random.Range(0, possibleColors.Count)]];
					MatchItem instantiatedMatchItem = Instantiate(matchItemPrefab, this.transform);
					instantiatedMatchItem.transform.position = this.GetItemWorldPosition(instantiatedMatchItem, new Vector2Int(x, y));
					this.tilesInfo[x, y].ownerItem = instantiatedMatchItem;
				}
			}
		}

		Vector3 GetItemWorldPosition(Item item, Vector2Int tilePosition)
		{
			Vector3 itemCenterOffset = item.ItemSize / 2;
			return new Vector3(tilePosition.x, tilePosition.y, 0) + itemCenterOffset;
		}

		void InitializeComponents()
		{
			this.boardInteractionOperative = this.gameObject.AddComponent<BoardInteractionOperative>();
			this.boardInteractionOperative.MinimumSlideDistance = this.minimumSlideDistance;
			this.boardInteractionOperative.enabled = false;

			this.boardMatchCheckerOperative = this.gameObject.AddComponent<BoardMatchCheckerOperative>();

			this.boardStateMachine = this.gameObject.AddComponent<BoardStateMachine>();
		}

		void SubscribeToEvents()
		{
			this.boardInteractionOperative.OnItemSlideInput += this.BoardInteraction_OnItemSlideInput;
		}

		void UnsubscribeFromEvents()
		{
			this.boardInteractionOperative.OnItemSlideInput -= this.BoardInteraction_OnItemSlideInput;
		}

		void BoardInteraction_OnItemSlideInput(InteractiveSlidableItem slidableItem, ItemSlideDirection slideDirection)
		{
			if (!slidableItem.IsStabilized)
			{
				return;
			}

			Vector2Int tile = this.WorldPositionToTile(slidableItem.transform.position);
			Vector2Int otherTile = tile + slideDirection.ConvertToVector2Int();
			if (this.PointIsOutsideBoard(otherTile))
			{
				return;
			}

			Item otherTileItem = this.tilesInfo[otherTile.x, otherTile.y].ownerItem;
			if (otherTileItem is MovingItem otherMovingItem && otherMovingItem.IsStabilized)
			{
				this.primarySwapItem = slidableItem;
				this.secondarySwapItem = otherMovingItem;
				this.OnItemSlideInput.EmitSignal();
			}
		}

		Vector2Int WorldPositionToTile(Vector3 position) =>
			new Vector2Int(Mathf.FloorToInt(position.x), Mathf.FloorToInt(position.y));

		bool PointIsOutsideBoard(Vector2Int point) =>
			point.x < 0 || point.x >= this.boardSize.x || point.y < 0 || point.y >= this.boardSize.y;

		struct TileInfo
		{
			public Item ownerItem;
		}
	}
}
