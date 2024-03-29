﻿using UnityEngine;
using System.Collections.Generic;

public sealed class Engine : MonoBehaviour
{
	private static List<IUpdatable> updateList = new List<IUpdatable>();

	private static bool quitting = false;

	public static bool IsQuitting
	{
		get { return quitting; }
	}

	private void Awake()
	{
		StateManager.ChangeState(GameState.Editing);
		EventManager.StartListening("ExitPressed", ExitPressedHandler);
		EventManager.StartListening("StateChanged", StateChangedHandler);
	}

	private void StateChangedHandler(Data data)
	{
		if (data.state == GameState.Window)
			System.GC.Collect();
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

	private void ExitPressedHandler(Data data)
	{
		Application.Quit();
	}

	private void OnApplicationQuit()
	{
		quitting = true;
	}
}
