using UnityEngine;

public class BackFiveCard : Card
{
	public override void RunFunction(Entity entity)
	{
		entity.Flip(5, true);
	}
}
