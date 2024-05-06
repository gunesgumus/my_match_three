namespace GNMS.MatchThree
{
	using UnityEngine;

	public class LShapedMatchPatternProvider : MatchPatternProvider
	{
		readonly MatchPattern[] acceptableMatchPatterns = new MatchPattern[]
		{
			new MatchPattern(new[] { Vector2Int.zero,
										Vector2Int.up,
										2 * Vector2Int.up,
										2 * Vector2Int.up + Vector2Int.right,
										2 * Vector2Int.up + 2 * Vector2Int.right }),
			new MatchPattern(new[] { Vector2Int.zero,
										Vector2Int.up,
										2 * Vector2Int.up,
										2 * Vector2Int.up + Vector2Int.left,
										2 * Vector2Int.up + 2 * Vector2Int.left }),
			new MatchPattern(new[] { Vector2Int.zero,
										Vector2Int.down,
										2 * Vector2Int.down,
										2 * Vector2Int.down + Vector2Int.right,
										2 * Vector2Int.down + 2 * Vector2Int.right }),
			new MatchPattern(new[] { Vector2Int.zero,
										Vector2Int.down,
										2 * Vector2Int.down,
										2 * Vector2Int.down + Vector2Int.left,
										2 * Vector2Int.down + 2 * Vector2Int.left }),
			new MatchPattern(new[] { Vector2Int.zero,
										Vector2Int.right,
										2 * Vector2Int.right,
										2 * Vector2Int.right + Vector2Int.up,
										2 * Vector2Int.right + 2 * Vector2Int.up }),
			new MatchPattern(new[] { Vector2Int.zero,
										Vector2Int.right,
										2 * Vector2Int.right,
										2 * Vector2Int.right + Vector2Int.down,
										2 * Vector2Int.right + 2 * Vector2Int.down }),
			new MatchPattern(new[] { Vector2Int.zero,
										Vector2Int.left,
										2 * Vector2Int.left,
										2 * Vector2Int.left + Vector2Int.up,
										2 * Vector2Int.left + 2 * Vector2Int.up }),
			new MatchPattern(new[] { Vector2Int.zero,
										Vector2Int.left,
										2 * Vector2Int.left,
										2 * Vector2Int.left + Vector2Int.down,
										2 * Vector2Int.left + 2 * Vector2Int.down }),
		};
		public override MatchPattern[] AcceptableMatchPatterns => this.acceptableMatchPatterns;
	}
}