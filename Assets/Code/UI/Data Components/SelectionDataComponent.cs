using UnityEngine;
using UnityEngine.UI;

public class SelectionDataComponent : MonoBehaviour 
{
	private void Start()
	{
		GetComponent<Button>().onClick.AddListener(SendEvent);
	}

	private void SendEvent()
	{
		EventManager.Notify("TileSelected", new Data(transform.position));
	}
}
