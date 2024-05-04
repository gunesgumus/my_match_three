namespace GNMS.MatchThree
{
	using UnityEngine;

	public abstract class Item : MonoBehaviour
	{
		[SerializeField, Min(1)]
		int itemWidth = 1;
		[SerializeField, Min(1)]
		int itemHeight = 1;
	}
}