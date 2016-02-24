using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public sealed class BoardEditor : MonoBehaviour, IUpdatable
{
	[SerializeField] private BoardManager boardManager;

	public const float PlaceLimit = 0.03f;
	private float time = 0.0f;

	private Tile activeTile = TileStore.Grass;
	private Tile[,] surroundingTiles = new Tile[3, 3];

	private Vector2i lastFunctionPos;

	private GameObject reticle;
	private Text selectedText;

	private void Awake()
	{
		Engine.StartUpdating(this);

		CreateReticle();

		EventManager.StartListening("StateChanged", StateChangedHandler);
	}

	private void Start()
	{
		selectedText = UIStore.GetGraphic<Text>("SelectedTileText");
		selectedText.text = activeTile.Name;
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
		rend.sprite = Resources.Load<Sprite>("Sprites/Reticle");
	}

	public void UpdateFrame()
	{
		if (StateManager.CurrentState != GameState.Editing || EventSystem.current.IsPointerOverGameObject())
		{
			DisableReticle();
			return;
		}

		DisplayReticle();
		HandleEditInput();
	}

	private void HandleEditInput()
	{
		time += Time.deltaTime;

		if (Input.GetKeyDown(KeyCode.F))
			GetFunction();

		if (Input.GetKeyDown(KeyCode.Q))
			PickTile();

		if (Input.GetMouseButtonDown(0))
			SetSingleTile(GetCursorTilePos(), activeTile);

		if (Input.GetMouseButtonDown(1))
			SetSingleTile(GetCursorTilePos(), TileStore.Air);

		if (Input.GetKey(KeyCode.LeftShift) && Input.GetMouseButton(0))
		{
			if (time >= PlaceLimit)
			{
				SetSingleTile(GetCursorTilePos(), activeTile);
				time -= PlaceLimit;
			}
		}

		if (Input.GetKey(KeyCode.LeftShift) && Input.GetMouseButton(1))
		{
			if (time >= PlaceLimit)
			{
				SetSingleTile(GetCursorTilePos(), TileStore.Air);
				time -= PlaceLimit;
			}
		}
	}

	private void PickTile()
	{
		Vector2i tPos = GetCursorTilePos();

		Tile tile = boardManager.GetTileSafe(1, tPos.x, tPos.y);

		if (tile.ID == 0)
			tile = boardManager.GetTileSafe(0, tPos.x, tPos.y);

		if (tile.ID != 0)
			SetActiveTile(tile);
	}

	public void SetActiveTile(Tile tile)
	{
		activeTile = tile;
		selectedText.text = tile.Name;
	}

	public Vector2i LastFunctionPos()
	{
		return lastFunctionPos;
	}

	private void GetFunction()
	{
		Vector2i tPos = GetCursorTilePos();
		lastFunctionPos = tPos;
		boardManager.GetTileSafe(1, tPos.x, tPos.y).OnFunction();
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

	public Vector2i GetCursorTilePos()
	{
		Vector2i wPos = GetCursorWorldPos();
		return new Vector2i(wPos.x >> Tile.SizeBits, wPos.y >> Tile.SizeBits);
	}

	public Vector2i GetCursorWorldPos()
	{
		Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

		int x = Utils.RoundToNearest(pos.x, Tile.Size);
		int y = Utils.RoundToNearest(pos.y, Tile.Size);

		return new Vector2i(x, y);
	}

	public void SetSingleTile(Vector2i tPos, Tile tile)
	{
		SetTile(tPos, tile);
		boardManager.RebuildChunks();
	}

	public void SetMultipleTiles(List<Vector2i> tPositions, Tile tile)
	{
		for (int i = 0; i < tPositions.Count; i++)
			SetTile(tPositions[i], tile);

		boardManager.RebuildChunks();
	}

	private void SetTile(Vector2i tPos, Tile tile)
	{
		bool deleting = tile.ID == 0;

		if (boardManager.InTileBounds(tPos.x, tPos.y))
		{
			if (IsValidEdit(tPos, tile))
			{
				if (deleting) 
					boardManager.DeleteTile(tPos);
				else 
				{
					BoardData data = boardManager.GetData();
					boardManager.GetTile(tile.PosIndex, tPos.x, tPos.y).OnDeleted(data, tPos);

					if (activeTile.CanAdd(data, tPos))
						boardManager.SetTile(tPos, tile);
				}

				boardManager.FlagChunkForRebuild(tPos);
			}
		}
	}

	private bool IsValidEdit(Vector2i tilePos, Tile tile)
	{
		if (tile.ID == 0) return true;

		if (tile.PosIndex == 1)
		{
			if (boardManager.GetTile(0, tilePos.x, tilePos.y).ID == 0)
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

				surroundingTiles[x, y] = boardManager.GetTileSafe(0, tilePos.x + xOffset, tilePos.y + yOffset);
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
