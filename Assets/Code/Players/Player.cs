//
//  Player.cs
//  BoardRPG
//
//  Created by Jason Bricco on 2/7/16.
//  Copyright © 2016 Jason Bricco. All rights reserved.
//

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public sealed class Player : Entity
{
	private Queue<Vector2i> forcedDirections = new Queue<Vector2i>();

	private void Awake()
	{
		EventManager.StartListening("MovePressed", MoveHandler);
		EventManager.StartListening("AttackPressed", AttackHandler);
		EventManager.StartListening("PassPressed", PassHandler);
		EventManager.StartListening("DirectionArrowPressed", DirectionArrowPressed);
	}

	public override void BeginTurn()
	{
		if (!beingDeleted)
			UI.EnableGraphic("ActionPanel");
	}

	private void MoveHandler(object state)
	{
		UI.DisableGraphic("ActionPanel");
		remainingMoves = GetDieRoll();
		StartCoroutine(DoMove());
	}

	private IEnumerator DoMove()
	{
		while (remainingMoves > 0)
		{
			Vector2i current = new Vector2i(transform.position);
			Vector2i move;

			if (forcedDirections.Count > 0)
				move = forcedDirections.Dequeue();
			else
			{
				bool success = GetMoveDirection(current, out move);

				if (!success) yield break;

				if (move.Equals(Vector2i.zero)) 
				{
					remainingMoves = 0;
					yield break;
				}
			}

			lastDirection = -move;
			Vector3 target = (current + move).ToVector3();

			yield return StartCoroutine(MoveToPosition(transform.position, target));

			remainingMoves--;
		}
			
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

	public override bool HandleSplitPath(out Vector2i dir)
	{
		dir = Vector2i.zero;

		ArrowManager arrowManager = UI.GetGraphic<ArrowManager>("DirectionArrows");
		arrowManager.DisableArrows();

		for (int i = 0; i < possibleMoves.Count; i++)
			arrowManager.EnableArrow(possibleMoves[i]);
		
		UI.EnableGraphic("DirectionArrows");

		return false;
	}

	private void DirectionArrowPressed(object direction)
	{
		switch ((string)direction)
		{
		case "Left":
			forcedDirections.Enqueue(Vector2i.left);
			break;

		case "Right":
			forcedDirections.Enqueue(Vector2i.right);
			break;

		case "Up":
			forcedDirections.Enqueue(Vector2i.up);
			break;

		case "Down":
			forcedDirections.Enqueue(Vector2i.down);
			break;
		}

		UI.DisableGraphic("DirectionArrows");
		StartCoroutine(DoMove());
	}

	public override void Delete()
	{
		EventManager.StopListening("MovePressed", MoveHandler);
		EventManager.StopListening("AttackPressed", AttackHandler);
		EventManager.StopListening("PassPressed", PassHandler);
		EventManager.StopListening("DirectionArrowPressed", DirectionArrowPressed);

		UI.DisableGraphic("ActionPanel");
		UI.DisableGraphic("DirectionArrows");
		base.Delete();
	}
}
