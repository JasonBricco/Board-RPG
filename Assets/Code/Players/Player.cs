//
//  Player.cs
//  BoardRPG
//
//  Created by Jason Bricco on 2/7/16.
//  Copyright © 2016 Jason Bricco. All rights reserved.
//

using UnityEngine;
using System.Collections;

public sealed class Player : Entity
{
	private void Awake()
	{
		EventManager.StartListening("MovePressed", MoveHandler);
		EventManager.StartListening("AttackPressed", AttackHandler);
		EventManager.StartListening("PassPressed", PassHandler);
	}

	public override void BeginTurn()
	{
		if (!beingDeleted)
			UI.EnableGraphic("ActionPanel");
	}

	private void MoveHandler(object state)
	{
		UI.DisableGraphic("ActionPanel");
		StartCoroutine(Move());
	}

	private IEnumerator Move()
	{
		yield return StartCoroutine(base.Move(GetDieRoll()));
		playerManager.NextTurn();
	}

	private void AttackHandler(object state)
	{
	}

	private void PassHandler(object state)
	{
		UI.DisableGraphic("ActionPanel");
		playerManager.NextTurn();
	}

	public override void Delete()
	{
		EventManager.StopListening("MovePressed", MoveHandler);
		EventManager.StopListening("AttackPressed", AttackHandler);
		EventManager.StopListening("PassPressed", PassHandler);

		UI.DisableGraphic("ActionPanel");

		base.Delete();
	}
}
