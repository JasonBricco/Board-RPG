using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public sealed class Player : Entity, IUpdatable
{
	private GameObject actionPanel;

	private void Start()
	{
		type = EntityType.Player;

		actionPanel = SceneItems.GetItem("ActionPanel");
		EventManager.StartListening("EndTurnPressed", EndTurnHandler);
	}

	public override Sprite LoadSprite()
	{
		return Resources.Load<Sprite>("Entity/Player");
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
				actionPanel.SetActive(true);

				Engine.StartUpdating(this);
			}
		}
	}

	public void UpdateFrame()
	{
		if (!isMoving && RemainingMP > 0)
		{
			int x = Mathf.RoundToInt(Input.GetAxisRaw("Horizontal"));
			int y = Mathf.RoundToInt(Input.GetAxisRaw("Vertical"));

			if (x == 0 && y == 0) 
				return;

			if (x != 0 && y != 0)
			{
				int n = Random.Range(0, 2);
				if (n == 0) x = 0;
				else y = 0;
			}

			Vector2i dir = new Vector2i(x, y);
			Vector2i cur = Utils.TileFromWorldPos(Position);

			Vector2i next = cur + dir;

			if (!Map.InTileBounds(next.x, next.y))
				return;
			
			if (Map.GetTileType(1, next.x, next.y).IsPassable(next.x, next.y))
				StartCoroutine(MoveToPosition(transform.position, Utils.WorldFromTilePos(next)));
		}
	}

	private void EndTurnHandler(Data data)
	{
		Engine.StopUpdating(this);
		actionPanel.SetActive(false);
		StartCoroutine(EndTurn());
	}

	private IEnumerator EndTurn()
	{
		while (isMoving) yield return null;

		ProcessPending(false);
		manager.NextTurn();
	}

	public override void Delete()
	{
		Engine.StopUpdating(this);
		EventManager.StopListening("EndTurnPressed", EndTurnHandler);
		actionPanel.SetActive(false);
		base.Delete();
	}
}
