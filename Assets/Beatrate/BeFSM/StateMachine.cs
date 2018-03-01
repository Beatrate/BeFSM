using System;
using System.Collections.Generic;

namespace Beatrate.BeFSM
{
	/// <summary>
	/// State machine object.
	/// </summary>
	/// <typeparam name="TState">State type</typeparam>
	/// <typeparam name="TTrigger">Trigger type</typeparam>
	public partial class StateMachine<TState, TTrigger>
	{
		private Dictionary<TState, StateConfiguration> stateConfigurations = new Dictionary<TState, StateConfiguration>();

		public StateMachine(TState initialState)
		{
			CurrentState = initialState;
			EnsureStateConfiguration(CurrentState);
		}

		public TState CurrentState { get; private set; }
		public event EventHandler<StateChangedEventArgs<TState>> StateChanged;

		/// <summary>
		/// Initiates configuration of the state.
		/// </summary>
		/// <param name="state">State to configure</param>
		/// <returns></returns>
		public StateConfiguration Configure(TState state)
		{
			EnsureStateConfiguration(state);
			return stateConfigurations[state];
		}

		/// <summary>
		/// Tries to activate the trigger if it's possible, silently fails otherwise.
		/// </summary>
		/// <param name="trigger">The trigger to be activated.</param>
		public void Activate(TTrigger trigger)
		{
			StateCondition stateCondition;
			if(stateConfigurations[CurrentState].TryActivateTrigger(trigger, out stateCondition))
			{
				if(stateCondition.Condition())
				{
					TransitionTo(stateCondition.DestinationState);
				}
			}
		}

		/// <summary>
		/// Initializes the starting state.
		/// </summary>
		public void Initialize()
		{
			stateConfigurations[CurrentState].EnterAction(CurrentState);
			if(stateConfigurations[CurrentState].HasImmediateTransition)
			{
				TransitionTo(stateConfigurations[CurrentState].ImmediateTransitionDestinationState);
			}
		}

		/// <summary>
		/// Checks for dynamic transitions and calls the state's update.
		/// </summary>
		public void Update()
		{
			StateCondition stateCondition;
			if(stateConfigurations[CurrentState].TryTransitionDynamic(out stateCondition))
			{
				TransitionTo(stateCondition.DestinationState);
			}
			stateConfigurations[CurrentState].UpdateAction();
		}

		/// <summary>
		/// Ensures that there is a configuration for the given state.
		/// </summary>
		/// <param name="state"></param>
		public void EnsureStateConfiguration(TState state)
		{
			if(!stateConfigurations.ContainsKey(state))
			{
				stateConfigurations.Add(state, new StateConfiguration(this));
			}
		}

		/// <summary>
		/// Transitions the state.
		/// </summary>
		/// <param name="nextState">State to transition to</param>
		public void TransitionTo(TState nextState)
		{
			// Prevent null references when the state changes
			// are coming from outside the state machine and the
			// new state was not configured.
			EnsureStateConfiguration(nextState);
			stateConfigurations[CurrentState].ExitAction(nextState);
			stateConfigurations[nextState].EnterAction(CurrentState);
			CurrentState = nextState;
			OnStateChanged(new StateChangedEventArgs<TState>(nextState));
			if(stateConfigurations[CurrentState].HasImmediateTransition)
			{
				TransitionTo(stateConfigurations[CurrentState].ImmediateTransitionDestinationState);
			}
		}

		private void OnStateChanged(StateChangedEventArgs<TState> args)
		{
			EventHandler<StateChangedEventArgs<TState>> handler = StateChanged;
			if(handler != null)
			{
				handler(this, args);
			}
		}
	}
}