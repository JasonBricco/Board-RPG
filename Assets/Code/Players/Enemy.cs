using UnityEngine;
using System.Collections;

public sealed class Enemy : Entity 
{
	public override void BeginTurn()
	{
		if (skipTurn) 
		{
			skipTurn = false;
			boardManager.NextTurn();
		}
		else
		{
			if (!beingDeleted)
				StartCoroutine(GetDirectionAndMove());
		}
	}

	private IEnumerator GetDirectionAndMove()
	{
		RemainingMoves = GetDieRoll();

		while (RemainingMoves > 0)
		{
			Vector2i current = new Vector2i(transform.position);
			Vector2i dir;

			if (forcedDirections.Count > 0)
				dir = forcedDirections.Dequeue();
			else
				GetMoveDirection(current, out dir);

			yield return StartCoroutine(Move(dir, current));
		}

		boardManager.NextTurn();
	}

	public override bool HandleSplitPath(out Vector2i dir)
	{
		int result = Random.Range(0, possibleMoves.Count);
		dir = possibleMoves[result];
		return true;
	}
}
