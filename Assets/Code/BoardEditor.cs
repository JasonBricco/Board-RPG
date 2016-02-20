//
//  BoardEditor.cs
//  BoardRPG
//
//  Created by Jason Bricco on 2/1/16.
//  Copyright © 2016 Jason Bricco. All rights reserved.
//

using UnityEngine;
using UnityEngine.EventSystems;

public sealed class BoardEditor : MonoBehaviour
{
	[SerializeField] private BoardManager boardManager;

	public const float PlaceLimit = 0.03f;
	private float time = 0.0f;

	private Tile activeTile = TileStore.Grass;
	private Tile[,] surroundingTiles = new Tile[3, 3];

	private Vector2i lastFunctionPos;

	private GameObject reticle;

	private void Awake()
	{
		CreateReticle();

		EventManager.StartListening("StateChanged", StateChangedHandler);
		EventManager.StartListening("TileButtonPressed", TileButtonPressed);
	}

	private void StateChangedHandler(int state)
	{
		switch ((GameState)state)
		{
		case GameState.Editing:
			reticle.SetActive(true);
			break;
		}
	}

	private void CreateReticle()
	{
		reticle = new GameObject("Reticle");
		reticle.transform.localScale = new Vector3(32.0f, 32.0f);
		SpriteRenderer rend = reticle.AddComponent<SpriteRenderer>();
		rend.sprite = Resources.Load<Sprite>("Textures/Reticle");
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
			UIManager.DisableWindow("TilePanel", GameState.Editing);
		
		if (StateManager.CurrentState != GameState.Editing || EventSystem.current.IsPointerOverGameObject())
		{
			DisableReticle();
			return;
		}

		HandleModeInput();
		DisplayReticle();
		HandleEditInput();
	}

	private void TileButtonPressed(int data)
	{
		SetActiveTile(TileStore.GetTileByID(data));
		UIManager.DisableWindow("TilePanel", GameState.Editing);
	}

	private void HandleModeInput()
	{
		if (Input.GetKeyDown(KeyCode.Alpha2))
			UIManager.ToggleWindow("TilePanel", GameState.Editing);
	}

	private void HandleEditInput()
	{
		time += Time.deltaTime;

		if (Input.GetKeyDown(KeyCode.F))
			GetFunction();

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

	public Vector2i LastFunctionPos()
	{
		return lastFunctionPos;
	}

	private void GetFunction()
	{
		Vector2i tPos = GetCursorTilePos();
		lastFunctionPos = tPos;
		boardManager.GetOverlayTileSafe(tPos.x, tPos.y).OnFunction();
	}

	private void DisplayReticle()
	{
		EnableReticle();
		Vector2i tilePos = GetCursorWorldPos();
		reticle.transform.position = new Vector3(tilePos.x, tilePos.y);
	}

	private void EnableReticle()
	{
		if (!reticle.activeSelf) reticle.SetActive(true);
	}

	private void DisableReticle()
	{
		if (reticle.activeSelf) reticle.SetActive(false);
	}

	private Vector2i GetCursorTilePos()
	{
		Vector2i wPos = GetCursorWorldPos();
		return new Vector2i(wPos.x >> Tile.SizeBits, wPos.y >> Tile.SizeBits);
	}

	private Vector2i GetCursorWorldPos()
	{
		Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

		int x = Utils.RoundToNearest(pos.x, Tile.Size);
		int y = Utils.RoundToNearest(pos.y, Tile.Size);

		return new Vector2i(x, y);
	}

	private void SetTile(bool deleting)
	{
		Vector2i tilePos = GetCursorTilePos();

		if (boardManager.InTileBounds(tilePos.x, tilePos.y))
		{
			if (IsValidEdit(tilePos, deleting ? TileStore.Air : activeTile))
			{
				if (deleting) boardManager.DeleteTile(tilePos);
				else 
				{
					if (activeTile.CanAdd(boardManager.GetData(), tilePos))
						boardManager.SetTile(tilePos, activeTile);
				}
			
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
