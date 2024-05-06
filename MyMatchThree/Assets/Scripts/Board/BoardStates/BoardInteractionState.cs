namespace GNMS.MatchThree
{
	public class BoardInteractionState : StateMachine.StateBehaviour
	{
		BoardInteractionOperative boardInteractionOperative;

		private void Awake()
		{
			this.boardInteractionOperative = this.GetComponent<BoardInteractionOperative>();
		}

		protected override void AfterEnter()
		{
			this.boardInteractionOperative.enabled = true;
		}

		protected override void BeforeExit()
		{
			this.boardInteractionOperative.enabled = false;
		}

		protected override void PreFixedUpdate()
		{

		}

		protected override void PostFixedUpdate()
		{

		}
	}
}