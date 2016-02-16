//
//  Engine.cs
//  BoardRPG
//
//  Created by Jason Bricco on 2/1/16.
//  Copyright © 2016 Jason Bricco. All rights reserved.
//

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public delegate void Callback();

public sealed class Engine : MonoBehaviour
{
	[SerializeField] private CameraControl camControl;
	[SerializeField] private Material[] materials;
	[SerializeField] private GameObject[] graphics;

	private BoardEditor boardEditor;
	private UIManager UIManager;

	private List<IUpdatable> updateList = new List<IUpdatable>();

	private string eventToCall;

	private static Engine instance;
	public static Engine Instance { get { return instance; } }

	private void Awake()
	{
		instance = this;
		StateManager.ChangeState(GameState.Editing);

		UIManager = new UIManager(graphics);
		BoardManager boardManager = new BoardManager(materials);

		boardEditor = new BoardEditor(boardManager, UIManager);

		updateList.Add(new ErrorHandler());
		updateList.Add(camControl);
		updateList.Add(new GridDrawer());
		updateList.Add(boardEditor);
		updateList.Add(new ChunkRenderer(boardManager));
		updateList.Add(new PlayerManager(boardManager, UIManager));
	}

	private void Update()
	{
		for (int i = 0; i < updateList.Count; i++)
			updateList[i].UpdateTick();
	}

	public void TileButtonPressed(GameObject button)
	{
		boardEditor.SetActiveTile(TileType.GetTileByName(button.name));
		UIManager.DisableGraphic("TilePanel");
	}

	public void ButtonPressedPrimer(string buttonEvent)
	{
		eventToCall = buttonEvent;
	}

	public void ButtonPressed(string data)
	{
		EventManager.TriggerEvent(eventToCall, data);
	}
}
