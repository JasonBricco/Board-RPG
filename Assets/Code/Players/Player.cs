using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public sealed class Player : Entity
{
	private GameObject actionPanel;
	private GameObject directionArrows;

	private Button leftArrow, rightArrow, upArrow, downArrow;

	private void Start()
	{
		actionPanel = UIStore.GetGraphic("ActionPanel");

		directionArrows = UIStore.GetGraphic("DirectionArrows");
		leftArrow = directionArrows.FindChild("LeftArrow").GetComponent<Button>();
		rightArrow = directionArrows.FindChild("RightArrow").GetComponent<Button>();
		upArrow = directionArrows.FindChild("UpArrow").GetComponent<Button>();
		downArrow = directionArrows.FindChild("DownArrow").GetComponent<Button>();

		EventManager.StartListening("MovePressed", MoveHandler);
		EventManager.StartListening("AttackPressed", AttackHandler);
		EventManager.StartListening("PassPressed", PassHandler);
		EventManager.StartListening("DirectionArrowPressed", DirectionArrowPressed);
	}

	public override void BeginTurn()
	{
		if (skipTurn) 
		{
			skipTurn = false;
			boardManager.NextTurn();
		}
		else
		{
			if (!beingDeleted)
				actionPanel.SetActive(true);
		}
	}

	private void MoveHandler(int data)
	{
		actionPanel.SetActive(false);
		RemainingMoves = GetDieRoll();
		StartCoroutine(GetDirectionAndMove());
	}

	private IEnumerator GetDirectionAndMove()
	{
		while (RemainingMoves > 0)
		{
			Vector2i current = new Vector2i(transform.position);
			Vector2i dir;

			if (forcedDirections.Count > 0)
				dir = forcedDirections.Dequeue();
			else
			{
				if (!GetMoveDirection(current, out dir))
					yield break;
			}
				
			yield return StartCoroutine(Move(dir, current));
		}
			
		boardManager.NextTurn();
	}

	private void AttackHandler(int data)
	{
	}

	private void PassHandler(int data)
	{
		actionPanel.SetActive(false);
		boardManager.NextTurn();
	}

	public override bool HandleSplitPath(out Vector2i dir)
	{
		dir = Vector2i.zero;

		DisableArrows();

		for (int i = 0; i < possibleMoves.Count; i++)
			EnableArrow(possibleMoves[i]);

		directionArrows.SetActive(true);
		return false;
	}

	private void DirectionArrowPressed(int direction)
	{
		switch ((Direction)direction)
		{
		case Direction.Left:
			forcedDirections.Enqueue(Vector2i.left);
			break;

		case Direction.Right:
			forcedDirections.Enqueue(Vector2i.right);
			break;

		case Direction.Up:
			forcedDirections.Enqueue(Vector2i.up);
			break;

		case Direction.Down:
			forcedDirections.Enqueue(Vector2i.down);
			break;
		}

		directionArrows.SetActive(false);
		StartCoroutine(GetDirectionAndMove());
	}

	public void EnableArrow(Vector2i direction)
	{
		if (direction.Equals(Vector2i.left)) leftArrow.interactable = true;
		else if (direction.Equals(Vector2i.right)) rightArrow.interactable = true;
		else if (direction.Equals(Vector2i.up)) upArrow.interactable = true;
		else if (direction.Equals(Vector2i.down)) downArrow.interactable = true;
	}

	public void DisableArrows()
	{
		leftArrow.interactable = false;
		rightArrow.interactable = false;
		upArrow.interactable = false;
		downArrow.interactable = false;
	}

	public override void Delete()
	{
		EventManager.StopListening("MovePressed", MoveHandler);
		EventManager.StopListening("AttackPressed", AttackHandler);
		EventManager.StopListening("PassPressed", PassHandler);
		EventManager.StopListening("DirectionArrowPressed", DirectionArrowPressed);

		actionPanel.SetActive(false);
		directionArrows.SetActive(false);

		base.Delete();
	}
}
