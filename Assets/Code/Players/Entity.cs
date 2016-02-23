using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Entity : MonoBehaviour
{
	protected int entityID; 
	public int EntityID { get { return entityID; } }

	protected BoardManager boardManager;
	protected PlayerManager playerManager;

	private float speed = 5.0f;

	private bool isFlipped = false;
	public bool IsFlipped { get { return isFlipped;  } }

	protected Vector2i lastDirection = Vector2i.zero;
	protected List<Vector2i> possibleMoves = new List<Vector2i>();
	protected Queue<Vector2i> forcedDirections = new Queue<Vector2i>();

	protected bool beingDeleted = false;

	public int RemainingMoves { get; set; }
	public bool Wait { get; set; }

	public void SetReferences(int entityID, BoardManager boardManager, PlayerManager playerManager)
	{
		this.entityID = entityID;
		this.boardManager = boardManager;
		this.playerManager = playerManager;
	}

	public virtual void BeginTurn()
	{
	}

	protected int GetDieRoll()
	{
		if (RemainingMoves == 0)
			return Random.Range(1, 7);

		return RemainingMoves;
	}

	protected Vector3 GetTargetPos(Vector2i current, Vector2i direction)
	{
		return (current + (direction * Tile.Size)).ToVector3();
	}

	protected IEnumerator Move(Vector2i dir, Vector2i current)
	{
		if (!dir.Equals(Vector2i.zero))
		{
			yield return StartCoroutine(MoveToPosition(transform.position, GetTargetPos(current, dir)));

			lastDirection = -dir;
			RemainingMoves--;
			
			TriggerTileFunction();
		}
		else
			RemainingMoves = 0;

		while (Wait) yield return null;
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

	protected void TriggerTileFunction()
	{
		Vector2i tPos = Utils.TileFromWorldPos(transform.position);
		boardManager.GetTile(0, tPos.x, tPos.y).OnEnter(tPos.x, tPos.y, this);
		boardManager.GetTile(1, tPos.x, tPos.y).OnEnter(tPos.x, tPos.y, this);
	}

	public void SetTo(Vector3 position)
	{
		lastDirection = Vector2i.zero;
		transform.position = position;
	}

	public void Flip()
	{
		isFlipped = !isFlipped;
		forcedDirections.Enqueue(lastDirection);
	}

	protected bool GetMoveDirection(Vector2i current, out Vector2i dir)
	{
		current.x >>= Tile.SizeBits;
		current.y >>= Tile.SizeBits;

		possibleMoves.Clear();

		for (int i = 0; i < 4; i++)
		{
			Vector2i direction = Vector2i.directions[i];

			if (!direction.Equals(lastDirection))
			{
				Vector2i newPos = current + direction;

				Tile tile = boardManager.GetTileSafe(0, newPos.x, newPos.y);

				if (tile.ID != 0)
					possibleMoves.Add(direction);
			}
		}

		switch (possibleMoves.Count)
		{
		case 0:
			dir = lastDirection;
			return true;

		case 1:
			dir = possibleMoves[0];
			return true;

		default:
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
