using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public sealed class Player : Entity
{
	private GameObject actionPanel;

	private Queue<PossibleTile> floodQueue = new Queue<PossibleTile>();
	private HashSet<Vector2i> filledPositions = new HashSet<Vector2i>();
	private List<GameObject> activeSelections = new List<GameObject>();

	private SelectionPool selectionPool = new SelectionPool();

	private void Start()
	{
		actionPanel = UIStore.GetGraphic("ActionPanel");

		EventManager.StartListening("MovePressed", MoveHandler);
		EventManager.StartListening("AttackPressed", AttackHandler);
		EventManager.StartListening("PassPressed", PassHandler);
		EventManager.StartListening("TileSelected", TileSelected);
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
				actionPanel.SetActive(true);
		}
	}

	private void MoveHandler(Data data)
	{
		actionPanel.SetActive(false);
		ShowRange(MP);
	}
		
	private void AttackHandler(Data data)
	{
	}

	private void PassHandler(Data data)
	{
		actionPanel.SetActive(false);
		manager.NextTurn();
	}

	private void ShowRange(int range)
	{
		Vector2i startPos = Utils.TileFromWorldPos(Position);

		PossibleTile startTile = new PossibleTile(startPos, range);
		floodQueue.Enqueue(startTile);
		filledPositions.Add(startPos);

		while (floodQueue.Count > 0)
		{
			PossibleTile next = floodQueue.Dequeue();

			if (next.remaining == 0)
				continue;

			Vector2i cur = next.pos;

			for (int i = 0; i < 4; i++)
			{
				Vector2i nextPos = cur + Vector2i.directions[i];

				if (filledPositions.Contains(nextPos))
					continue;

				if (Map.GetTileTypeSafe(1, nextPos.x, nextPos.y).IsPassable(nextPos.x, nextPos.y))
				{
					floodQueue.Enqueue(new PossibleTile(nextPos, next.remaining - 1));
					filledPositions.Add(nextPos);

					GameObject selection = selectionPool.GetSelection();
					selection.transform.SetParent(manager.WorldCanvas.transform);
					selection.transform.position = Utils.WorldFromTilePos(nextPos);
					activeSelections.Add(selection);
				}
			}
		}

		manager.WorldCanvas.SetActive(true);
	}

	private void TileSelected(Data data)
	{
		ClearSelections();

		Vector2i start = Utils.TileFromWorldPos(Position);
		Vector2i end = Utils.TileFromWorldPos(data.position);

		StartCoroutine(FollowPath(pathfinder.FindPath(start, end)));
	}

	private IEnumerator FollowPath(List<Vector2i> path)
	{
		for (int i = 0; i < path.Count; i++)
			yield return StartCoroutine(MoveToPosition(transform.position, Utils.WorldFromTilePos(path[i])));

		manager.NextTurn();
	}

	private void ClearSelections()
	{
		GameObject canvas = manager.WorldCanvas;

		if (canvas.activeSelf)
		{
			canvas.SetActive(false);

			for (int i = 0; i < activeSelections.Count; i++)
				selectionPool.ReturnSelection(activeSelections[i]);

			activeSelections.Clear();
			filledPositions.Clear();
		}
	}

	public override void Delete()
	{
		ClearSelections();

		EventManager.StopListening("MovePressed", MoveHandler);
		EventManager.StopListening("AttackPressed", AttackHandler);
		EventManager.StopListening("PassPressed", PassHandler);
		EventManager.StopListening("TileSelected", TileSelected);

		actionPanel.SetActive(false);

		base.Delete();
	}
}
