using UnityEngine;
using UnityEngine.UI;

public class ButtonDataComponent : MonoBehaviour
{
	public string eventName;
	public int data;

	private void Start()
	{
		GetComponent<Button>().onClick.AddListener(SendEvent);
	}

	private void SendEvent()
	{
		EventManager.Notify(eventName, new Data(data));
	}
}
