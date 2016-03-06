using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public sealed class Player : Entity
{
	private GameObject actionPanel;

	private SelectionPool selectedPool = new SelectionPool();

	private void Start()
	{
		actionPanel = UIStore.GetGraphic("ActionPanel");

		EventManager.StartListening("MovePressed", MoveHandler);
		EventManager.StartListening("AttackPressed", AttackHandler);
		EventManager.StartListening("PassPressed", PassHandler);
	}

	public override void BeginTurn()
	{
		if (skipTurn) 
		{
			skipTurn = false;
			map.NextTurn();
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
		map.NextTurn();
	}
		
	private void AttackHandler(int data)
	{
	}

	private void PassHandler(int data)
	{
		actionPanel.SetActive(false);
		map.NextTurn();
	}

	public override void Delete()
	{
		EventManager.StopListening("MovePressed", MoveHandler);
		EventManager.StopListening("AttackPressed", AttackHandler);
		EventManager.StopListening("PassPressed", PassHandler);

		actionPanel.SetActive(false);

		base.Delete();
	}
}
