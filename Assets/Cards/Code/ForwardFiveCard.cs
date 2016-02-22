using UnityEngine;

public class ForwardFiveCard : Card
{
	public override void RunFunction(Entity entity)
	{
		entity.RemainingMoves = 5;
	}
}
