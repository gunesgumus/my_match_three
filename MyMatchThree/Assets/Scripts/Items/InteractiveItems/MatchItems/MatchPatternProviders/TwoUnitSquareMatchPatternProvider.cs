namespace GNMS.MatchThree
{
	using UnityEngine;

	public class TwoUnitSquareMatchPatternProvider : MatchPatternProvider
	{
		readonly MatchPattern[] acceptableMatchPatterns = new MatchPattern[]
		{
			new MatchPattern(new[] { Vector2Int.zero, Vector2Int.right, Vector2Int.up, Vector2Int.right + Vector2Int.up }),
		};
		public override MatchPattern[] AcceptableMatchPatterns => this.acceptableMatchPatterns;
	}
}