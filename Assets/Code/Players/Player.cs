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
	private ArrowManager arrowManager = new ArrowManager();
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
			UIManager.EnableGraphic("ActionPanel");
	}

	private void MoveHandler(int data)
	{
		UIManager.DisableGraphic("ActionPanel");
		remainingMoves = GetDieRoll();
		StartCoroutine(DoMove());
	}

	private IEnumerator DoMove()
	{
		while (remainingMoves > 0)
		{
			Vector2i current = new Vector2i(transform.position);
			Vector2i dir;

			if (forcedDirections.Count > 0)
				dir = forcedDirections.Dequeue();
			else
			{
				bool success = GetMoveDirection(current, out dir);

				if (!success) yield break;

				if (dir.Equals(Vector2i.zero)) 
				{
					remainingMoves = 0;
					yield break;
				}
			}
				
			yield return StartCoroutine(MoveToPosition(transform.position, GetTargetPos(current, dir)));

			lastDirection = -dir;
			remainingMoves--;
		}
			
		playerManager.NextTurn();
	}

	private void AttackHandler(int data)
	{
	}

	private void PassHandler(int data)
	{
		UIManager.DisableGraphic("ActionPanel");
		playerManager.NextTurn();
	}

	public override bool HandleSplitPath(out Vector2i dir)
	{
		dir = Vector2i.zero;

		arrowManager.DisableArrows();

		for (int i = 0; i < possibleMoves.Count; i++)
			arrowManager.EnableArrow(possibleMoves[i]);

		UIManager.EnableGraphic("DirectionArrows");
		return false;
	}

	private void DirectionArrowPressed(int direction)
	{
		switch ((Direction)direction)
		{
		case Direction.Left:
			forcedDirections.Enqueue(Vector2i.left);
			break;

		case Direction.Right:
			forcedDirections.Enqueue(Vector2i.right);
			break;

		case Direction.Up:
			forcedDirections.Enqueue(Vector2i.up);
			break;

		case Direction.Down:
			forcedDirections.Enqueue(Vector2i.down);
			break;
		}

		UIManager.DisableGraphic("DirectionArrows");
		StartCoroutine(DoMove());
	}

	public override void Delete()
	{
		EventManager.StopListening("MovePressed", MoveHandler);
		EventManager.StopListening("AttackPressed", AttackHandler);
		EventManager.StopListening("PassPressed", PassHandler);
		EventManager.StopListening("DirectionArrowPressed", DirectionArrowPressed);

		UIManager.DisableGraphic("ActionPanel");
		UIManager.DisableGraphic("DirectionArrows");

		base.Delete();
	}
}
