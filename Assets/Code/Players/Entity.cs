using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public delegate int DiceMod();

public class Entity : MonoBehaviour
{
	protected EntityManager manager;
	protected int entityID; 

	private float speed = 160.0f;

	protected bool skipTurn = false;
	protected bool beingDeleted = false;

	protected Pathfinder pathfinder;

	public Vector2 Position
	{
		get { return transform.position; }
	}

	public int EntityID 
	{ 
		get { return entityID; } 
	}

	public int MP = 5;
	public int remainingMoves;
	public bool wait;

	public void SetReferences(int entityID, EntityManager manager, Pathfinder pathfinder)
	{
		this.manager = manager;
		this.entityID = entityID;
		this.pathfinder = pathfinder;
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

	protected IEnumerator MoveToPosition(Vector3 current, Vector3 target)
	{
		while ((transform.position - target).magnitude > 0.05f)
		{
			transform.position = Vector3.MoveTowards(transform.position, target, Time.deltaTime * speed);
			yield return null;
		}

		transform.position = target;
		remainingMoves--;
		TriggerTileFunction();

		while (wait) yield return null;
	}

	protected void TriggerTileFunction()
	{
		Vector2i tPos = Utils.TileFromWorldPos(transform.position);
		Map.GetTileType(0, tPos.x, tPos.y).OnEnter(tPos.x, tPos.y, this);
		Map.GetTileType(1, tPos.x, tPos.y).OnEnter(tPos.x, tPos.y, this);
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
