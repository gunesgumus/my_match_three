namespace GNMS.MatchThree.Test
{
	using GNMS.StateMachine;
	using UnityEngine;

	public class TestStateA : StateBehaviour
	{
		protected override void AfterEnter()
		{
			Debug.LogWarning($"Entered test state A");
		}

		protected override void BeforeExit()
		{
			Debug.LogWarning($"Exiting test state A");
		}

		protected override void PostFixedUpdate()
		{
			Debug.Log($"Evaluated test state A");
		}

		protected override void PreFixedUpdate()
		{
			Debug.Log($"Evaluating test state A");
		}
	}
}
