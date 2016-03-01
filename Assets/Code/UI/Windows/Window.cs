using UnityEngine;

public class Window : MonoBehaviour, IUpdatable
{
	[SerializeField] protected BoardManager boardManager;
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
		if (Input.GetKeyDown(enableKey))
		{
			if (StateManager.CurrentState == GameState.Window)
			{
				if (gameObject.activeSelf)
					gameObject.SetActive(false);
			}
			else
			{
				if (StateManager.CurrentState == GameState.Editing)
					gameObject.SetActive(true);
			}
		}
	}
}
