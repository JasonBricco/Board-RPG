//
//  Utils.cs
//  BoardRPG
//
//  Created by Jason Bricco on 2/16/16.
//  Copyright © 2016 Jason Bricco. All rights reserved.
//

using UnityEngine;

public static class Utils 
{
	public static int RoundToNearest(float value, int nearest)
	{
		return nearest * ((int)(value + nearest * 0.5f) >> 5);
	}
}
		