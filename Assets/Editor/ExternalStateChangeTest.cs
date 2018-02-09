using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using Beatrate.BeFSM;

public class ExternalStateChangeTest
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

	[Test(Description = "State changes requested from outside the machine")]
	public void ExternalStateChange()
	{
		var machine = new StateMachine<LampState, LampTrigger>(LampState.Off);
		machine.Initialize();
		Assert.DoesNotThrow(() => machine.TransitionTo(LampState.On));
	}
}