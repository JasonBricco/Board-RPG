//
//  Enemy.cs
//  BoardRPG
//
//  Created by Jason Bricco on 2/7/16.
//  Copyright © 2016 Jason Bricco. All rights reserved.
//

using UnityEngine;
using System.Collections;

public sealed class Enemy : Entity 
{
	public override void BeginTurn()
	{
		if (!beingDeleted)
			StartCoroutine(DoMove());
	}

	private IEnumerator DoMove()
	{
		remainingMoves = GetDieRoll();

		while (remainingMoves > 0)
		{
			Vector2i current = new Vector2i(transform.position);
			Vector2i dir;

			GetMoveDirection(current, out dir);

			if (!dir.Equals(Vector2i.zero)) 
			{
				yield return StartCoroutine(MoveToPosition(transform.position, GetTargetPos(current, dir)));

				lastDirection = -dir;
				remainingMoves--;
				TriggerTileFunction();
			}
			else
				remainingMoves = 0;
		}

		playerManager.NextTurn();
	}

	public override bool HandleSplitPath(out Vector2i dir)
	{
		int result = Random.Range(0, possibleMoves.Count);
		dir = possibleMoves[result];
		return true;
	}
}
