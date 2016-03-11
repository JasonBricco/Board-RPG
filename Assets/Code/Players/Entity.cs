using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Entity : MonoBehaviour
{
	protected EntityManager manager;

	protected int entityID;
	public int EntityID { get { return entityID; } }

	protected EntityType type;
	public EntityType Type { get { return type; } }

	private float speed = 160.0f;

	protected bool skipTurn = false;
	protected bool beingDeleted = false;

	protected Pathfinder pathfinder;

	public Vector3 Position { get { return transform.position; } }

	public int MP = 5;

	private int remainingMP;

	public int RemainingMP
	{
		get { return remainingMP; }
		set { remainingMP = Mathf.Clamp(value, 0, 20); }
	}

	protected int targetMP;
	public int TargetMP { get { return targetMP; } }

	public bool wait;

	private List<PendingFunction> pendingEnd = new List<PendingFunction>();
	private List<PendingFunction> pendingBegin = new List<PendingFunction>();

	public void SetReferences(int entityID, EntityManager manager, Pathfinder pathfinder)
	{
		this.manager = manager;
		this.entityID = entityID;
		this.pathfinder = pathfinder;
	}

	public virtual Sprite LoadSprite()
	{
		return null;
	}

	public void AddPending(int turns, Data data, EventDelegate func, bool onBegin)
	{
		if (turns <= 0) return;

		PendingFunction pending = new PendingFunction(turns, data, func);

		if (onBegin) pendingBegin.Add(pending);
		else pendingEnd.Add(pending);
	}

	protected void ProcessPending(bool turnBegan)
	{
		List<PendingFunction> pending = turnBegan ? pendingBegin : pendingEnd;

		for (int i = pending.Count - 1; i >= 0; i--)
		{
			PendingFunction next = pending[i];
			next.turnsRemaining--;

			if (next.turnsRemaining == 0)
			{
				next.function(next.data);
				pending.RemoveAt(i);
			}
		}
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
		RemainingMP--;
		targetMP--;
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

		while ((transform.position - wPos).magnitude > 0.05f)
		{
			transform.position = Vector3.MoveTowards(transform.position, wPos, Time.deltaTime * speed);
			yield return null;
		}

		transform.position = wPos;
		TriggerTileFunction();
	}

	protected bool PathTargetValid(Vector2i target)
	{
		Vector2i pos = Utils.TileFromWorldPos(Position);

		if (Mathf.Abs(pos.x - target.x) + Mathf.Abs(pos.y - target.y) != 1)
			return false;

		if (!Map.GetTileType(1, target.x, target.y).IsPassable(target.x, target.y))
			return false;

		return true;
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
