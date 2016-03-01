using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public delegate int DiceMod();

public class Entity : MonoBehaviour
{
	protected int entityID; 

	protected BoardManager boardManager;

	private float speed = 160.0f;

	protected bool skipTurn = false;
	private bool isFlipped = false;

	protected Vector2i lastDirection = Vector2i.zero;
	protected List<Vector2i> possibleMoves = new List<Vector2i>();
	protected Queue<Vector2i> forcedDirections = new Queue<Vector2i>();

	protected bool beingDeleted = false;

	private DiceMod diceMod = null;

	public Vector2 Position
	{
		get { return transform.position; }
	}

	public int EntityID 
	{ 
		get { return entityID; } 
	}

	public bool IsFlipped 
	{ 
		get { return isFlipped; } 
	}

	public int RemainingMoves { get; set; }
	public bool Wait { get; set; }

	public void SetReferences(int entityID, BoardManager boardManager)
	{
		this.entityID = entityID;
		this.boardManager = boardManager;
	}

	public virtual void BeginTurn() {}

	public void SkipNextTurn()
	{
		skipTurn = true;
	}

	public void SetDiceMod(DiceMod modFunc)
	{
		diceMod = modFunc;
	}

	public int GetDieRoll()
	{
		if (diceMod != null)
			return diceMod.Invoke();
		
		if (RemainingMoves == 0)
			return Random.Range(1, 7);

		return RemainingMoves;
	}

	protected Vector3 GetTargetPos(Vector2i current, Vector2i direction)
	{
		return (current + (direction * TileType.Size)).ToVector3();
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
		while ((transform.position - target).magnitude > 0.05f)
		{
			transform.position = Vector3.MoveTowards(transform.position, target, Time.deltaTime * speed);
			yield return null;
		}

		transform.position = target;
	}

	protected void TriggerTileFunction()
	{
		Vector2i tPos = Utils.TileFromWorldPos(transform.position);
		boardManager.GetTileType(0, tPos.x, tPos.y).OnEnter(tPos.x, tPos.y, this);
		boardManager.GetTileType(1, tPos.x, tPos.y).OnEnter(tPos.x, tPos.y, this);
	}

	public void SetTo(Vector3 wPos)
	{
		lastDirection = Vector2i.zero;
		transform.position = wPos;
	}

	public IEnumerator SlideTo(Vector2i tPos, Vector2i dir)
	{
		Vector3 wPos = Utils.WorldFromTilePos(tPos);
		yield return StartCoroutine(MoveToPosition(transform.position, wPos));
		lastDirection = -dir;
		TriggerTileFunction();
	}

	public void Flip()
	{
		isFlipped = !isFlipped;
		forcedDirections.Clear();
		forcedDirections.Enqueue(lastDirection);
	}

	protected bool GetMoveDirection(Vector2i current, out Vector2i dir)
	{
		current.x >>= TileType.SizeBits;
		current.y >>= TileType.SizeBits;

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
