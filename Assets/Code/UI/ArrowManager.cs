//
//  ArrowManager.cs
//  BoardRPG
//
//  Created by Jason Bricco on 2/15/16.
//  Copyright © 2016 Jason Bricco. All rights reserved.
//

using UnityEngine;
using UnityEngine.UI;

public sealed class ArrowManager 
{
	public void EnableArrow(Vector2i direction)
	{
		if (direction.Equals(Vector2i.left)) UIManager.SetInteractable("LeftArrow", true);
		else if (direction.Equals(Vector2i.right)) UIManager.SetInteractable("RightArrow", true);
		else if (direction.Equals(Vector2i.up)) UIManager.SetInteractable("UpArrow", true);
		else if (direction.Equals(Vector2i.down)) UIManager.SetInteractable("DownArrow", true);
	}

	public void DisableArrows()
	{
		UIManager.SetInteractable("LeftArrow", false);
		UIManager.SetInteractable("RightArrow", false);
		UIManager.SetInteractable("UpArrow", false);
		UIManager.SetInteractable("DownArrow", false);
	}
}
