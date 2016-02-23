using UnityEngine;

public class FlipCard : Card
{
	public override void RunFunction(Entity entity)
	{
		entity.Flip();
	}
}
