//
//  Entity.cs
//  BoardRPG
//
//  Created by Jason Bricco on 2/6/16.
//  Copyright © 2016 Jason Bricco. All rights reserved.
//

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Entity : MonoBehaviour
{
	protected UIManager UI;
	protected BoardManager boardManager;
	protected PlayerManager playerManager;

	private float speed = 5.0f;

	private Vector2i lastDirection = Vector2i.zero;
	private List<Vector2i> possibleMoves = new List<Vector2i>();

	protected bool beingDeleted = false;

	public void SetReferences(UIManager UI, BoardManager boardManager, PlayerManager playerManager)
	{
		this.UI = UI;
		this.boardManager = boardManager;
		this.playerManager = playerManager;
	}

	protected int GetDieRoll()
	{
		return Random.Range(1, 7);
	}

	public IEnumerator Move(int distance)
	{
		for (int i = 0; i < distance; i++)
		{
			Vector2i current = new Vector2i(transform.position);

			Vector2i move = GetMoveDirection(current);

			if (move.Equals(Vector2i.zero)) continue; 

			lastDirection = -move;
			Vector3 target = (current + move).ToVector3();

			yield return StartCoroutine(MoveToPosition(transform.position, target));
		}
	}

	private IEnumerator MoveToPosition(Vector3 current, Vector3 target)
	{
		float t = 0.0f;

		while (t < 1.0f)
		{
			transform.position = Vector3.Lerp(current, target, t);
			t += Time.deltaTime * speed;
			yield return null;
		}

		transform.position = target;
	}

	public void SetTo(Vector3 position)
	{
		transform.position = position;
	}

	public virtual void BeginTurn()
	{
	}

	protected Vector2i GetMoveDirection(Vector2i current)
	{
		possibleMoves.Clear();

		for (int i = 0; i < 4; i++)
		{
			Vector2i direction = Vector2i.directions[i];

			if (!direction.Equals(lastDirection))
			{
				Vector2i newPos = current + direction;

				Tile tile = boardManager.GetTileSafe(newPos.x, newPos.y);

				if (tile.ID != 0)
					possibleMoves.Add(direction);
			}
		}

		switch (possibleMoves.Count)
		{
		case 0:
			return lastDirection;

		case 1:
			return possibleMoves[0];

		default: 
			return Vector2i.zero;
		}
	}

	public virtual void Delete()
	{
		beingDeleted = true;
		StopAllCoroutines();

		StartCoroutine(DestroyAtFrameEnd());
	}

	private IEnumerator DestroyAtFrameEnd()
	{
		yield return new WaitForEndOfFrame();
		Destroy(gameObject);
	}
}
