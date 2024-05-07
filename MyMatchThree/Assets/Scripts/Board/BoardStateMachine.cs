namespace GNMS.MatchThree
{
	public class BoardStateMachine : StateMachine.StateMachineBehaviour
	{
		Board board;
		BoardMatchCheckerOperative boardMatchCheckerOperative;

		BoardInteractionState boardInteractionState;
		BoardItemSwapState boardItemSwapState;
		BoardDestructionState boardDestructionState;

		protected override void ConstructStateMachine()
		{
			this.board = this.GetComponent<Board>();
			this.boardMatchCheckerOperative = this.GetComponent<BoardMatchCheckerOperative>();

			this.AddStateBehaviours();
			this.CreateStateTransitions();
			this.SetState(this.boardInteractionState);
		}

		void AddStateBehaviours()
		{
			this.boardInteractionState = this.gameObject.AddComponent<BoardInteractionState>();
			this.boardItemSwapState = this.gameObject.AddComponent<BoardItemSwapState>();
			this.boardDestructionState = this.gameObject.AddComponent<BoardDestructionState>();
		}

		void CreateStateTransitions()
		{
			this.boardInteractionState.AssignTransitionSignal(this.boardItemSwapState, this.board.OnItemSlideInput);

			this.boardItemSwapState.AssignTransitionSignal(this.boardInteractionState, this.board.OnInvalidSlideComplete);
			this.boardItemSwapState.AssignTransitionSignal(this.boardDestructionState, this.board.OnValidSlideComplete);

			this.boardDestructionState.AssignTransitionCondition(
				this.boardInteractionState,
				() => !this.board.InteractionIsStalled);
		}
	}
}