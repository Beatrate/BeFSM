using System;
using System.Collections.Generic;

namespace Beatrate.BeFSM
{
	public partial class StateMachine<TState, TTrigger>
	{
		/// <summary>
		/// Stores state specific actions.
		/// </summary>
		public class StateConfiguration
		{
			private StateMachine<TState, TTrigger> machine;
			private Dictionary<TTrigger, StateCondition> staticTransitions = new Dictionary<TTrigger, StateCondition>();
			private List<StateCondition> dynamicTransitions = new List<StateCondition>();
			public Action EnterAction { get; private set; } = () => { };
			public Action ExitAction { get; private set; } = () => { };
			public Action UpdateAction { get; private set; } = () => { };
			public bool HasImmediateTransition { get; private set; } = false;
			public TState ImmediateTransitionDestinationState
			{
				get
				{
					if(!HasImmediateTransition)
					{
						throw new ArgumentNullException(nameof(immediateTransitionDestinationState));
					}
					return immediateTransitionDestinationState;
				}
				set
				{
					immediateTransitionDestinationState = value;
					HasImmediateTransition = true;
				}
			}
			private TState immediateTransitionDestinationState;

			public StateConfiguration(StateMachine<TState, TTrigger> machine)
			{
				this.machine = machine;
			}

			/// <summary>
			/// Allows transition to the new state if the trigger is active.
			/// </summary>
			/// <param name="trigger">Trigger to activate</param>
			/// <param name="destinationState">State to transition to</param>
			/// <returns>Configuration</returns>
			public StateConfiguration Permit(TTrigger trigger, TState destinationState)
			{
				machine.EnsureStateConfiguration(destinationState);
				staticTransitions.Add(trigger, new StateCondition(destinationState, () => true));
				return this;
			}

			/// <summary>
			/// Allows transition to the new state if the trigger is active and the predicate returns true.
			/// </summary>
			/// <param name="trigger">Trigger to activate</param>
			/// <param name="destinationState">State to transition to</param>
			/// <param name="predicate">Transition predicate</param>
			/// <returns>Configuration</returns>
			public StateConfiguration PermitIf(TTrigger trigger, TState destinationState, Func<bool> predicate)
			{
				machine.EnsureStateConfiguration(destinationState);
				staticTransitions.Add(trigger, new StateCondition(destinationState, predicate));
				return this;
			}

			/// <summary>
			/// Allows transition to the new state if the predicate return true this frame.
			/// </summary>
			/// <param name="destinationState">State to transition to</param>
			/// <param name="predicate">Transition predicate</param>
			/// <returns>Configuration</returns>
			public StateConfiguration PermitDynamic(TState destinationState, Func<bool> predicate)
			{
				machine.EnsureStateConfiguration(destinationState);
				dynamicTransitions.Add(new StateCondition(destinationState, predicate));
				return this;
			}

			/// <summary>
			/// Allows to chain a transition to the new state immediately after entering this one.
			/// </summary>
			/// <param name="destinationState"></param>
			/// <returns>Configuration</returns>
			public StateConfiguration PermitImmediate(TState destinationState)
			{
				machine.EnsureStateConfiguration(destinationState);
				ImmediateTransitionDestinationState = destinationState;
				return this;
			}

			/// <summary>
			/// Configures state's entry action.
			/// </summary>
			/// <param name="enterAction">Entry action</param>
			/// <returns>Configuration</returns>
			public StateConfiguration OnEntry(Action enterAction)
			{
				EnterAction = enterAction;
				return this;
			}

			/// <summary>
			/// Configures state's exit action.
			/// </summary>
			/// <param name="exitAction">Exit action</param>
			/// <returns>Configuration</returns>
			public StateConfiguration OnExit(Action exitAction)
			{
				ExitAction = exitAction;
				return this;
			}

			/// <summary>
			/// Configures state's update action that's called every frame.
			/// </summary>
			/// <param name="updateAction"></param>
			/// <returns>Configuration</returns>
			public StateConfiguration OnUpdate(Action updateAction)
			{
				UpdateAction = updateAction;
				return this;
			}

			public bool TryActivateTrigger(TTrigger trigger, out StateCondition stateCondition)
			{
				return staticTransitions.TryGetValue(trigger, out stateCondition);
			}

			public bool TryTransitionDynamic(out StateCondition stateCondition)
			{
				foreach(StateCondition transitionCondition in dynamicTransitions)
				{
					if(transitionCondition.Condition())
					{
						stateCondition = transitionCondition;
						return true;
					}
				}
				stateCondition = null;
				return false;
			}
		}
	}
}
