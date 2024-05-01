using GNMS.StateMachine;
using UnityEngine;

namespace GNMS.MatchThree.Test.StateMachineTest
{
	public class TestStateB : StateBehaviour
	{
		protected override void AfterEnter()
		{
			Debug.LogWarning($"Entered test state B");
		}

		protected override void AfterEvaluate()
		{
			Debug.Log($"Evaluated test state B");
		}

		protected override void BeforeEvaluate()
		{
			Debug.Log($"Evaluating test state B");
		}

		protected override void BeforeExit()
		{
			Debug.LogWarning($"Exiting test state B");
		}
	}
}