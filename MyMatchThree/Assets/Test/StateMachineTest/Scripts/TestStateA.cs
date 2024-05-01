using GNMS.StateMachine;
using UnityEngine;

namespace GNMS.MatchThree.Test.StateMachineTest
{
	public class TestStateA : StateBehaviour
	{
		protected override void AfterEnter()
		{
			Debug.LogWarning($"Entered test state A");
		}

		protected override void AfterEvaluate()
		{
			Debug.Log($"Evaluated test state A");
		}

		protected override void BeforeEvaluate()
		{
			Debug.Log($"Evaluating test state A");
		}

		protected override void BeforeExit()
		{
			Debug.LogWarning($"Exiting test state A");
		}
	}
}
