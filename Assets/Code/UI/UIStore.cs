using UnityEngine;
using System.Collections.Generic;

public class UIStore : MonoBehaviour 
{
	[SerializeField] private GameObject[] graphicObjects;
	private static Dictionary<string, GameObject> graphics = new Dictionary<string, GameObject>();

	private void Awake()
	{
		for (int i = 0; i < graphicObjects.Length; i++)
			graphics.Add(graphicObjects[i].name, graphicObjects[i]);
	}

	public static GameObject GetGraphic(string name)
	{
		GameObject graphic;

		if (graphics.TryGetValue(name, out graphic))
			return graphic;
		else
		{
			Debug.LogError("Could not find a graphic with the name: " + name);
			return null;
		}
	}

	public static T GetGraphic<T>(string name)
	{
		return GetGraphic(name).GetComponent<T>();
	}
}
