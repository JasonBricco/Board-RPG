using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class PendingFunction
{
	public int entityID;
	public int turnsRemaining;
	public Data data;
	public UnityAction<Data> function;

	public PendingFunction(int entityID, int turns, Data data, UnityAction<Data> function)
	{
		this.entityID = entityID;
		this.turnsRemaining = turns;
		this.data = data;
		this.function = function;
	}
}

public sealed class PlayerManager : MonoBehaviour, IUpdatable
{
	[SerializeField] private BoardManager boardManager;
	private GameObject turnDisplayPanel;
	private Text turnText;

	private Sprite playerSprite, enemySprite;

	private List<Entity> entityList = new List<Entity>();

	private int lastTurn = -1;

	private float startTime = 0.0f;
	private float exitDelay = 0.0f;

	private Entity currentEntity;

	private List<PendingFunction> pendingFunctions = new List<PendingFunction>();

	public Entity CurrentEntity 
	{
		get { return currentEntity; }
	}

	private void Start()
	{
		turnDisplayPanel = UIStore.GetGraphic("TurnDisplayPanel");
		turnText = UIStore.GetGraphic<Text>("TurnText");

		Engine.StartUpdating(this);

		playerSprite = Resources.Load<Sprite>("Sprites/Player");
		enemySprite = Resources.Load<Sprite>("Sprites/Enemy");

		EventManager.StartListening("PlayPressed", PlayPressedHandler);
	}

	private void StateChangedHandler(int state)
	{
		if ((GameState)state == GameState.Editing)
			pendingFunctions.Clear();
	}

	private Entity CreateEntity(string name, System.Type type, Sprite sprite, int ID)
	{
		GameObject entityObj = new GameObject(name);
		entityObj.transform.localScale = new Vector3(32.0f, 32.0f);
		SpriteRenderer rend = entityObj.AddComponent<SpriteRenderer>();
		entityObj.AddComponent(type);
		rend.sprite = sprite;

		Entity entity = entityObj.GetComponent<Entity>();
		entity.SetReferences(ID, boardManager, this);
		return entity;
	}

	public Entity GetEntity(int entityID)
	{
		if (entityID < 0 || entityID >= entityList.Count)
			return null;
		
		return entityList[entityID];
	}

	private void PlayPressedHandler(int data)
	{
		startTime = Time.time;
		exitDelay = startTime + 2.0f;

		List<Vector2i> startTiles = boardManager.GetData().startTiles;

		if (startTiles.Count == 0)
			return;

		entityList.Add(CreateEntity("Player", typeof(Player), playerSprite, 0));
		entityList.Add(CreateEntity("Enemy", typeof(Enemy), enemySprite, 1));

		int initialIndex = Random.Range(0, startTiles.Count);
		Vector2i playerTile = startTiles[initialIndex];
		Vector3 worldPos = Utils.WorldFromTilePos(playerTile);

		entityList[0].SetTo(worldPos);

		if (startTiles.Count == 1)
			entityList[1].SetTo(worldPos);
		else
		{
			int newIndex;

			do { newIndex = Random.Range(0, startTiles.Count); }
			while (newIndex == initialIndex);

			Vector3 newPos = Utils.WorldFromTilePos(startTiles[newIndex]);
			entityList[1].SetTo(new Vector3(newPos.x, newPos.y, 0.0f));
		}
			
		int turnIndex = Random.Range(0, entityList.Count);
		currentEntity = entityList[turnIndex];

		StateManager.ChangeState(GameState.Playing);
		NextTurn(turnIndex);
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
					for (int i = 0; i < entityList.Count; i++)
						entityList[i].Delete();

					entityList.Clear();

					StateManager.ChangeState(GameState.Editing);
				}
			}
		}
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
}
