using UnityEngine;
using System.Collections.Generic;

public sealed class SelectionPool
{
	private GameObject selectionPrefab;
	private Queue<GameObject> selections = new Queue<GameObject>(128);

	public SelectionPool()
	{
		selectionPrefab = Resources.Load<GameObject>("Prefabs/Selection");
	}

	public GameObject GetSelection()
	{
		if (selections.Count > 0)
		{
			GameObject selection = selections.Dequeue();
			selection.SetActive(true);

			return selection;
		}

		return GameObject.Instantiate(this.selectionPrefab);
	}

	public void ReturnSelection(GameObject selection)
	{
		selection.SetActive(false);
		selections.Enqueue(selection);
	}
}
