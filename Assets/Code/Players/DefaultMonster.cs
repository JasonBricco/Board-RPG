using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public sealed class DefaultMonster : Entity 
{
	private void Start()
	{
		type = EntityType.Monster;
	}

	public override Sprite LoadSprite()
	{
		return Resources.Load<Sprite>("Entity/DefaultMonster");
	}

	public override void BeginTurn()
	{
		if (skipTurn) 
		{
			skipTurn = false;
			manager.NextTurn();
		}
		else
		{
			if (!beingDeleted)
			{
				RemainingMP = MP;
				ProcessPending(true);
				FindPath(manager.GetEntity(0).Position);
			}
		}
	}

	private void FindPath(Vector3 target)
	{
		Vector2i start = Utils.TileFromWorldPos(Position);
		Vector2i end = Utils.TileFromWorldPos(target);

		List<Vector2i> path = pathfinder.FindPath(start, end);
		StartCoroutine(FollowPath(path));
	}

	private IEnumerator FollowPath(List<Vector2i> path)
	{
		for (int i = 0; i < path.Count; i++)
		{
			if (!PathTargetValid(path[i]))
				break;
			
			yield return StartCoroutine(MoveToPosition(transform.position, Utils.WorldFromTilePos(path[i])));

			if (RemainingMP == 0)
				break;
		}

		ProcessPending(false);
		manager.NextTurn();
	}
}
