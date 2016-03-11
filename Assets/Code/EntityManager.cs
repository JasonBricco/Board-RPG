using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class EntityManager : MonoBehaviour 
{
	[SerializeField] private GameCamera cam;
	[SerializeField] private GameObject worldCanvas;

	public GameObject WorldCanvas { get { return worldCanvas; } }

	private List<Entity> entityList = new List<Entity>();

	private GameObject turnDisplayPanel;
	private Text turnText;

	private int lastTurn = -1;

	private List<Vector2i> startPositions = new List<Vector2i>();

	private Entity currentEntity;

	private Pathfinder pathfinder;

	private void Awake()
	{
		pathfinder = Map.Pathfinder;

		EventManager.StartListening("StateChanged", StateChangedHandler);
		EventManager.StartListening("SwapEntities", SwapEntities);
		EventManager.StartListening("Teleport", TeleportEntity);
		EventManager.StartListening("SpawnEntity", SpawnEntity);
		EventManager.StartListening("PlayPressed", PlayPressedHandler);
		EventManager.StartListening("ValidateEntities", ValidateEntities);
		EventManager.StartListening("SetEntityMoves", SetEntityMoves);
		EventManager.StartListening("SkipEntityTurn", SkipEntityTurn);
		EventManager.StartListening("RemoveStartTile", RemoveStartTile);
	}

	private void Start()
	{
		turnDisplayPanel = UIStore.GetGraphic("TurnDisplayPanel");
		turnText = UIStore.GetGraphic<Text>("TurnText");
	}

	private void StateChangedHandler(Data data)
	{
		switch (data.state)
		{
		case GameState.Editing:
			ClearEntityList();
			break;
		}
	}

	public Entity GetEntity(int ID)
	{
		if (ID >= 0 && ID < entityList.Count)
			return entityList[ID];

		return null;
	}

	private void SwapEntities(Data data)
	{
		if (entityList.Count == 1)
			return;
		
		Entity a, b;

		if (data.entity != null)
		{
			a = data.entity;
		}
		else
		{
			if (!IsValidEntity(data.num) || !IsValidEntity(data.secondNum))
				return;

			a = entityList[data.num];
		}

		do { b = entityList[Random.Range(0, entityList.Count)]; }
		while (b.EntityID == a.EntityID);
			
		Vector3 otherPos = b.Position;
		b.SetTo(a.Position);
		a.SetTo(otherPos);
	}

	private void TeleportEntity(Data data)
	{
		if (data.entity != null)
			data.entity.SetTo(data.position);
		else
		{
			if (!IsValidEntity(data.num))
				return;

			entityList[data.num].SetTo(data.position);
		}
	}

	private void SetEntityMoves(Data data)
	{
		Entity entity;

		if (data.entity != null)
			entity = data.entity;
		else
		{
			if (!IsValidEntity(data.num))
				return;

			entity = entityList[data.num];
		}

		if (!data.mode)
			entity.MP = data.secondNum;

		entity.RemainingMP = data.secondNum;
	}

	private void SkipEntityTurn(Data data)
	{
		if (data.entity != null)
			data.entity.SkipNextTurn();
		else
		{
			if (!IsValidEntity(data.num))
				return;

			entityList[data.num].SkipNextTurn();
		}
	}

	private void PlayPressedHandler(Data data)
	{
		Serializer.Save();

		Entity player = CreateEntity("Player", typeof(Player));
		currentEntity = player;

		pathfinder.FillSearchGrid();
		Initialize();

		if (startPositions.Count == 0)
			CreateDefaultStartTile();

		SpawnEntity(player);

		StateManager.ChangeState(GameState.Playing);
		NextTurn(0);
	}

	private Entity CreateEntity(string name, System.Type type)
	{
		GameObject entityObj = new GameObject(name);
		entityObj.transform.localScale = new Vector3(32.0f, 32.0f);
		entityObj.transform.SetParent(this.transform);
		SpriteRenderer rend = entityObj.AddComponent<SpriteRenderer>();
		entityObj.AddComponent(type);

		Entity entity = entityObj.GetComponent<Entity>();
		entityList.Add(entity);

		rend.sprite = entity.LoadSprite();

		entity.SetReferences(entityList.Count - 1, this, pathfinder);

		return entity;
	}

	private void SpawnEntity(Entity entity)
	{
		if (startPositions.Count == 0)
			CreateDefaultStartTile();

		entity.SetTo(startPositions[Random.Range(0, startPositions.Count)]);
	}

	private void SpawnEntity(Data data)
	{
		if (!IsValidEntity(data.num))
		{
			ErrorHandler.LogText("Found an invalid entity ID when attempting to spawn.");
			return;
		}

		Entity entity = entityList[data.num];

		if (entity.Type == EntityType.Player)
			SpawnEntity(entity);
	}

	private void Initialize()
	{
		startPositions.Clear();

		for (int x = 0; x < Map.WidthInChunks; x++)
		{
			for (int y = 0; y < Map.WidthInChunks; y++)
			{
				Chunk chunk = Map.GetChunkDirect(x, y);

				if (chunk != null)
				{
					for (int cX = 0; cX < Chunk.Size; cX++)
					{
						for (int cY = 0; cY < Chunk.Size; cY++)
						{
							Tile tile = chunk.GetTile(1, cX, cY);
							Vector2i tPos = new Vector2i((x * Chunk.Size) + cX, (y * Chunk.Size) + cY);

							if (tile.Equals(Tiles.Start))
								startPositions.Add(tPos);

							if (tile.Equals(Tiles.Monster))
							{
								Entity monster = CreateEntity("Monster", typeof(DefaultMonster));
								monster.SetTo(tPos);
							}
						}
					}
				}
			}
		}
	}

	private void CreateDefaultStartTile()
	{
		Vector2i midPos = new Vector2i(Map.Size / 2, Map.Size / 2);
		Tile midTile = Map.GetTile(0, midPos.x, midPos.y);

		if (midTile.Equals(Tiles.Air))
			Map.SetTileFast(midPos, Tiles.Grass);

		Map.SetTileFast(midPos, Tiles.Start);
		startPositions.Add(midPos);

		Map.RebuildChunks();
	}

	private void RemoveStartTile(Data data)
	{
		startPositions.Remove(new Vector2i(data.position));
	}

	public void NextTurn(int forcedTurn = -1)
	{
		int turnIndex = forcedTurn == -1 ? (lastTurn + 1) % entityList.Count : forcedTurn;
		lastTurn = turnIndex;

		currentEntity = entityList[turnIndex];
		StartCoroutine(CallTurn(currentEntity));
	}

	private IEnumerator CallTurn(Entity entity)
	{
		cam.followTarget = entity;
		turnText.text = entity.name + "'s Turn";

		turnDisplayPanel.SetActive(true);
		yield return new WaitForSeconds(1.5f);
		turnDisplayPanel.SetActive(false);

		entity.BeginTurn();
	}

	public void ClearEntityList()
	{
		for (int i = 0; i < entityList.Count; i++)
			entityList[i].Delete();

		entityList.Clear();
	}

	private void ValidateEntities(Data data)
	{
		for (int i = entityList.Count - 1; i >= 0; i--)
		{
			Entity entity = entityList[i];
			Vector2i tPos = Utils.TileFromWorldPos(entity.Position);

			if (!Map.GetTileType(1, tPos.x, tPos.y).IsPassable(tPos.x, tPos.y))
			{
				if (entity.Type == EntityType.Player)
					SpawnEntity(entityList[i]);
				else
				{
					entity.Delete();
					entityList.RemoveAt(i);
				}
			}
		}
	}

	private bool IsValidEntity(int entityID)
	{
		if (entityID < 0 || entityID > entityList.Count)
			return false;

		return true;
	}
}
