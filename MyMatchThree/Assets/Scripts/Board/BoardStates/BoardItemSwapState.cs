namespace GNMS.MatchThree
{
	using System.Collections;
	using UnityEngine;

	public class BoardItemSwapState : StateMachine.StateBehaviour
	{
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
				this.board.SwapItems(primarySwapItem, secondarySwapItem);
				yield return new WaitUntil(() => primarySwapItem.IsStabilized && secondarySwapItem.IsStabilized);
				this.board.HandleInvalidSlide();
			}
		}
	}
}