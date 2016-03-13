using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public sealed class Player : Entity
{
	private GameObject actionPanel;
	private Button moveButton;

	private GameObject worldCanvas;

	private Queue<PossibleTile> floodQueue = new Queue<PossibleTile>();
	private HashSet<Vector2i> filledPositions = new HashSet<Vector2i>();
	private List<GameObject> activeSelections = new List<GameObject>();

	private SelectionPool selectionPool = new SelectionPool();

	private void Start()
	{
		type = EntityType.Player;

		worldCanvas = SceneItems.GetItem("WorldCanvas");

		actionPanel = SceneItems.GetItem("ActionPanel");
		moveButton = actionPanel.FindChild("Move").GetComponent<Button>();

		EventManager.StartListening("MovePressed", MoveHandler);
		EventManager.StartListening("AttackPressed", AttackHandler);
		EventManager.StartListening("EndTurnPressed", EndTurnHandler);
		EventManager.StartListening("TileSelected", TileSelected);
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
				moveButton.interactable = true;
			}
		}
	}

	private void MoveHandler(Data data)
	{
		actionPanel.SetActive(false);
		ShowRange(RemainingMP);
	}
		
	private void AttackHandler(Data data)
	{
	}

	private void EndTurnHandler(Data data)
	{
		actionPanel.SetActive(false);
		ProcessPending(false);
		manager.NextTurn();
	}

	private void ShowRange(int range)
	{
		if (range == 0) return;

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

				if (!Map.InTileBounds(nextPos.x, nextPos.y))
					continue;
				
				if (Map.GetTileType(1, nextPos.x, nextPos.y).IsPassable(nextPos.x, nextPos.y))
				{
					floodQueue.Enqueue(new PossibleTile(nextPos, next.remaining - 1));
					filledPositions.Add(nextPos);

					GameObject selection = selectionPool.GetSelection();
					selection.transform.SetParent(worldCanvas.transform);
					selection.transform.position = Utils.WorldFromTilePos(nextPos);
					activeSelections.Add(selection);
				}
			}
		}

		if (activeSelections.Count == 0)
		{
			filledPositions.Clear();
			moveButton.interactable = false;
			actionPanel.SetActive(true);
		}
	}

	private void TileSelected(Data data)
	{
		ClearSelections();

		Vector2i start = Utils.TileFromWorldPos(Position);
		Vector2i end = Utils.TileFromWorldPos(data.position);

		List<Vector2i> path = pathfinder.FindPath(start, end);
		targetMP = path.Count;

		StartCoroutine(FollowPath(path));
	}

	private IEnumerator FollowPath(List<Vector2i> path)
	{
		for (int i = 0; i < path.Count; i++)
		{
			if (!PathTargetValid(path[i]))
				break;
			
			yield return StartCoroutine(MoveToPosition(transform.position, Utils.WorldFromTilePos(path[i])));
		}

		if (RemainingMP == 0)
			moveButton.interactable = false;
		
		actionPanel.SetActive(true);
	}

	private void ClearSelections()
	{
		for (int i = 0; i < activeSelections.Count; i++)
				selectionPool.ReturnSelection(activeSelections[i]);

		activeSelections.Clear();
		filledPositions.Clear();
	}

	public override void Delete()
	{
		ClearSelections();

		EventManager.StopListening("MovePressed", MoveHandler);
		EventManager.StopListening("AttackPressed", AttackHandler);
		EventManager.StopListening("EndTurnPressed", EndTurnHandler);
		EventManager.StopListening("TileSelected", TileSelected);

		actionPanel.SetActive(false);

		base.Delete();
	}
}
