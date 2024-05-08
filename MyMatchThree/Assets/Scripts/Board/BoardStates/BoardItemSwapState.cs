namespace GNMS.MatchThree
{
	using System.Collections;
	using UnityEngine;

	public class BoardItemSwapState : StateMachine.StateBehaviour
	{
		const float waitDurationAfterInvalidSwap = 0.5f;

		Board board;

		private void Awake()
		{
			this.board = this.GetComponent<Board>();
		}

		protected override void AfterEnter()
		{
			StartCoroutine(this.SwapItemsCoroutine());
		}

		protected override void BeforeExit()
		{

		}

		protected override void PreFixedUpdate()
		{

		}

		protected override void PostFixedUpdate()
		{

		}

		IEnumerator SwapItemsCoroutine()
		{
			MovingItem primarySwapItem = this.board.PrimarySwapItem;
			MovingItem secondarySwapItem = this.board.SecondarySwapItem;
			this.board.SwapItems(primarySwapItem, secondarySwapItem);
			yield return new WaitUntil(() => primarySwapItem.IsStabilized && secondarySwapItem.IsStabilized);
			bool primaryItemPlacementIsValid = this.board.CheckItemBoardPlacement(primarySwapItem);
			bool secondaryItemPlacementIsValid = this.board.CheckItemBoardPlacement(secondarySwapItem);
			bool movementIsValid = primaryItemPlacementIsValid || secondaryItemPlacementIsValid;
			if (movementIsValid)
			{
				this.board.HandleValidSlide();
			}
			else
			{
				// prevent items from being calculated for other match checks while animating
				// after swapped back, they will get stable automatically
				primarySwapItem.SetStabilized(false);
				secondarySwapItem.SetStabilized(false);

				yield return new WaitForSeconds(BoardItemSwapState.waitDurationAfterInvalidSwap);
				this.board.SwapItems(primarySwapItem, secondarySwapItem);
				yield return new WaitUntil(() => primarySwapItem.IsStabilized && secondarySwapItem.IsStabilized);
				this.board.HandleInvalidSlide();
			}
		}
	}
}