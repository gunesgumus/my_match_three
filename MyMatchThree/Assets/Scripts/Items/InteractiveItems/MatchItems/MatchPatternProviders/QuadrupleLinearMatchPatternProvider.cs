namespace GNMS.MatchThree
{
	using UnityEngine;

	public class QuadrupleLinearMatchPatternProvider : MatchPatternProvider
	{
		readonly MatchPattern[] acceptableMatchPatterns = new MatchPattern[]
		{
			new MatchPattern(new[] { Vector2Int.zero, Vector2Int.right, 2 * Vector2Int.right, 3 * Vector2Int.right }),
			new MatchPattern(new[] { Vector2Int.zero, Vector2Int.up, 2 * Vector2Int.up, 3 * Vector2Int.up }),
		};
		public override MatchPattern[] AcceptableMatchPatterns => this.acceptableMatchPatterns;
	}
}