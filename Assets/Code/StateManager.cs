//
//  StateManager.cs
//  BoardRPG
//
//  Created by Jason Bricco on 2/7/16.
//  Copyright © 2016 Jason Bricco. All rights reserved.
//

using UnityEngine;

public enum GameState { Playing, Editing }

public static class StateManager  
{
	private static GameState state = GameState.Editing;
	public static GameState CurrentState { get { return state; } }

	public static void ChangeState(GameState newState)
	{
		state = newState;
		EventManager.TriggerEvent("StateChanged", newState);
	}
}
