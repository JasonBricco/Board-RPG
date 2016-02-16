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

public sealed class PlayerManager : IUpdatable
{
	private BoardManager boardManager;
	private UIManager UI;

	private Sprite playerSprite, enemySprite;

	private List<Entity> entityList = new List<Entity>();

	private int lastTurn = -1;

	public PlayerManager(BoardManager boardManager, UIManager UI)
	{
		this.boardManager = boardManager;
		this.UI = UI;

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
		entity.SetReferences(UI, boardManager, this);

		return entity;
	}

	private void PlayPressedHandler(object data)
	{
		List<Vector2i> startTiles = boardManager.GetData().startTiles;

		if (startTiles.Count == 0)
			return;

		StateManager.ChangeState(GameState.Playing);

		entityList.Add(CreateEntity("Player", typeof(Player), playerSprite));
		entityList.Add(CreateEntity("Enemy", typeof(Enemy), enemySprite));

		int initialIndex = Random.Range(0, startTiles.Count);
		Vector2i playerTile = startTiles[initialIndex];

		Vector3 playerPos = new Vector3(playerTile.x, playerTile.y, 0.0f);
		entityList[0].SetTo(playerPos);

		if (startTiles.Count == 1)
			entityList[1].SetTo(playerPos);
		else
		{
			int newIndex;

			do { newIndex = Random.Range(0, startTiles.Count); }
			while (newIndex == initialIndex);

			Vector2i newPos = startTiles[newIndex];
			entityList[1].SetTo(new Vector3(newPos.x, newPos.y, 0.0f));
		}
			
		NextTurn();
	}

	public void UpdateTick()
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

		Engine.Instance.StartCoroutine(CallTurn(entityList[turnIndex]));
	}

	private IEnumerator CallTurn(Entity entity)
	{
		Text turnText = UI.GetGraphic<Text>("TurnText");
		turnText.text = entity.name + "'s Turn";

		UI.EnableGraphic("TurnDisplayPanel");
		yield return new WaitForSeconds(1.5f);
		UI.DisableGraphic("TurnDisplayPanel");

		entity.BeginTurn();
	}
}
