using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using Beatrate.BeFSM;

public class StateChangedEventTest
{
	private enum LampState
	{
		Off,
		On
	}

	private enum LampTrigger
	{
		Switch
	}

	[Test(Description = "State change event")]
	public void StateChangedEvent()
	{
		var machine = new StateMachine<LampState, LampTrigger>(LampState.Off);
		machine.Configure(LampState.Off).PermitImmediate(LampState.On);
		LampState caughtState = LampState.Off;
		machine.StateChanged += (sender, args) => caughtState = args.NextState;
		machine.Initialize();
		Assert.AreEqual(caughtState, LampState.On);
	}
}