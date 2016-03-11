﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public sealed class MapEditor : MonoBehaviour, IUpdatable 
{
	public const float PlaceLimit = 0.03f;
	private float time = 0.0f;

	private float startTime = 0.0f;
	private float exitDelay = 0.0f;

	private Tile activeTile = Tiles.Grass;

	private GameObject mainButtons;
	private GameObject reticle;
	private Text selectedText;

	private EditMode editMode = EditMode.Normal;

	private void Awake()
	{
		Engine.StartUpdating(this);
		CreateReticle();
	}

	private void Start()
	{
		mainButtons = UIStore.GetGraphic("MainButtons");
		ShowMainButtons();

		EventManager.StartListening("StateChanged", StateChangedHandler);

		selectedText = UIStore.GetGraphic<Text>("SelectedTileText");
		selectedText.text = Map.GetTileType(activeTile).Name;
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

		if (!overUI && StateManager.CurrentState == GameState.Editing)
			HandleEditInput();
	}

	private void HandleEditInput()
	{
		time += Time.deltaTime;

		if (Input.GetKeyDown(KeyCode.F))
			GetFunction();

		if (Input.GetKeyDown(KeyCode.Q))
			PickTile();
			
		if (Input.GetMouseButton(0))
		{
			if (time >= PlaceLimit)
			{
				Map.SetTile(Utils.GetCursorTilePos(), activeTile);
				time -= PlaceLimit;
			}
		}

		if (Input.GetMouseButton(1))
		{
			if (time >= PlaceLimit)
			{
				Map.SetTile(Utils.GetCursorTilePos(), Tiles.Air);
				time -= PlaceLimit;
			}
		}

		switch (editMode)
		{
		case EditMode.Normal:
			break;

		case EditMode.MassDelete:
			break;

		case EditMode.SquareFill:
			break;

		case EditMode.AreaFill:
			break;
		}
	}

	public void SetActiveTile(ushort ID)
	{
		activeTile = new Tile(ID);
		selectedText.text = Map.GetTileType(activeTile).Name;
	}

	private void GetFunction()
	{
		Vector2i tPos = Utils.GetCursorTilePos();
		Map.GetTileTypeSafe(1, tPos.x, tPos.y).OnFunction(tPos);
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
