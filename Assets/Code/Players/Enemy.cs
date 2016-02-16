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
			Vector2i move;

			GetMoveDirection(current, out move);

			if (move.Equals(Vector2i.zero)) 
			{
				remainingMoves = 0;
				yield break;
			}

			lastDirection = -move;
			Vector3 target = (current + move).ToVector3();

			yield return StartCoroutine(MoveToPosition(transform.position, target));

			remainingMoves--;
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
