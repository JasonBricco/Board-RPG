using UnityEngine;
using System.Collections.Generic;

public class SceneItems : MonoBehaviour 
{
	private static Dictionary<string, GameObject> sceneItems = new Dictionary<string, GameObject>();

	private void Awake()
	{
		Transform[] items = gameObject.GetComponentsInChildren<Transform>(true);

		for (int i = 0; i < items.Length; i++)
		{
			GameObject go = items[i].gameObject;
			sceneItems.Add(go.name, go);
		}
	}

	public static GameObject GetItem(string name)
	{
		GameObject item;

		if (sceneItems.TryGetValue(name, out item))
			return item;
		else
		{
			Debug.LogError("Could not find a scene item with the name: " + name);
			return null;
		}
	}

	public static T GetItem<T>(string name)
	{
		return GetItem(name).GetComponent<T>();
	}
}
