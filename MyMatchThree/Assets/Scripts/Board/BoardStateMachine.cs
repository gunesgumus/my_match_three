namespace GNMS.MatchThree
{
	public class BoardStateMachine : StateMachine.StateMachineBehaviour
	{
		Board board;

		BoardInteractionState boardInteractionState;
		BoardPieceSwapState boardPieceSwapState;
		BoardDestructionState boardDestructionState;

		protected override void ConstructStateMachine()
		{
			this.board = this.GetComponent<Board>();

			this.AddStateBehaviours();
			this.CreateStateTransitions();
			this.SetState(this.boardInteractionState);
		}

		void AddStateBehaviours()
		{
			this.boardInteractionState = this.gameObject.AddComponent<BoardInteractionState>();
			this.boardPieceSwapState = this.gameObject.AddComponent<BoardPieceSwapState>();
			this.boardDestructionState = this.gameObject.AddComponent<BoardDestructionState>();
		}

		void CreateStateTransitions()
		{
			this.boardInteractionState.AssignTransitionSignal(this.boardPieceSwapState, this.board.OnItemSlideInput);

			this.boardPieceSwapState.AssignTransitionSignal(this.boardInteractionState, this.board.OnInvalidSlideComplete);
			this.boardPieceSwapState.AssignTransitionSignal(this.boardDestructionState, this.board.OnValidSlideComplete);

			this.boardDestructionState.AssignTransitionSignal(this.boardInteractionState, this.board.OnDestructionComplete);
		}
	}
}