namespace GNMS.MatchThree.Test
{
	using GNMS.StateMachine;
	using UnityEngine;

	public class TestStateB : StateBehaviour
	{
		protected override void AfterEnter()
		{
			Debug.LogWarning($"Entered test state B");
		}

		protected override void BeforeExit()
		{
			Debug.LogWarning($"Exiting test state B");
		}

		protected override void PreFixedUpdate()
		{
			Debug.Log($"Evaluating test state B");
		}

		protected override void PostFixedUpdate()
		{
			Debug.Log($"Evaluated test state B");
		}
	}
}