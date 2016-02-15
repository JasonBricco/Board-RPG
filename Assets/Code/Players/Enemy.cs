//
//  Enemy.cs
//  BoardRPG
//
//  Created by Jason Bricco on 2/7/16.
//  Copyright © 2016 Jason Bricco. All rights reserved.
//

using UnityEngine;
using System.Collections;

public sealed class Enemy : Entity 
{
	public override void BeginTurn()
	{
		if (!beingDeleted)
			StartCoroutine(Move());
	}

	private IEnumerator Move()
	{
		yield return StartCoroutine(base.Move(GetDieRoll()));
		playerManager.NextTurn();
	}
}
