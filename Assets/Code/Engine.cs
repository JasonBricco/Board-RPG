using UnityEngine;
using System.Collections.Generic;

public delegate void Method();

public sealed class Engine : MonoBehaviour
{
	private static List<IUpdatable> updateList = new List<IUpdatable>();

	private static bool quitting = false;

	public static bool IsQuitting
	{
		get { return quitting; }
	}

	private static BoardManager boardManager;
	private static PlayerManager playerManager;
	private static BoardEditor boardEditor;
	private static CardLibrary cardLibrary;
	private static CommandProcessor commandProcessor;

	public static BoardManager BoardManager
	{
		get { return boardManager; }
	}

	public static PlayerManager PlayerManager
	{
		get { return playerManager; }
	}

	public static BoardEditor BoardEditor
	{
		get { return boardEditor; }
	}

	public static CardLibrary CardLibrary
	{
		get { return cardLibrary; }
	}

	public static CommandProcessor CommandProcessor
	{
		get { return commandProcessor; }
	}

	private void Awake()
	{
		boardManager = GetComponent<BoardManager>();
		playerManager = GetComponent<PlayerManager>();
		boardEditor = GetComponent<BoardEditor>();
		cardLibrary = GetComponent<CardLibrary>();
		commandProcessor = GetComponent<CommandProcessor>();
	
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
		quitting = true;
		EventManager.TriggerEvent("Quit");
	}
}
