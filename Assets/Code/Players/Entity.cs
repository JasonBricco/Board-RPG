//
//  Entity.cs
//  BoardRPG
//
//  Created by Jason Bricco on 2/6/16.
//  Copyright © 2016 Jason Bricco. All rights reserved.
//

using UnityEngine;

public class Entity : MonoBehaviour
{
	public void Move(Vector3 translation)
	{
		transform.Translate(translation);
	}

	public void SetTo(Vector3 position)
	{
		transform.position = position;
	}
}
