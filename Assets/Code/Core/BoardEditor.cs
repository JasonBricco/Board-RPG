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

	public BoardEditor(BoardManager boardManager, UIManager UIManager)
	{
		this.boardManager = boardManager;
		this.UIManager = UIManager;
	}

	public void UpdateTick()
	{
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
			if (deleting) boardManager.DeleteTile(tilePos);
			else boardManager.SetTile(tilePos, activeTile);
		
			boardManager.FlagChunkForRebuild(tilePos);
			boardManager.RebuildChunks();
		}
	}
}
