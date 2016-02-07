//
//  PlayerManager.cs
//  BoardRPG
//
//  Created by Jason Bricco on 2/6/16.
//  Copyright © 2016 Jason Bricco. All rights reserved.
//

using UnityEngine;
using System.Collections.Generic;

public sealed class PlayerManager : IUpdatable
{
	private GameObject playerObject, enemyObject;
	private Entity player, enemy;
	private BoardManager boardManager;

	public PlayerManager(GameObject player, GameObject enemy, BoardManager boardManager)
	{
		this.playerObject = player;
		this.enemyObject = enemy;
		this.boardManager = boardManager;

		EventManager.StartListening("PlayPressed", PlayPressedHandler);
	}

	private void PlayPressedHandler(object data)
	{
		List<Vector2i> startTiles = boardManager.GetData().startTiles;

		if (startTiles.Count == 0)
			return;

		player = GameObject.Instantiate(playerObject).GetComponent<Entity>();
		enemy = GameObject.Instantiate(enemyObject).GetComponent<Entity>();

		int initialIndex = Random.Range(0, startTiles.Count);
		Vector2i playerTile = startTiles[initialIndex];

		Vector3 playerPos = new Vector3(playerTile.x, playerTile.y, -2.0f);
		player.SetTo(playerPos);

		if (startTiles.Count == 1)
			enemy.SetTo(playerPos);
		else
		{
			int newIndex;

			do { newIndex = Random.Range(0, startTiles.Count); }
			while (newIndex == initialIndex);

			Vector2i newPos = startTiles[newIndex];
			enemy.SetTo(new Vector3(newPos.x, newPos.y, -2.0f));
		}
	}

	public void UpdateTick()
	{

	}
}
