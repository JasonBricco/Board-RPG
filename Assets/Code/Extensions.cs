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

	public static GameObject FindChild(this GameObject obj, string name)
	{
		return obj.transform.Find(name).gameObject;
	}

	public static void SetXY(this Transform t, Vector3 newPos)
	{
		Vector3 pos = t.position;
		pos.x = newPos.x;
		pos.y = newPos.y;
		t.position = pos;
	}
}
