namespace GNMS.MatchThree
{
	using UnityEngine;

	public abstract class Item : MonoBehaviour
	{
		[SerializeField]
		Vector2Int itemSize = new Vector2Int(1, 1);

		public Vector2 ItemSize => this.itemSize;
	}
}