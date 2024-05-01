using GNMS.StateMachine;
using Unity.VisualScripting;
using UnityEngine;

namespace GNMS.MatchThree.Test.StateMachineTest
{
	public class StateMachineTestObject : StateMachine.StateMachineBehaviour
	{
		TestStateA testStateA;
		TestStateB testStateB;

		SignalContainer OnPressedA = new SignalContainer();
		SignalContainer OnPressedB = new SignalContainer();

		public bool isOnB = false;

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.B))
			{
				this.isOnB = true;
				this.OnPressedB.EmitSignal();
			}
			if (Input.GetKeyDown(KeyCode.A))
			{
				this.isOnB = false;
				this.OnPressedA.EmitSignal();
			}
		}

		protected override void ConstructStateMachine()
		{
			this.AddStateBehaviours();
			this.CreateStateTransitions();
			this.SetState(this.testStateA);
		}

		void AddStateBehaviours()
		{
			this.testStateA = this.AddComponent<TestStateA>();
			this.testStateB = this.AddComponent<TestStateB>();
		}

		void CreateStateTransitions()
		{
			this.testStateA.AssignTransitionSignal(this.testStateB, this.OnPressedB);
			this.testStateB.AssignTransitionSignal(this.testStateA, this.OnPressedA);

			this.testStateA.AssignTransitionCondition(this.testStateB, () => this.isOnB);
			this.testStateB.AssignTransitionCondition(this.testStateA, () => !this.isOnB);
		}
	}
}