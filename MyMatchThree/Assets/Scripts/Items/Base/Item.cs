namespace GNMS.MatchThree
{
	using UnityEngine;

	public abstract class Item : MonoBehaviour
	{
		[SerializeField]
		Vector2Int itemSize = new Vector2Int(1, 1);

		Board board;

		public Vector2 ItemSize => this.itemSize;
		public Vector2Int ItemSizeInt => this.itemSize;

		private void Awake()
		{
			this.board = this.GetComponentInParent<Board>();
		}

		private void OnDestroy()
		{
			this.board.RemoveItem(this);
		}
	}
}