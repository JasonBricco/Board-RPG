//
//  Engine.cs
//  BoardRPG
//
//  Created by Jason Bricco on 2/1/16.
//  Copyright © 2016 Jason Bricco. All rights reserved.
//

using UnityEngine;
using System.Collections.Generic;

public sealed class Engine : MonoBehaviour
{
	[SerializeField] private CameraControl camControl;
	[SerializeField] private Material[] materials;
	[SerializeField] private GameObject[] graphics;

	private BoardEditor boardEditor;

	private List<IUpdatable> updateList = new List<IUpdatable>();

	private void Awake()
	{
		UIManager UIManager = new UIManager(graphics);
		BoardManager boardManager = new BoardManager(materials);

		boardEditor = new BoardEditor(boardManager, UIManager);

		updateList.Add(camControl);
		updateList.Add(new GridDrawer());
		updateList.Add(boardEditor);
		updateList.Add(new ChunkRenderer(boardManager));
	}

	private void Update()
	{
		for (int i = 0; i < updateList.Count; i++)
			updateList[i].UpdateTick();
	}

	public void TileButtonPressed(GameObject button)
	{
		boardEditor.SetActiveTile(TileType.GetTileByName(button.name));
	}
}
