//
//  GridDrawer.cs
//  BoardRPG
//
//  Created by Jason Bricco on 2/1/16.
//  Copyright © 2016 Jason Bricco. All rights reserved.
//

using UnityEngine;
using System.Collections.Generic;

public sealed class GridDrawer : IUpdatable
{
	private VectorLine grid;
	private List<Vector3> gridPoints = new List<Vector3>();

	private int lastWidth, lastHeight;

	public GridDrawer()
	{
		EventManager.StartListening("StateChanged", StateChangedHandler);

		lastWidth = Screen.width;
		lastHeight = Screen.height;

		grid = new VectorLine("Grid", gridPoints, 1.0f);
		CreateGrid();
	}

	private void StateChangedHandler(object data)
	{
		GameState state = (GameState)data;

		switch (state)
		{
		case GameState.Editing:
			grid.Active = true;
			break;

		case GameState.Playing:
			grid.Active = false;
			break;
		}
	}

	public void UpdateTick()
	{
		if (Screen.width != lastWidth)
		{
			int newHeight = (Screen.width >> 2) * 3;
			Screen.SetResolution(Screen.width, newHeight, false);
			grid.Draw();

			lastWidth = Screen.width;
			lastHeight = Screen.height;
		}
		else if (Screen.height != lastHeight)
		{
			int newWidth = (Screen.height / 3) * 4;
			Screen.SetResolution(newWidth, Screen.height, false);
			grid.Draw();

			lastWidth = Screen.width;
			lastHeight = Screen.height;
		}
	}
		
	private void CreateGrid()
	{
		float width = BoardManager.Size - 0.5f;
		float height = BoardManager.Size - 0.5f;

		for (float x = -0.5f; x <= width; x++)
		{
			gridPoints.Add(new Vector3(x, -0.5f));
			gridPoints.Add(new Vector3(x, height));
		}

		for (float y = -0.5f; y <= height; y++)
		{
			gridPoints.Add(new Vector3(-0.5f, y));
			gridPoints.Add(new Vector3(width, y));
		}

		grid.Draw();
	}
}
