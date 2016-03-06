using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public sealed class Map : MonoBehaviour, IUpdatable
{
	public const int Size = 256;
	public const int WidthInChunks = Size / Chunk.Size;
	private const int WToXShift = Chunk.SizeBits + TileType.SizeBits;

	private static int maxMeshes;

	[SerializeField] private Material[] materials;
	[SerializeField] private Sprite[] cardSprites;

	private CommandProcessor processor;

	private List<TileType> tilesList = new List<TileType>();
	private Dictionary<string, TileType> tilesByName = new Dictionary<string, TileType>();

	private Card[] cards = new Card[7];
	private List<Card> allowedCards = new List<Card>();

	private Chunk[,] chunks = new Chunk[WidthInChunks, WidthInChunks];

	private List<Chunk> chunksToRebuild = new List<Chunk>();

	public const float PlaceLimit = 0.03f;
	private float time = 0.0f;

	private Tile activeTile = Tiles.Grass;

	private GameObject mainButtons;
	private GameObject reticle;
	private GameObject turnDisplayPanel;
	private Text selectedText;
	private Text turnText;
	private Image currentCardImage;

	private Sprite playerSprite, enemySprite;

	private Entity currentEntity;

	private List<PendingFunction> pendingFunctions = new List<PendingFunction>();

	private List<Entity> entityList = new List<Entity>();

	private int lastTurn = -1;

	private float startTime = 0.0f;
	private float exitDelay = 0.0f;

	private List<Vector2i> startPositions = new List<Vector2i>();

	private Dictionary<string, Function> functions = new Dictionary<string, Function>();

	public static int MaxMeshes
	{
		get { return maxMeshes; }
	}

	public Entity CurrentEntity 
	{
		get { return currentEntity; }
	}

	private void Awake()
	{
		maxMeshes = materials.Length;

		Engine.StartUpdating(this);

		Serializer.ListenForSave(SaveTiles);
		Serializer.ListenForLoad(LoadTiles);

		processor = GetComponent<CommandProcessor>();

		CreateReticle();
		CreateTiles();
		CreateCards();
		CreateFunctions();
	}

	private void Start()
	{
		mainButtons = UIStore.GetGraphic("MainButtons");
		ShowMainButtons();

		EventManager.StartListening("ClearPressed", ClearBoard);
		EventManager.StartListening("StateChanged", StateChangedHandler);
		EventManager.StartListening("PlayPressed", PlayPressedHandler);

		selectedText = UIStore.GetGraphic<Text>("SelectedTileText");
		selectedText.text = GetTileType(activeTile).Name;

		turnDisplayPanel = UIStore.GetGraphic("TurnDisplayPanel");
		turnText = UIStore.GetGraphic<Text>("TurnText");
		currentCardImage = UIStore.GetGraphic<Image>("Card");

		playerSprite = Resources.Load<Sprite>("Sprites/Player");
		enemySprite = Resources.Load<Sprite>("Sprites/Enemy");
	}

	private void StateChangedHandler(int state)
	{
		switch ((GameState)state)
		{
		case GameState.Editing:
			ShowMainButtons();
			reticle.SetActive(true);
			if (selectedText != null) 
				selectedText.enabled = true;
			pendingFunctions.Clear();
			break;

		default:
			mainButtons.SetActive(false);
			selectedText.enabled = false;
			break;
		}
	}

	public void UpdateFrame()
	{
		RenderChunks();

		startTime += Time.deltaTime;

		if (startTime > exitDelay)
		{
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				if (StateManager.CurrentState == GameState.Playing)
				{
					for (int i = 0; i < entityList.Count; i++)
						entityList[i].Delete();

					entityList.Clear();
					StateManager.ChangeState(GameState.Editing);
					ClearBoard(0);
					Serializer.Load();
				}
			}
		}

		if (StateManager.CurrentState != GameState.Editing || EventSystem.current.IsPointerOverGameObject())
		{
			DisableReticle();
			return;
		}

		DisplayReticle();
		HandleEditInput();
	}

	private void RenderChunks()
	{
		Vector3 middle = new Vector3(Screen.width >> 1, Screen.height >> 1);
		Vector3 point = Camera.main.ScreenToWorldPoint(middle);

		int cX = (int)point.x >> WToXShift, cY = (int)point.y >> WToXShift;

		for (int x = cX - 1; x <= cX + 1; x++)
		{
			for (int y = cY - 1; y <= cY + 1; y++)
			{
				if (InChunkBounds(x, y))
				{
					Chunk chunkToRender = GetChunkDirect(x, y);

					if (chunkToRender != null)
						chunkToRender.Render();
				}
			}
		}
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

	private void CreateReticle()
	{
		reticle = new GameObject("Reticle");
		reticle.transform.localScale = new Vector3(32.0f, 32.0f);
		SpriteRenderer rend = reticle.AddComponent<SpriteRenderer>();
		rend.sprite = Resources.Load<Sprite>("Sprites/Reticle");
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

	private void PickTile()
	{
		Vector2i tPos = GetCursorTilePos();

		Tile tile = GetTileSafe(1, tPos.x, tPos.y);

		if (tile.ID == 0)
			tile = GetTileSafe(0, tPos.x, tPos.y);

		if (tile.ID != 0)
			SetActiveTile(tile.ID);
	}

	public void SetActiveTile(ushort ID)
	{
		activeTile = new Tile(ID);
		selectedText.text = GetTileType(activeTile).Name;
	}

	private void ShowMainButtons()
	{
		if (!mainButtons.activeSelf)
			mainButtons.SetActive(true);
	}

	public Material GetMaterial(int index)
	{
		return materials[index];
	}

	private void GetFunction()
	{
		Vector2i tPos = GetCursorTilePos();
		GetTileTypeSafe(1, tPos.x, tPos.y).OnFunction(tPos);
	}

	public void SetStartPositions()
	{
		startPositions.Clear();

		for (int x = 0; x < chunks.GetLength(0); x++)
		{
			for (int y = 0; y < chunks.GetLength(0); y++)
			{
				Chunk chunk = chunks[x, y];

				if (chunk != null)
				{
					for (int cX = 0; cX < Chunk.Size; cX++)
					{
						for (int cY = 0; cY < Chunk.Size; cY++)
						{
							Tile tile = chunk.GetTile(1, cX, cY);

							if (tile.Equals(Tiles.Start))
								startPositions.Add(new Vector2i((x * Chunk.Size) + cX, (y * Chunk.Size) + cY));
						}
					}
				}
			}
		}
	}

	private void CreateDefaultStartTile()
	{
		Vector2i midPos = new Vector2i(Size / 2, Size / 2);
		Tile midTile = GetTile(0, midPos.x, midPos.y);

		if (midTile.Equals(Tiles.Air))
			SetTileFast(midPos, Tiles.Grass);

		SetTileFast(midPos, Tiles.Start);
		startPositions.Add(midPos);

		FlagChunkForRebuild(midPos);
		RebuildChunks();
	}

	public void RemoveStartTile(Vector2i pos)
	{
		startPositions.Remove(pos);
	}

	private Entity CreateEntity(string name, System.Type type, Sprite sprite, int ID)
	{
		GameObject entityObj = new GameObject(name);
		entityObj.transform.localScale = new Vector3(32.0f, 32.0f);
		SpriteRenderer rend = entityObj.AddComponent<SpriteRenderer>();
		entityObj.AddComponent(type);
		rend.sprite = sprite;

		Entity entity = entityObj.GetComponent<Entity>();
		entity.SetReferences(ID, this);
		return entity;
	}

	public Entity GetEntity(int entityID)
	{
		if (entityID < 0 || entityID >= entityList.Count)
			return null;

		return entityList[entityID];
	}

	public void ValidateEntities()
	{
		for (int i = 0; i < entityList.Count; i++)
		{
			Entity entity = entityList[i];
			Vector2i tPos = Utils.TileFromWorldPos(entity.Position);

			if (GetTile(0, tPos.x, tPos.y).ID == 0)
				SpawnEntity(entityList[i]);
		}
	}

	public void SpawnEntity(Entity entity)
	{
		if (startPositions.Count == 0)
			CreateDefaultStartTile();
		
		entity.SetTo(startPositions[Random.Range(0, startPositions.Count)]);
	}

	private void PlayPressedHandler(int data)
	{
		Serializer.Save();

		startTime = Time.time;
		exitDelay = startTime + 2.0f;

		SetStartPositions();

		if (startPositions.Count == 0)
			CreateDefaultStartTile();

		entityList.Add(CreateEntity("Player", typeof(Player), playerSprite, 0));
		entityList.Add(CreateEntity("Enemy", typeof(Enemy), enemySprite, 1));

		for (int i = 0; i < entityList.Count; i++)
			SpawnEntity(entityList[i]);

		int turnIndex = Random.Range(0, entityList.Count);
		currentEntity = entityList[turnIndex];

		StateManager.ChangeState(GameState.Playing);
		NextTurn(turnIndex);
	}

	public void WaitForTurns(int entityID, int turns, Data data, UnityAction<Data> function)
	{
		pendingFunctions.Add(new PendingFunction(entityID, turns, data, function));
	}

	private void ProcessPendingList(int entityID)
	{
		for (int i = pendingFunctions.Count - 1; i >= 0; i--)
		{
			PendingFunction pF = pendingFunctions[i];

			if (pF.entityID == entityID)
			{
				pF.turnsRemaining--;

				if (pF.turnsRemaining == 0)
				{
					pF.function(pF.data);
					pendingFunctions.RemoveAt(i);
				}
			}
		}
	}

	public void NextTurn(int forcedTurn = -1)
	{
		if (lastTurn != -1) ProcessPendingList(entityList[lastTurn].EntityID);

		int turnIndex = forcedTurn == -1 ? (lastTurn + 1) % entityList.Count : forcedTurn;
		lastTurn = turnIndex;

		currentEntity = entityList[turnIndex];
		StartCoroutine(CallTurn(currentEntity));
	}

	private IEnumerator CallTurn(Entity entity)
	{
		turnText.text = entity.name + "'s Turn";

		turnDisplayPanel.SetActive(true);
		yield return new WaitForSeconds(1.5f);
		turnDisplayPanel.SetActive(false);

		entity.BeginTurn();
	}

	public void SetSingleTile(Vector2i tPos, Tile tile)
	{
		SetTileAdvanced(tPos, tile);
		RebuildChunks();
	}

	public void SetMultipleTiles(List<Vector2i> tPositions, Tile tile)
	{
		for (int i = 0; i < tPositions.Count; i++)
			SetTileAdvanced(tPositions[i], tile);

		RebuildChunks();
	}

	private void SetTileAdvanced(Vector2i tPos, Tile tile)
	{
		bool deleting = tile.ID == 0;

		if (InTileBounds(tPos.x, tPos.y))
		{
			if (deleting) 
			{
				DeleteTile(tPos);
				FlagChunkForRebuild(tPos);
			}
			else 
			{
				TileType type = GetTileType(tile);
				GetTileType(type.Layer, tPos.x, tPos.y).OnDeleted(tPos);

				if (type.CanAdd(tPos))
				{
					SetTileFast(tPos, tile);
					FlagChunkForRebuild(tPos);
				}
			}
		}
	}

	public Tile GetTileSafe(int layer, int tX, int tY)
	{
		if (!InTileBounds(tX, tY)) return Tiles.Air;
		return GetTile(layer, tX, tY);
	}

	public Tile GetTile(int layer, int tX, int tY)
	{
		Chunk chunk = GetChunk(tX, tY);
		return chunk == null ? Tiles.Air : chunk.GetTile(layer, tX & Chunk.Size - 1, tY & Chunk.Size - 1);
	}

	public void SetTileFast(Vector2i tPos, Tile tile)
	{
		tile = GetTileType(tile).Preprocess(tile, tPos);
		GetChunkSafe(tPos.x, tPos.y).SetTile(tPos.x & Chunk.Size - 1, tPos.y & Chunk.Size - 1, tile);
	}

	public void DeleteTile(Vector2i tPos)
	{
		int localX = tPos.x & Chunk.Size - 1, localY = tPos.y & Chunk.Size - 1;
		GetChunkSafe(tPos.x, tPos.y).DeleteTile(localX, localY, tPos.x, tPos.y);
	}

	private Chunk GetChunk(int tX, int tY)
	{
		return chunks[tX >> Chunk.SizeBits, tY >> Chunk.SizeBits];
	}

	private Chunk GetChunkSafe(int tX, int tY)
	{
		Vector2i pos = new Vector2i(tX >> Chunk.SizeBits, tY >> Chunk.SizeBits);
		Chunk chunk = chunks[pos.x, pos.y];

		if (chunk == null)
		{
			chunk = new Chunk(pos.x, pos.y, this);
			chunks[pos.x, pos.y] = chunk;
		}

		return chunk;
	}

	public Chunk GetChunkDirect(int chunkX, int chunkY)
	{
		return chunks[chunkX, chunkY];
	}

	public void FlagChunkForRebuild(Vector2i tPos)
	{
		Chunk chunk = GetChunk(tPos.x, tPos.y);

		if (!chunk.FlaggedForUpdate)
		{
			chunk.FlaggedForUpdate = true;
			chunksToRebuild.Add(chunk);
		}
	}

	public void RebuildChunks()
	{
		for (int i = 0; i < chunksToRebuild.Count; i++)
			chunksToRebuild[i].BuildMesh();

		chunksToRebuild.Clear();
	}

	public bool IsPassable(int tX, int tY)
	{
		Tile mainTile = GetTileSafe(0, tX, tY);
		Tile overlayTile = GetTileSafe(1, tX, tY);

		return GetTileType(mainTile).IsPassable(0, mainTile, overlayTile);
	}

	public bool InTileBounds(int tX, int tY)
	{
		return tX >= 0 && tY >= 0 && tX < Size && tY < Size;
	}

	public bool InChunkBounds(int cX, int cY)
	{
		return cX >= 0 && cY >= 0 && cX < WidthInChunks && cY < WidthInChunks; 
	}

	private void ClearBoard(int data)
	{
		for (int x = 0; x < chunks.GetLength(0); x++)
		{
			for (int y = 0; y < chunks.GetLength(0); y++)
			{
				Chunk chunk = chunks[x, y];

				if (chunk != null)
					chunk.Destroy();

				chunks[x, y] = null;
			}
		}

		processor.ClearAll();
	}

	private void SaveTiles(MapData data)
	{
		for (int x = 0; x < chunks.GetLength(0); x++)
		{
			for (int y = 0; y < chunks.GetLength(1); y++)
			{
				Chunk chunk = chunks[x, y];

				if (chunk != null)
					chunk.Save(data);
			}
		}
	}

	private void LoadTiles(MapData data)
	{
		for (int i = 0; i < data.savedChunks.Count; i++)
		{
			int pos = data.savedChunks[i];
			int cX = pos & (Map.WidthInChunks - 1);
			int cY = (pos >> 4) & (Map.WidthInChunks - 1);

			Chunk chunk = new Chunk(cX, cY, this);
			chunks[cX, cY] = chunk;

			chunk.Load(data.chunkData[i]);
		}
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

	public Vector2i GetLineEnd(Vector2i start, Vector2i dir)
	{
		Vector2i current = start;
		int distance = 0;

		for (int i = 0; i < Size; i++)
		{
			Vector2i next = current + dir;
			Tile nextTile = GetTileSafe(0, next.x, next.y);
			Tile nextOverlay = GetTileSafe(1, next.x, next.y);

			if (nextTile.Equals(Tiles.Stopper) || nextOverlay.Equals(Tiles.Arrow) || nextOverlay.Equals(Tiles.RandomArrow))
				return next;

			if (!IsPassable(next.x, next.y))
				return current;

			current = next;
			distance++;
		}

		return start;
	}

	public TileType GetTileType(string name)
	{
		TileType tile;
		bool success = tilesByName.TryGetValue(name, out tile);
		return success ? tile : null;
	}

	public TileType GetTileType(Tile tile)
	{
		return tilesList[tile.ID];
	}

	public TileType GetTileType(int layer, int x, int y)
	{
		return tilesList[GetTile(layer, x, y).ID];
	}

	public TileType GetTileTypeSafe(int layer, int x, int y)
	{
		return tilesList[GetTileSafe(layer, x, y).ID];
	}

	public Function GetFunction(string name)
	{
		return functions[name];
	}

	public bool TryGetFunction(string name, out Function function)
	{
		return functions.TryGetValue(name, out function);
	}

	public void ToggleCard(int cardID)
	{
		Card card = cards[cardID];
		card.allowed = !card.allowed;
	}

	public void ReplaceAllowedList()
	{
		allowedCards.Clear();

		for (int i = 0; i < cards.Length; i++)
		{
			Card card = cards[i];

			if (card.allowed)
				allowedCards.Add(card);
		}
	}

	public void DrawCard(Entity entity)
	{
		if (allowedCards.Count == 0) return;

		entity.wait = true;

		Card card = allowedCards[Random.Range(0, allowedCards.Count)];
		currentCardImage.sprite = card.sprite;
		currentCardImage.enabled = true;

		StartCoroutine(PlayCard(card, entity));
	}

	private IEnumerator PlayCard(Card card, Entity entity)
	{
		yield return new WaitForSeconds(2.0f);

		currentCardImage.enabled = false;
		entity.wait = false;

		if (StateManager.CurrentState == GameState.Playing)
		{
			if (entity != null)
				card.RunFunction(entity);
		}
	}

	private void CreateTiles()
	{
		tilesList.Add(new AirTile(0, this));
		tilesList.Add(new BasicTile(1, 0, 0, "Grass", this));
		tilesList.Add(new StartTile(2, 1, this));
		tilesList.Add(new BasicTile(3, 2, 0, "Stone", this));
		tilesList.Add(new BasicTile(4, 3, 0, "Sand", this));
		tilesList.Add(new TriggerTile(5, 4, this, processor));
		tilesList.Add(new LandTriggerTile(6, 5, this, processor));
		tilesList.Add(new CardTile(7, 6, this));
		tilesList.Add(new ArrowTile(8, 7, this));
		tilesList.Add(new BasicTile(9, 8, 0, "Stopper", this));
		tilesList.Add(new RandomArrowTile(10, 9, this));
		tilesList.Add(new WaterTile(11, 10, 11, this));
		tilesList.Add(new BorderTile(12, 12, 13, this));

		for (int i = 0; i < tilesList.Count; i++)
			tilesByName.Add(tilesList[i].Name, tilesList[i]);
	}

	private void CreateCards()
	{
		cards[0] = new ForwardFiveCard(cardSprites[0], this);
		cards[2] = new SwapCard(cardSprites[1], this);
		cards[3] = new ExtraRollCard(cardSprites[2], this);
		cards[4] = new DoubleRollsCard(cardSprites[3], this);
		cards[5] = new HalfRollsCard(cardSprites[4], this);
		cards[6] = new SkipTurnCard(cardSprites[5], this);

		for (int i = 0; i < cards.Length; i++)
			allowedCards.Add(cards[i]);
	}

	private void CreateFunctions()
	{
		functions.Add("Teleport", new TeleportFunction(this));
		functions.Add("SetTile", new SetTileFunction(this));
		functions.Add("SetMoves", new SetMovesFunction(this));
		functions.Add("Random", new RandomFunction(this));
		functions.Add("RandomTeleport", new RandomTeleportFunction(this));
		functions.Add("Timer", new TimerFunction(this));
		functions.Add("SetLine", new SetLineFunction(this));
		functions.Add("SetSquare", new SetSquareFunction(this));
		functions.Add("If", new IfFunction(this));
		functions.Add("TeleportInArea", new TeleportInAreaFunction(this));
		functions.Add("Repeat", new RepeatFunction(this));
		functions.Add("Swap", new SwapFunction(this));
		functions.Add("Skip", new SkipFunction(this));
		functions.Add("SetData", new SetDataFunction(this));
		functions.Add("Direction", new DirectionFunction(this));
		functions.Add("Respawn", new RespawnFunction(this));
	}
}
