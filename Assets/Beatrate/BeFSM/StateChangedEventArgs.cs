using System;

namespace Beatrate.BeFSM
{
	/// <summary>
	/// Args for new state transition.
	/// </summary>
	/// <typeparam name="TState">State type</typeparam>
	public class StateChangedEventArgs<TState> : EventArgs
	{
		public TState NextState { get; private set; }

		public StateChangedEventArgs(TState nextState)
		{
			NextState = nextState;
		}
	}
}