namespace GNMS.MatchThree
{
	using System;
	using System.Collections;
	using UnityEngine;

	public abstract class MovingItem : Item
	{
		const float movementDuration = 0.2f;

		public Action<MovingItem> OnMovementComplete;

		bool isStabilized = true;

		public bool IsStabilized => this.isStabilized;

		public void MoveToPosition(Vector3 targetPosition)
		{
			StartCoroutine(this.MoveToPositionCoroutine(targetPosition, MovingItem.movementDuration));
		}

		IEnumerator MoveToPositionCoroutine(Vector3 targetPosition, float duration)
		{
			this.isStabilized = false;
			float movementTimer = 0f;
			Vector3 startPosition = this.transform.position;
			while (movementTimer < duration)
			{
				yield return new WaitForEndOfFrame();
				movementTimer += Time.deltaTime;
				this.transform.position = Vector3.Lerp(startPosition, targetPosition, movementTimer / duration);
			}
			this.transform.position = targetPosition;
			this.isStabilized = true;
			this.OnMovementComplete?.Invoke(this);
		}
	}
}