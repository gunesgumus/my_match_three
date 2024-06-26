namespace GNMS.MatchThree
{
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;

	public class MatchPattern
	{
		readonly Vector2Int[] relativeMatchPositions;
		Vector2Int matchExtentMin;
		Vector2Int matchExtentMax;

		public Vector2Int[] RelativePositions => this.relativeMatchPositions;
		public Vector2Int ExtentMin => this.matchExtentMin;
		public Vector2Int ExtentMax => this.matchExtentMax;

		public MatchPattern(Vector2Int[] relativePositions)
		{
			this.relativeMatchPositions = relativePositions;
			this.matchExtentMin = this.relativeMatchPositions
				.Aggregate(new Vector2Int(int.MaxValue, int.MaxValue), (extentMin, next) => Vector2Int.Min(extentMin, next));
			this.matchExtentMax = this.relativeMatchPositions
				.Aggregate(new Vector2Int(int.MinValue, int.MinValue), (extentMax, next) => Vector2Int.Max(extentMax, next));
		}

		public List<MatchItem> GetMatchedItems(Board board, Vector2Int matchLocation)
		{
			List<MatchItem> matchedItems = new List<MatchItem>();
			MatchItemColor? matchColor = null;
			foreach (Vector2Int relativeMatchPosition in this.relativeMatchPositions)
			{
				MatchItem matchItem = board.GetItemAtPosition(matchLocation + relativeMatchPosition) as MatchItem;
				if (matchItem == null || !matchItem.IsStabilized)
				{
					return new List<MatchItem>();
				}
				if (matchColor == null)
				{
					matchColor = matchItem.ItemColor;
				}
				if (matchItem.ItemColor != matchColor)
				{
					return new List<MatchItem>();
				}
				matchedItems.Add(matchItem);
			}
			return matchedItems;
		}

		public MatchItemColor? GetMatchColorToAvoidForTargetPosition(Board board, Vector2Int matchLocation, Vector2Int targetPosition)
		{

			if (this.relativeMatchPositions.All(relativeMatchPosition => matchLocation + relativeMatchPosition != targetPosition))
			{
				return null;
			}

			MatchItemColor? matchColor = null;
			foreach (Vector2Int relativeMatchPosition in this.relativeMatchPositions)
			{
				Vector2Int position = matchLocation + relativeMatchPosition;
				if (position == targetPosition)
				{
					continue;
				}
				MatchItem matchItem = board.GetItemAtPosition(position) as MatchItem;
				if (matchItem == null)
				{
					return null;
				}
				if (!matchColor.HasValue)
				{
					matchColor = matchItem.ItemColor;
				}
				if (matchItem.ItemColor != matchColor.Value)
				{
					return null;
				}
			}
			return matchColor;
		}
	}
}