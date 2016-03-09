using UnityEngine;

public class SwapFunction : Function 
{
	public override void Compute(string[] args, Entity entity)
	{
		EventManager.Notify("GetEntity", new Data(entity));
	}
}
