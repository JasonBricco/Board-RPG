using UnityEngine;

public class Window : MonoBehaviour, IUpdatable
{
	protected KeyCode enableKey = KeyCode.Alpha0;

	public virtual void Initialize()
	{
	}

	public void OnEnable()
	{
		StateManager.ChangeState(GameState.Window);
	}

	public void OnDisable()
	{
		if (!Engine.IsQuitting)
			StateManager.ChangeState(GameState.Editing);
	}

	public void UpdateFrame()
	{
		GameState state = StateManager.CurrentState;

		if (state == GameState.SelectingCoords) return;

		if (Input.GetKeyDown(enableKey))
		{
			if (state == GameState.Window)
			{
				if (gameObject.activeSelf)
					gameObject.SetActive(false);
			}
			else
			{
				if (state == GameState.Editing)
					gameObject.SetActive(true);
			}
		}
	}
}
