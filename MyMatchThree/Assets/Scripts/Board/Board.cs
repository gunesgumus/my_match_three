namespace GNMS.MatchThree
{
	using GNMS.StateMachine;
	using UnityEngine;

	public class Board : MonoBehaviour
	{
		[SerializeField]
		Vector2Int boardSize = new Vector2Int(5, 5);
		[SerializeField, Min(0)]
		float tileSize = 1;
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

		public Vector2Int Size => this.boardSize;
		public MovingItem PrimarySwapItem => this.primarySwapItem;
		public MovingItem SecondarySwapItem => this.secondarySwapItem;

		// TODO: Implement this after adding LightBall, etc., since that stalls interaction while being destroyed
		public bool InteractionIsStalled => false;

		private void Awake()
		{
			this.InitializeBoardContent();
			this.InitializeComponents();
			this.SubscribeToEvents();
		}

		private void OnDestroy()
		{
			this.UnsubscribeFromEvents();
		}

		public Item GetItemAtPosition(Vector2Int position)
		{
			if (position.x < 0 || position.x >= this.boardSize.x || position.y < 0 || position.y >= this.boardSize.y)
			{
				throw new System.ArgumentOutOfRangeException(nameof(position), $"Position is outside the bounds of the board.");
			}

			return this.tilesInfo[position.x, position.y].ownerItem;
		}

		public Item GetItemAtPosition(Vector3 position)
		{
			int x = Mathf.FloorToInt(position.x / this.tileSize);
			int y = Mathf.FloorToInt(position.y / this.tileSize);
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

		void InitializeBoardContent()
		{
			this.tilesInfo = new TileInfo[this.boardSize.x, this.boardSize.y];

			for (int x = 0; x < this.boardSize.x; x++)
			{
				for (int y = 0; y < this.boardSize.y; y++)
				{
					MatchItem matchItemPrefab = this.matchItemPrefabs[Random.Range(0, this.matchItemPrefabs.Length)];
					MatchItem instantiatedMatchItem = Instantiate(matchItemPrefab, this.transform);
					Vector3 itemCenterOffset = instantiatedMatchItem.ItemSize / 2;
					Vector3 instantiatedPosition = this.tileSize * new Vector3(x, y, 0) + itemCenterOffset;
					instantiatedMatchItem.transform.position = instantiatedPosition;
					this.tilesInfo[x, y].ownerItem = instantiatedMatchItem;
				}
			}
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
