using UnityEngine;

public static class StateManager  
{
	private static GameState state = GameState.Editing;
	public static GameState CurrentState { get { return state; } }

	public static void ChangeState(GameState newState)
	{
		state = newState;
		EventManager.Notify("StateChanged", new Data(newState));
	}
}
