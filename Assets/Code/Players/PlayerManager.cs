//
//  PlayerManager.cs
//  BoardRPG
//
//  Created by Jason Bricco on 2/6/16.
//  Copyright © 2016 Jason Bricco. All rights reserved.
//

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public sealed class PlayerManager : MonoBehaviour
{
	[SerializeField] private BoardManager boardManager;

	private Sprite playerSprite, enemySprite;

	private List<Entity> entityList = new List<Entity>();

	private int lastTurn = -1;

	private void Awake()
	{
		playerSprite = Resources.Load<Sprite>("Textures/Player");
		enemySprite = Resources.Load<Sprite>("Textures/Enemy");

		EventManager.StartListening("PlayPressed", PlayPressedHandler);
	}

	private Entity CreateEntity(string name, System.Type type, Sprite sprite)
	{
		GameObject entityObj = new GameObject(name);
		entityObj.transform.localScale = new Vector3(32.0f, 32.0f);
		SpriteRenderer rend = entityObj.AddComponent<SpriteRenderer>();
		entityObj.AddComponent(type);
		rend.sprite = sprite;

		Entity entity = entityObj.GetComponent<Entity>();
		return entity;
	}

	public Entity GetEntity(int entityID)
	{
		return entityList[entityID];
	}

	private void PlayPressedHandler(int data)
	{
		List<Vector2i> startTiles = boardManager.GetData().startTiles;

		if (startTiles.Count == 0)
			return;

		StateManager.ChangeState(GameState.Playing);

		entityList.Add(CreateEntity("Player", typeof(Player), playerSprite));
		entityList.Add(CreateEntity("Enemy", typeof(Enemy), enemySprite));

		int initialIndex = Random.Range(0, startTiles.Count);
		Vector2i playerTile = startTiles[initialIndex];
		Vector3 worldPos = Utils.WorldFromTileCoords(playerTile);

		entityList[0].SetTo(worldPos);

		if (startTiles.Count == 1)
			entityList[1].SetTo(worldPos);
		else
		{
			int newIndex;

			do { newIndex = Random.Range(0, startTiles.Count); }
			while (newIndex == initialIndex);

			Vector3 newPos = Utils.WorldFromTileCoords(startTiles[newIndex]);
			entityList[1].SetTo(new Vector3(newPos.x, newPos.y, 0.0f));
		}
			
		NextTurn();
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			for (int i = 0; i < entityList.Count; i++)
				entityList[i].Delete();

			entityList.Clear();

			StateManager.ChangeState(GameState.Editing);
		}
	}

	public void NextTurn()
	{
		int turnIndex = lastTurn == -1 ? Random.Range(0, entityList.Count) : (lastTurn + 1) % entityList.Count;
		lastTurn = turnIndex;

		StartCoroutine(CallTurn(entityList[turnIndex]));
	}

	private IEnumerator CallTurn(Entity entity)
	{
		UIManager.SetText("TurnText", entity.name + "'s Turn");

		UIManager.EnableGraphic("TurnDisplayPanel");
		yield return new WaitForSeconds(1.5f);
		UIManager.DisableGraphic("TurnDisplayPanel");

		entity.BeginTurn();
	}
}
