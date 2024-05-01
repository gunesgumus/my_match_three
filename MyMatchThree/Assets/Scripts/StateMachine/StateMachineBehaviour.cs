﻿using UnityEngine;

namespace GNMS.StateMachine
{
	public abstract class StateMachineBehaviour : MonoBehaviour
	{
		StateBehaviour activeState;

		private void Awake()
		{
			this.ConstructStateMachine();
		}

		private void OnDestroy()
		{
			if (this.activeState != null)
			{
				this.activeState.OnStateTransition -= this.HandleStateTransition;
			}
		}

		private void FixedUpdate()
		{
			if (this.activeState == null)
			{
				return;
			}

			this.activeState.EvaluateState();
		}

		public void SetState(StateBehaviour state)
		{
			this.ExitActiveStateIfExists();

			this.activeState = state;
			state.OnStateTransition += this.HandleStateTransition;
			this.activeState.EnterState();
		}

		protected abstract void ConstructStateMachine();

		void ExitActiveStateIfExists()
		{
			if (this.activeState == null)
			{
				return;
			}

			this.activeState.OnStateTransition -= this.HandleStateTransition;
			this.activeState.ExitState();
		}

		void HandleStateTransition(StateBehaviour state)
		{
			this.SetState(state);
		}
	}
}