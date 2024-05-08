namespace GNMS.MatchThree
{
	using System.Collections;
	using UnityEngine;

	public abstract class MovingItem : Item
	{
		const float movementDuration = 0.2f;
		const float dropSpeedAcceleration = 20f;
		const float maxDropSpeed = 10f;

		bool isStabilized = true;
		Coroutine dropCoroutine = null;

		public bool IsStabilized => this.isStabilized;

		public void MoveToPosition(Vector3 targetPosition)
		{
			StartCoroutine(this.MoveToPositionCoroutine(targetPosition, MovingItem.movementDuration));
		}

		public void DropToPosition(Vector3 targetPosition)
		{
			if (this.dropCoroutine != null)
			{
				StopCoroutine(this.dropCoroutine);
			}
			this.dropCoroutine = StartCoroutine(this.DropToPositionCoroutine(targetPosition, MovingItem.movementDuration));
		}

		public void SetStabilized(bool isStable)
		{
			this.isStabilized = isStable;
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
		}

		IEnumerator DropToPositionCoroutine(Vector3 targetPosition, float duration)
		{
			this.isStabilized = false;
			float dropSpeed = 0f;
			while (true)
			{
				yield return new WaitForEndOfFrame();
				dropSpeed = Mathf.Min(dropSpeed + MovingItem.dropSpeedAcceleration * Time.deltaTime, MovingItem.maxDropSpeed);
				this.transform.position = Vector3.MoveTowards(this.transform.position, targetPosition, Time.deltaTime * dropSpeed);
				if (this.transform.position == targetPosition)
				{
					break;
				}
			}
			this.transform.position = targetPosition;
			this.isStabilized = true;
			yield return new WaitForEndOfFrame();
			if (this is MatchItem matchItem)
			{
				this.board.CheckAndRegisterForMatch(matchItem);
			}
		}
	}
}