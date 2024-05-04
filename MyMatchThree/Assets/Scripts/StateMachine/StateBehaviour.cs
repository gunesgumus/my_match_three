namespace GNMS.StateMachine
{
	using System;
	using System.Collections.Generic;
	using UnityEngine;

	public abstract class StateBehaviour : MonoBehaviour
	{
		public Action<StateBehaviour> OnStateTransition;

		protected StateMachineBehaviour stateMachine;

		Dictionary<SignalContainer, StateBehaviour> transitionSignalsAndTargetStates = new Dictionary<SignalContainer, StateBehaviour>();
		Dictionary<Func<bool>, StateBehaviour> transitionConditionsAndTargetStates = new Dictionary<Func<bool>, StateBehaviour>();

		private void OnDestroy()
		{
			foreach (SignalContainer transitionSignal in this.transitionSignalsAndTargetStates.Keys)
			{
				transitionSignal.RemoveListener(this.HandleTransitionSignal);
			}
			this.transitionConditionsAndTargetStates.Clear();
		}

		public void AssignTransitionSignal(StateBehaviour targetState, SignalContainer transitionSignal)
		{
			if (this.transitionSignalsAndTargetStates.ContainsKey(transitionSignal))
			{
				return;
			}
			this.transitionSignalsAndTargetStates[transitionSignal] = targetState;
		}

		public void AssignTransitionCondition(StateBehaviour targetState, Func<bool> transitionCondition)
		{
			if (this.transitionConditionsAndTargetStates.ContainsKey(transitionCondition))
			{
				return;
			}
			this.transitionConditionsAndTargetStates[transitionCondition] = targetState;
		}

		public void StateFixedUpdate()
		{
			this.PreFixedUpdate();
			foreach (Func<bool> transitionCondition in this.transitionConditionsAndTargetStates.Keys)
			{
				bool evaluationResult = transitionCondition();
				if (evaluationResult)
				{
					StateBehaviour targetState = this.transitionConditionsAndTargetStates[transitionCondition];
					this.OnStateTransition?.Invoke(targetState);
					break;
				}
			}
			this.PostFixedUpdate();
		}

		public void EnterState()
		{
			this.SubscribeToTransitionSignals();
			this.AfterEnter();
		}

		public void ExitState()
		{
			this.BeforeExit();
			this.UnsubscribeFromTransitionSignals();
		}

		protected abstract void AfterEnter();
		protected abstract void BeforeExit();
		protected abstract void PostFixedUpdate();
		protected abstract void PreFixedUpdate();

		void SubscribeToTransitionSignals()
		{
			foreach (SignalContainer transitionSignal in this.transitionSignalsAndTargetStates.Keys)
			{
				transitionSignal.AddListener(this.HandleTransitionSignal);
			}
		}

		void UnsubscribeFromTransitionSignals()
		{
			foreach (SignalContainer transitionSignal in this.transitionSignalsAndTargetStates.Keys)
			{
				transitionSignal.RemoveListener(this.HandleTransitionSignal);
			}
		}

		void HandleTransitionSignal(SignalContainer transitionSignal)
		{
			if (!this.transitionSignalsAndTargetStates.ContainsKey(transitionSignal))
			{
				return;
			}
			StateBehaviour targetState = this.transitionSignalsAndTargetStates[transitionSignal];
			this.OnStateTransition?.Invoke(targetState);
		}
	}
}