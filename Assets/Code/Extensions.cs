using UnityEngine;
using System;

public static class Extensions
{
	public static GameObject AddChild(this GameObject obj, string name, params Type[] types)
	{
		GameObject newObj = new GameObject(name, types);
		newObj.transform.SetParent(obj.transform);

		return newObj;
	}
}
