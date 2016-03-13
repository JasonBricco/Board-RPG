using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public sealed class MapEditor : MonoBehaviour, IUpdatable
{
	private float startTime = 0.0f;
	private float exitDelay = 0.0f;

	private Tile activeTile = Tiles.Grass;

	private GameObject mainButtons;
	private GameObject reticle;
	private RectTransform dragRect;
	private Text selectedText;

	private EditMode editMode = EditMode.Normal;

	private List<Vector2i> massEditList = new List<Vector2i>(16);

	private Vector2 initialClickPos = Vector3.zero;
	private bool initialIsLeft;

	private void Awake()
	{
		Engine.StartUpdating(this);
		CreateReticle();
	}

	private void Start()
	{
		mainButtons = SceneItems.GetItem("MainButtons");
		ShowMainButtons();

		EventManager.StartListening("StateChanged", StateChangedHandler);

		selectedText = SceneItems.GetItem<Text>("SelectedTileText");
		selectedText.text = Map.GetTileType(activeTile).Name;

		dragRect = SceneItems.GetItem<RectTransform>("DragRect");
	}

	private void StateChangedHandler(Data data)
	{
		switch (data.state)
		{
		case GameState.Editing:
			ShowMainButtons();
			reticle.SetActive(true);
			if (selectedText != null) 
				selectedText.enabled = true;
			break;

		case GameState.SelectingCoords:
			mainButtons.SetActive(false);
			selectedText.enabled = false;
			break;

		default:
			mainButtons.SetActive(false);
			startTime = Time.time;
			exitDelay = startTime + 2.0f;
			selectedText.enabled = false;
			break;
		}
	}

	private void ShowMainButtons()
	{
		if (!mainButtons.activeSelf)
			mainButtons.SetActive(true);
	}

	public void UpdateFrame()
	{
		startTime += Time.deltaTime;

		if (startTime > exitDelay)
		{
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				if (StateManager.CurrentState == GameState.Playing)
				{
					StateManager.ChangeState(GameState.Editing);
					Map.Clear(null);
					Serializer.Load();
				}
			}
		}

		bool overUI = EventSystem.current.IsPointerOverGameObject();

		ProcessReticle(overUI);

		if (StateManager.CurrentState == GameState.Editing)
			HandleEditInput(overUI);
	}

	public void SetEditMode(int mode)
	{
		editMode = (EditMode)mode;
	}

	private void HandleEditInput(bool overUI)
	{
		if (editMode == EditMode.SquareFill)
			FillSquare(overUI);

		if (overUI) return;

		if (Input.GetKeyDown(KeyCode.F))
			GetFunction();

		if (Input.GetKeyDown(KeyCode.Q))
			PickTile();

		switch (editMode)
		{
		case EditMode.Normal:
			NormalEdit();
			break;

		case EditMode.MassEdit:
			MassEdit();
			break;

		case EditMode.AreaFill:
			FillArea();
			break;
		}
	}

	private void NormalEdit()
	{
		if (Input.GetMouseButtonDown(0) || (Input.GetKey(KeyCode.LeftShift) && Input.GetMouseButton(0)))
			Map.SetTile(Utils.GetCursorTilePos(), activeTile);

		if (Input.GetMouseButtonDown(1) || (Input.GetKey(KeyCode.LeftShift) && Input.GetMouseButton(1)))
			Map.SetTile(Utils.GetCursorTilePos(), Tiles.Air);
	}

	private void MassEdit()
	{
		Tile? tile = null;

		if (Input.GetMouseButton(0)) tile = activeTile;
		if (Input.GetMouseButton(1)) tile = Tiles.Air;

		if (tile == null) return;

		Vector2i cursorPos = Utils.GetCursorTilePos();

		for (int x = cursorPos.x - 1; x <= cursorPos.x + 1; x++)
		{
			for (int y = cursorPos.y - 1; y <= cursorPos.y + 1; y++)
				massEditList.Add(new Vector2i(x, y));
		}

		Map.SetMultipleTiles(massEditList, tile.Value);
		massEditList.Clear();
	}

	private void FillSquare(bool overUI)
	{
		DisableReticle();

		GameObject dragObj = dragRect.gameObject;

		if (overUI)
		{
			if (dragObj.activeSelf) dragObj.SetActive(false);
			return;
		}

		bool leftDown = Input.GetMouseButtonDown(0);
		bool rightDown = Input.GetMouseButtonDown(1);

		if (leftDown || rightDown)
		{
			initialIsLeft = leftDown;
			Vector2i wPos = Utils.GetCursorWorldPos();
			initialClickPos = new Vector2(wPos.x - Tile.HalfSize, wPos.y + Tile.HalfSize);
			dragRect.anchoredPosition = initialClickPos;
			dragRect.gameObject.SetActive(true);
		}

		if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
		{
			if (!dragObj.activeSelf) dragObj.SetActive(true);

			Vector2i wPos = Utils.GetCursorWorldPos();
			Vector2 currentMouse = new Vector2(wPos.x - Tile.HalfSize, wPos.y + Tile.HalfSize);

			Vector2 difference = currentMouse - initialClickPos;
			Vector2 startPoint = initialClickPos;

			if (difference.x < 0)
			{
				startPoint.x = currentMouse.x;
				difference.x = -difference.x;
			}

			if (difference.y < 0)
			{
				startPoint.y = currentMouse.y;
				difference.y = -difference.y;
			}

			dragRect.anchoredPosition = startPoint;

			difference.x = Utils.RoundToNearest(difference.x, Tile.Size);
			difference.y = Utils.RoundToNearest(difference.y, Tile.Size);

			dragRect.sizeDelta = difference;
		}

		if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))
		{
			dragRect.gameObject.SetActive(false);
			initialClickPos = Vector2.zero;

			Vector2i tPos = Utils.TileFromWorldPos(dragRect.position);
			tPos.x += 1;

			List<Vector2i> tiles = new List<Vector2i>();

			int offsetX = (int)dragRect.sizeDelta.x >> Tile.SizeBits;
			int offsetY = (int)dragRect.sizeDelta.y >> Tile.SizeBits;

			for (int x = tPos.x; x < tPos.x + offsetX; x++)
			{
				for (int y = tPos.y + offsetY; y > tPos.y; y--)
					tiles.Add(new Vector2i(x, y));
			}
	
			Map.SetMultipleTiles(tiles, initialIsLeft ? activeTile : Tiles.Air);
		}
	}

	private void FillArea()
	{
		if (Input.GetMouseButtonDown(0))
		{
			List<Vector2i> tiles = new List<Vector2i>();
			HashSet<Vector2i> checkSet = new HashSet<Vector2i>();

			Vector2i first = Utils.GetCursorTilePos();

			if (!Map.InTileBounds(first.x, first.y)) return;

			tiles.Add(first);
			checkSet.Add(first);

			for (int i = 0; i < tiles.Count; i++)
			{
				Vector2i current = tiles[i];

				for (int j = 0; j < 4; j++)
				{
					Vector2i next = current + Vector2i.directions[j];

					if (!Map.InTileBounds(next.x, next.y) || checkSet.Contains(next))
						continue;
					
					if (Map.GetTile(0, next.x, next.y).Equals(Tiles.Air) && Map.GetTile(1, next.x, next.y).Equals(Tiles.Air))
					{
						tiles.Add(next);
						checkSet.Add(next);
					}
				}
			}
	
			Map.SetMultipleTiles(tiles, activeTile);
		}

		if (Input.GetMouseButtonDown(1) || (Input.GetKey(KeyCode.LeftShift) && Input.GetMouseButton(1)))
			Map.SetTile(Utils.GetCursorTilePos(), Tiles.Air);
	}

	public void SetActiveTile(ushort ID)
	{
		activeTile = new Tile(ID);
		selectedText.text = Map.GetTileType(activeTile).Name;
	}

	private void GetFunction()
	{
		Vector2i tPos = Utils.GetCursorTilePos();

		if (Map.InTileBounds(tPos.x, tPos.y))
		{
			Map.GetTileType(0, tPos.x, tPos.y).OnFunction(tPos);
			Map.GetTileType(1, tPos.x, tPos.y).OnFunction(tPos);
		}
	}

	private void PickTile()
	{
		Vector2i tPos = Utils.GetCursorTilePos();

		Tile tile = Map.GetTileSafe(1, tPos.x, tPos.y);

		if (tile.ID == 0)
			tile = Map.GetTileSafe(0, tPos.x, tPos.y);

		if (tile.ID != 0)
			SetActiveTile(tile.ID);
	}

	private void CreateReticle()
	{
		reticle = new GameObject("Reticle");
		reticle.transform.localScale = new Vector3(32.0f, 32.0f);
		SpriteRenderer rend = reticle.AddComponent<SpriteRenderer>();
		rend.sprite = Resources.Load<Sprite>("General/Reticle");
	}

	private void ProcessReticle(bool overUI)
	{
		if (StateManager.CurrentState == GameState.Editing || StateManager.CurrentState == GameState.SelectingCoords)
		{
			if (overUI)
			{
				DisableReticle();
				return;
			}

			EnableReticle();
			Vector2i tilePos = Utils.GetCursorWorldPos();
			reticle.transform.position = new Vector3(tilePos.x, tilePos.y);
		}
		else
			DisableReticle();
	}

	private void EnableReticle()
	{
		if (!reticle.activeSelf) reticle.SetActive(true);
	}

	private void DisableReticle()
	{
		if (reticle.activeSelf) reticle.SetActive(false);
	}
}
