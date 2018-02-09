using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using Beatrate.BeFSM;

public class TransitionActionTest
{
	private enum DoorState
	{
		Open,
		Closed
	}

	private enum Interaction
	{
		Knock,
		Kick
	}

	[Test(Description = "State entry action")]
	public void Entry()
	{
		var machine = new StateMachine<DoorState, Interaction>(DoorState.Closed);
		bool indicator = false;
		machine.Configure(DoorState.Closed)
			.OnEntry(() => indicator = true)
			.Permit(Interaction.Kick, DoorState.Open);
		machine.Configure(DoorState.Open).OnEntry(() => indicator = false);
		machine.Initialize();
		Assert.True(indicator);
		machine.Activate(Interaction.Kick);
		Assert.False(indicator);
	}

	[Test(Description = "State exit action")]
	public void Exit()
	{
		var machine = new StateMachine<DoorState, Interaction>(DoorState.Closed);
		bool indicator = false;
		machine.Configure(DoorState.Closed)
			.OnExit(() => indicator = true)
			.Permit(Interaction.Knock, DoorState.Open);
		machine.Initialize();
		machine.Activate(Interaction.Knock);
		Assert.True(indicator);
	}

	[Test(Description = "State update action")]
	public void Update()
	{
		var machine = new StateMachine<DoorState, Interaction>(DoorState.Closed);
		bool indicator = false;
		machine.Configure(DoorState.Closed).OnUpdate(() => indicator = !indicator);
		machine.Initialize();
		machine.Update();
		Assert.True(indicator);
		machine.Update();
		Assert.False(indicator);
	}
}