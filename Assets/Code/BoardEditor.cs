using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public sealed class BoardEditor : MonoBehaviour, IUpdatable
{
	[SerializeField] private BoardManager boardManager;

	public const float PlaceLimit = 0.03f;
	private float time = 0.0f;

	private Tile activeTile = Tiles.Grass;

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
		selectedText.text = activeTile.Type.Name;
	}

	private void StateChangedHandler(int state)
	{
		switch ((GameState)state)
		{
		case GameState.Editing:
			reticle.SetActive(true);
			if (selectedText != null) 
				selectedText.enabled = true;
			break;

		case GameState.Playing:
			selectedText.enabled = false;
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
			SetSingleTile(GetCursorTilePos(), Tiles.Air);

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
				SetSingleTile(GetCursorTilePos(), Tiles.Air);
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
			SetActiveTile(tile.ID);
	}

	public void SetActiveTile(ushort ID)
	{
		activeTile = new Tile(ID);
		selectedText.text = activeTile.Type.Name;
	}

	private void GetFunction()
	{
		Vector2i tPos = GetCursorTilePos();
		boardManager.GetTileSafe(1, tPos.x, tPos.y).Type.OnFunction(tPos);
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
		return new Vector2i(wPos.x >> TileType.SizeBits, wPos.y >> TileType.SizeBits);
	}

	public Vector2i GetCursorWorldPos()
	{
		Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

		int x = Utils.RoundToNearest(pos.x, TileType.Size);
		int y = Utils.RoundToNearest(pos.y, TileType.Size);

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
					boardManager.GetTile(tile.Type.Layer, tPos.x, tPos.y).Type.OnDeleted(tPos);

					if (activeTile.Type.CanAdd(tPos))
						boardManager.SetTile(tPos, tile);
				}

				boardManager.FlagChunkForRebuild(tPos);
			}
		}
	}

	private bool IsValidEdit(Vector2i tilePos, Tile tile)
	{
		if (tile.ID == 0) return true;

		if (tile.Type.Layer == 1)
		{
			if (boardManager.GetTile(0, tilePos.x, tilePos.y).ID == 0)
				return false;
		}

		return true;
	}
}
