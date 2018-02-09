using System;

namespace Beatrate.BeFSM
{
	public partial class StateMachine<TState, TTrigger>
	{
		/// <summary>
		/// Stores the state and the condition to reach it.
		/// </summary>
		public class StateCondition
		{
			public TState DestinationState { get; private set; }
			public Func<bool> Condition { get; private set; }

			public StateCondition(TState destinationState, Func<bool> condition)
			{
				DestinationState = destinationState;
				Condition = condition;
			}
		}
	}
}
