using UnityEngine;

public sealed class UIEventReceiver : MonoBehaviour
{
	private string eventToCall;

	public void ButtonPressed(string buttonEvent)
	{
		eventToCall = buttonEvent;
	}

	public void Toggled(string toggleEvent)
	{
		eventToCall = toggleEvent;
	}

	public void SendEvent(int data)
	{
		EventManager.TriggerEvent(eventToCall, data);
	}
}
