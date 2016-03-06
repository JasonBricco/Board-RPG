using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public delegate int DiceMod();

public class Entity : MonoBehaviour
{
	protected int entityID; 

	protected Map map;

	private float speed = 160.0f;

	protected bool skipTurn = false;
	protected bool beingDeleted = false;

	public Vector2 Position
	{
		get { return transform.position; }
	}

	public int EntityID 
	{ 
		get { return entityID; } 
	}

	public int MP = 6;
	public int remainingMoves;
	public bool wait;

	public void SetReferences(int entityID, Map map)
	{
		this.entityID = entityID;
		this.map = map;
	}

	public virtual void BeginTurn() {}

	public void SkipNextTurn()
	{
		skipTurn = true;
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

			remainingMoves--;
			TriggerTileFunction();
		}
		else
			remainingMoves = 0;

		while (wait) yield return null;
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
		map.GetTileType(0, tPos.x, tPos.y).OnEnter(tPos.x, tPos.y, this);
		map.GetTileType(1, tPos.x, tPos.y).OnEnter(tPos.x, tPos.y, this);
	}

	public void SetTo(Vector3 wPos)
	{
		transform.position = wPos;
	}

	public void SetTo(Vector2i tPos)
	{
		SetTo(Utils.WorldFromTilePos(tPos));
	}

	public IEnumerator SlideTo(Vector2i tPos, Vector2i dir)
	{
		Vector3 wPos = Utils.WorldFromTilePos(tPos);
		yield return StartCoroutine(MoveToPosition(transform.position, wPos));
		TriggerTileFunction();
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
