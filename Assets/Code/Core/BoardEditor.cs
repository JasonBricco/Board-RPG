//
//  BoardEditor.cs
//  BoardRPG
//
//  Created by Jason Bricco on 2/1/16.
//  Copyright © 2016 Jason Bricco. All rights reserved.
//

using UnityEngine;
using UnityEngine.EventSystems;

public sealed class BoardEditor : IUpdatable
{
	private BoardManager boardManager;
	private UIManager UIManager;

	public const float PlaceLimit = 0.03f;
	private float time = 0.0f;

	private Tile activeTile = TileType.Grass;
	private Tile[,] surroundingTiles = new Tile[3, 3];

	public BoardEditor(BoardManager boardManager, UIManager UIManager)
	{
		this.boardManager = boardManager;
		this.UIManager = UIManager;
	}

	public void UpdateTick()
	{
		if (StateManager.CurrentState == GameState.Playing) return;

		HandleModeInput();

		if (EventSystem.current.IsPointerOverGameObject())
			return;

		HandleEditInput();
	}

	private void HandleModeInput()
	{
		if (Input.GetKeyDown(KeyCode.Alpha2))
			UIManager.ToggleGraphic("TilePanel");
	}

	private void HandleEditInput()
	{
		time += Time.deltaTime;

		if (Input.GetMouseButtonDown(0))
			SetTile(false);

		if (Input.GetMouseButtonDown(1))
			SetTile(true);

		if (Input.GetKey(KeyCode.LeftShift) && Input.GetMouseButton(0))
		{
			if (time >= PlaceLimit)
			{
				SetTile(false);
				time -= PlaceLimit;
			}
		}

		if (Input.GetKey(KeyCode.LeftShift) && Input.GetMouseButton(1))
		{
			if (time >= PlaceLimit)
			{
				SetTile(true);
				time -= PlaceLimit;
			}
		}
	}

	public void SetActiveTile(Tile tile)
	{
		activeTile = tile;
	}

	private Vector2i GetCursorTilePos()
	{
		Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		return new Vector2i(pos);
	}

	private void SetTile(bool deleting)
	{
		Vector2i tilePos = GetCursorTilePos();

		if (boardManager.InTileBounds(tilePos.x, tilePos.y))
		{
			if (IsValidEdit(tilePos, deleting ? TileType.Air : activeTile))
			{
				if (deleting) boardManager.DeleteTile(tilePos);
				else boardManager.SetTile(tilePos, activeTile);
			
				boardManager.FlagChunkForRebuild(tilePos);
				boardManager.RebuildChunks();
			}
		}
	}

	private bool IsValidEdit(Vector2i tilePos, Tile tile)
	{
		if (tile.ID == 0) return true;

		if (tile.IsOverlay)
		{
			if (boardManager.GetTile(tilePos.x, tilePos.y).ID == 0)
				return false;
		}

		int xOffset, yOffset;
		surroundingTiles[1, 1] = tile;

		for (int x = 0; x < 3; x++)
		{
			for (int y = 0; y < 3; y++)
			{
				if (x == 1 && y == 1) continue;

				xOffset = x - 1;
				yOffset = y - 1;

				surroundingTiles[x, y] = boardManager.GetTileSafe(tilePos.x + xOffset, tilePos.y + yOffset);
			}
		}

		bool validA = AreaValid(0, 0);
		bool validB = AreaValid(1, 0);
		bool validC = AreaValid(0, 1);
		bool validD = AreaValid(1, 1);

		if (!validA || !validB || !validC || !validD)
			return false;

		return true;
	}

	private bool AreaValid(int startX, int startY)
	{
		int tilesFound = 0;

		for (int x = startX; x <= startX + 1; x++)
		{
			for (int y = startY; y <= startY + 1; y++)
			{
				if (surroundingTiles[x, y].ID != 0)
					tilesFound++;
			}
		}

		return tilesFound != 4;
	}
}
