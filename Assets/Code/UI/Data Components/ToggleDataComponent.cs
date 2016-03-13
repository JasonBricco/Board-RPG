using UnityEngine;
using UnityEngine.UI;

public class ToggleDataComponent : MonoBehaviour
{
	public string eventName;
	public int data;

	private void Start()
	{
		GetComponent<Toggle>().onValueChanged.AddListener(SendEvent);
	}

	private void SendEvent(bool on)
	{
		EventManager.Notify(eventName, new Data(data));
	}
}
