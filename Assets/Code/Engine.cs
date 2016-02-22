using UnityEngine;
using System.Collections.Generic;

public delegate void Method();

public sealed class Engine : MonoBehaviour
{
	[SerializeField] private CameraControl camControl;

	private static List<IUpdatable> updateList = new List<IUpdatable>();

	private static GameObject instance;
	public static GameObject Instance { get { return instance; } }

	private void Awake()
	{
		instance = gameObject;
	
		StateManager.ChangeState(GameState.Editing);
		EventManager.StartListening("ExitPressed", ExitPressedHandler);
	}

	public static void StartUpdating(IUpdatable item)
	{
		if (!updateList.Contains(item))
			updateList.Add(item);
	}

	public static void StopUpdating(IUpdatable item)
	{
		updateList.Remove(item);
	}

	private void Update()
	{
		for (int i = 0; i < updateList.Count; i++)
			updateList[i].UpdateFrame();
	}

	private void ExitPressedHandler(int data)
	{
		Application.Quit();
	}

	private void OnApplicationQuit()
	{
		EventManager.TriggerEvent("Quit");
	}
}
