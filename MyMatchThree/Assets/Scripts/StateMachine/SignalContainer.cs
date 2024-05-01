using System;

namespace GNMS.StateMachine
{
	public class SignalContainer
	{
		public Action<SignalContainer> signal;

		public void EmitSignal()
		{
			this.signal?.Invoke(this);
		}

		public void AddListener(Action<SignalContainer> listener)
		{
			this.signal += listener;
		}

		public void RemoveListener(Action<SignalContainer> listener)
		{
			this.signal -= listener;
		}
	}
}