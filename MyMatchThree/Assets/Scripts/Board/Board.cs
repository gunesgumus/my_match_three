using GNMS.StateMachine;
using UnityEngine;

namespace GNMS.MatchThree
{
	public class Board : MonoBehaviour
	{
		[SerializeField]
		Vector2Int boardSize = new Vector2Int(5, 5);
		[SerializeField, Min(0)]
		float tileSize = 1;
		[SerializeField]
		MatchItem[] matchItemPrefabs;
		[SerializeField]
		PowerUpItem[] powerUpItemPrefabs;

		public SignalContainer OnItemSlideInput = new SignalContainer();
		public SignalContainer OnInvalidSlideComplete = new SignalContainer();
		public SignalContainer OnValidSlideComplete = new SignalContainer();
		public SignalContainer OnDestructionComplete = new SignalContainer();

		BoardStateMachine boardStateMachine;

		TileInfo[,] tilesInfo;

		private void Awake()
		{
			this.InitializeBoardContent();

			this.boardStateMachine = this.gameObject.AddComponent<BoardStateMachine>();
		}

		public Item GetItemAtPosition(Vector3 position)
		{
			int x = Mathf.FloorToInt(position.x / this.tileSize);
			int y = Mathf.FloorToInt(position.y / this.tileSize);
			return (x >= 0 && x < this.boardSize.x && y >= 0 && y < this.boardSize.y) ?
				this.tilesInfo[x, y].ownerItem :
				null;
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

		struct TileInfo
		{
			public Item ownerItem;
		}
	}
}
