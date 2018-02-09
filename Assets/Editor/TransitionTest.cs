using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using Beatrate.BeFSM;

public class TransitionTest
{
	private enum Food
	{
		Cake,
		Cookie,
		Ham,
		Donut,
		Banana
	}

	private enum Trigger
	{
		Honk,
		Bark,
		Scream,
		Whisper
	}

	[Test(Description = "Trigger transition")]
	public void TriggerTransition()
	{
		var machine = new StateMachine<Food, Trigger>(Food.Banana);
		machine.Configure(Food.Banana).Permit(Trigger.Bark, Food.Cake);
		machine.Initialize();
		machine.Activate(Trigger.Bark);
		Assert.AreEqual(machine.CurrentState, Food.Cake);
	}

	[Test(Description = "Immediate transition")]
	public void ImmediateTransitionChaining()
	{
		var machine = new StateMachine<Food, Trigger>(Food.Banana);
		machine.Configure(Food.Banana).PermitImmediate(Food.Cake);
		machine.Configure(Food.Cake).PermitImmediate(Food.Donut);
		machine.Initialize();
		Assert.AreEqual(machine.CurrentState, Food.Donut);
	}

	[Test(Description = "Trigger transition with false predicate")]
	public void TriggerTransitionWithFalsePredicate()
	{
		var machine = new StateMachine<Food, Trigger>(Food.Banana);
		machine.Configure(Food.Banana).PermitIf(Trigger.Bark, Food.Cake, () => false);
		machine.Initialize();
		machine.Activate(Trigger.Bark);
		Assert.AreEqual(machine.CurrentState, Food.Banana);
	}

	[Test(Description = "Trigger transition with true predicate")]
	public void TriggerTransitionWithTruePredicate()
	{
		var machine = new StateMachine<Food, Trigger>(Food.Banana);
		machine.Configure(Food.Banana).PermitIf(Trigger.Bark, Food.Cake, () => true);
		machine.Initialize();
		machine.Activate(Trigger.Bark);
		Assert.AreEqual(machine.CurrentState, Food.Cake);
	}

	[Test(Description = "Dynamic transition")]
	public void DynamicTransition()
	{
		var machine = new StateMachine<Food, Trigger>(Food.Ham);
		bool dynamicParameter = false;
		machine.Configure(Food.Ham).PermitDynamic(Food.Cookie, () => dynamicParameter);
		machine.Initialize();
		machine.Update();
		Assert.AreEqual(machine.CurrentState, Food.Ham);
		dynamicParameter = true;
		machine.Update();
		Assert.AreEqual(machine.CurrentState, Food.Cookie);
	}
}