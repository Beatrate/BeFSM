# BeFSM

**State machine library for Unity. Inspired by [Stateless](https://github.com/dotnet-state-machine/stateless).**

```csharp
var car = new StateMachine<CarState, CarAction>(State.Idle);
car.Configure(CarState.Idle)
	.Permit(CarAction.Ignition, CarState.Running);
car.Configure(CarState.Running)
	.PermitIf(CarAction.Crashed, CarState.Wrecked, () => health == 0);
car.Configure(CarState.Running)
	.PermitDynamic(CarState.Idle, () => fuel == 0);
car.Configure(CarState.Running)
	.Permit(CarAction.BoosterActivated, CarState.Boosting);
car.Configure(CarState.Boosting)
	.OnEntry(BoostVelocity)
	.PermitImmediate(CarState.Running);
	
car.Initialize();
car.Activate(CarAction.Ignition);
```

## Download

Get the [unitypackage](https://github.com/Beatrate/BeFSM/releases)

## Features

Standard state machine features supported:

* States and triggers of any type.
* Entry/exit state events.
* Conditional trigger transitions.

**Unity**-oriented features:

* State update events.
* Conditional transitions without triggers.
* Immediate transitions for intermediate states.

### Entry/Exit/Update Events

```csharp
public class ItemActor : MonoBehaviour
{
	// ...
	
	private void Awake()
	{
		itemMachine = new StateMachine<ItemState, ItemTrigger>(ItemState.Dropped);
		itemMachine.Configure(ItemState.Dropped)
			.OnEntry(() => Debug.Log("Dropped."))
			.OnExit(() => Debug.Log("Picked up"))
			.OnUpdate(EmitAura);
		itemMachine.Initialize();
	}
	
	private void Update()
	{
		itemMachine.Update();
	}
	
	// ...
}
```

### Trigger Transitions

The state machine will transition to a new state as soon as the trigger is called.
```csharp
car.Configure(CarState.Idle)
	.Permit(CarAction.Ignition, CarState.Running);
```
Unhandled triggers for the current state are ignored.

### Conditional Transitions

Transitions can be triggered by conditional triggers or can be based solely on external variable changes.
```csharp
trader.Configure(TraderState.Waiting)
	.PermitIf(TraderTrigger.Insulted, TraderState.Closed, () => reputation <= 0)
	.PermitDynamic(TraderState.Closed, () => mood.Mediocre);
```

### Intermediate States

Immediate transitions can be configured for intermediate states that need to do useful work and transition to another state right away.

```csharp
movement.Configure(MoveState.Jumping)
	.OnEntry(() => AddJumpImpulse)
	.PermitImmediate(MoveState.Falling);
```

### State Change Events

StateChanged event can be used for observing state changes.
```csharp
var baseMachine = new Machine<State, Trigger>(State.Default);
var observerMachine = new Machine<State, Trigger>(State.Default);
baseMachine.StateChanged += (Object sender, StateChangedEventArgs<State> args) => observerMachine.TransitionTo(args.NextState);
```


## Compatibility

Compatible with Unity versions starting from Unity 2017.1 with experimental Mono runtime enabled.
