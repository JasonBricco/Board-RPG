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

	protected Vector2i lastDirection = Vector2i.zero;
	protected List<Vector2i> possibleMoves = new List<Vector2i>();

	protected bool beingDeleted = false;
	protected int remainingMoves = 0;

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

	protected Vector3 GetTargetPos(Vector2i current, Vector2i direction)
	{
		return (current + (direction * Tile.Size)).ToVector3();
	}

	protected IEnumerator MoveToPosition(Vector3 current, Vector3 target)
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

	protected bool GetMoveDirection(Vector2i current, out Vector2i dir)
	{
//		Debug.Log("Current position in W coords: " + current);

		current.x >>= Tile.SizeBits;
		current.y >>= Tile.SizeBits;

//		Debug.Log("Converted to T coords: " + current);

		possibleMoves.Clear();

		for (int i = 0; i < 4; i++)
		{
			Vector2i direction = Vector2i.directions[i];

			if (!direction.Equals(lastDirection))
			{
				Vector2i newPos = current + direction;

				Tile tile = boardManager.GetTileSafe(newPos.x, newPos.y);

//				Debug.Log("Looking in direction: " + direction + ", we find: " + tile.Name);

				if (tile.ID != 0)
					possibleMoves.Add(direction);
			}
		}

		switch (possibleMoves.Count)
		{
		case 0:
//			Debug.Log("Found 0 possible moves.");
			dir = lastDirection;
			return true;

		case 1:
//			Debug.Log("Found 1 possible move.");
			dir = possibleMoves[0];
			return true;

		default:
//			Debug.Log("Found a split path.");
			return HandleSplitPath(out dir);
		}
	}

	public virtual bool HandleSplitPath(out Vector2i dir)
	{
		dir = Vector2i.zero;
		return false;
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
