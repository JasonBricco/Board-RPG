using UnityEngine;

public sealed class UIEventReceiver : MonoBehaviour
{
	private string eventToCall;

	public void ButtonPressedPrimer(string buttonEvent)
	{
		eventToCall = buttonEvent;
	}

	public void ButtonPressed(int data)
	{
		EventManager.TriggerEvent(eventToCall, data);
	}
}
