//
//  ArrowManager.cs
//  BoardRPG
//
//  Created by Jason Bricco on 2/15/16.
//  Copyright © 2016 Jason Bricco. All rights reserved.
//

using UnityEngine;
using UnityEngine.UI;

public sealed class ArrowManager : MonoBehaviour 
{
	[SerializeField] private Button left;
	[SerializeField] private Button right;
	[SerializeField] private Button up;
	[SerializeField] private Button down;

	public void EnableArrow(Vector2i direction)
	{
		if (direction.Equals(Vector2i.left)) left.interactable = true;
		else if (direction.Equals(Vector2i.right)) right.interactable = true;
		else if (direction.Equals(Vector2i.up)) up.interactable = true;
		else if (direction.Equals(Vector2i.down)) down.interactable = true;
	}

	public void DisableArrows()
	{
		left.interactable = false;
		right.interactable = false;
		up.interactable = false;
		down.interactable = false;
	}
}
