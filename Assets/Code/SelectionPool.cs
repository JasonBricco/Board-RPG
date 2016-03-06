using UnityEngine;
using System.Collections.Generic;

public sealed class SelectionPool
{
	private GameObject selection;
	private Queue<GameObject> selections = new Queue<GameObject>(128);

	public SelectionPool()
	{
		selection = Resources.Load<GameObject>("Prefabs/Selection");
	}

	public GameObject GetSelection()
	{
		if (selections.Count > 0)
		{
			GameObject selection = selections.Dequeue();
			selection.SetActive(true);

			return selection;
		}

		return GameObject.Instantiate(this.selection);
	}

	public void ReturnSelection(GameObject selection)
	{
		selection.SetActive(false);
		selections.Enqueue(selection);
	}
}
