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

public sealed class Engine : MonoBehaviour
{
	[SerializeField] private CameraControl camControl;
	[SerializeField] private Material[] materials;
	[SerializeField] private GameObject[] graphics;

	[SerializeField] private GameObject player;
	[SerializeField] private GameObject enemy;

	private BoardEditor boardEditor;
	private UIManager UIManager;

	private List<IUpdatable> updateList = new List<IUpdatable>();

	private void Awake()
	{
		UIManager = new UIManager(graphics);
		BoardManager boardManager = new BoardManager(materials);

		boardEditor = new BoardEditor(boardManager, UIManager);

		updateList.Add(camControl);
		updateList.Add(new GridDrawer());
		updateList.Add(boardEditor);
		updateList.Add(new ChunkRenderer(boardManager));
		updateList.Add(new PlayerManager(player, enemy, boardManager));
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

	public void ButtonPressed(string buttonEvent)
	{
		EventManager.TriggerEvent(buttonEvent, null);
	}
}
