using UnityEngine;
using System.Collections.Generic;

public class Card
{
	public Sprite sprite;
	public bool allowed = true;

	public Card(Sprite sprite)
	{
		this.sprite = sprite;
	}

	public virtual void RunFunction(Entity entity)
	{
	}
}

public sealed class ForwardFiveCard : Card
{
	public ForwardFiveCard(Sprite sprite) : base(sprite) {}

	public override void RunFunction(Entity entity)
	{
		entity.RemainingMoves = 5;
	}
}

public sealed class FlipCard : Card
{
	public FlipCard(Sprite sprite) : base(sprite) {}

	public override void RunFunction(Entity entity)
	{
		entity.Flip();
	}
}

public sealed class SwapCard : Card 
{
	public SwapCard(Sprite sprite) : base(sprite) {}

	public override void RunFunction(Entity entity)
	{
		Entity other = entity.PlayerManager.GetEntity((entity.EntityID + 1) & 1);

		Vector3 otherPos = other.Position;
		other.SetTo(entity.Position);
		entity.SetTo(otherPos);
	}
}

public sealed class ExtraRollCard : Card 
{
	public ExtraRollCard(Sprite sprite) : base(sprite) {}

	public override void RunFunction(Entity entity)
	{
		entity.RemainingMoves = entity.GetDieRoll();
	}
}

public sealed class DoubleRollsCard : Card 
{
	public DoubleRollsCard(Sprite sprite) : base(sprite) {}

	public override void RunFunction(Entity entity)
	{
		entity.SetDiceMod(DoubledRoll);

		Data data = new Data();
		data.num0 = entity.EntityID;
		data.obj = entity.PlayerManager;

		entity.PlayerManager.WaitForTurns(entity.EntityID, 4, data, EndEffect);
	}

	private int DoubledRoll()
	{
		return Random.Range(2, 13);
	}

	private void EndEffect(Data data)
	{
		PlayerManager playerManager = (PlayerManager)data.obj;
		Entity entity = playerManager.GetEntity(data.num0);

		entity.SetDiceMod(null);
	}
}

public sealed class HalfRollsCard : Card 
{
	public HalfRollsCard(Sprite sprite) : base(sprite) {}

	public override void RunFunction(Entity entity)
	{
		entity.SetDiceMod(HalvedRoll);

		Data data = new Data();
		data.num0 = entity.EntityID;
		data.obj = entity.PlayerManager;

		entity.PlayerManager.WaitForTurns(entity.EntityID, 4, data, EndEffect);
	}

	private int HalvedRoll()
	{
		return Random.Range(1, 4);
	}

	private void EndEffect(Data data)
	{
		PlayerManager playerManager = (PlayerManager)data.obj;
		Entity entity = playerManager.GetEntity(data.num0);

		entity.SetDiceMod(null);
	}
}

public sealed class SkipTurnCard : Card 
{
	public SkipTurnCard(Sprite sprite) : base(sprite) {}

	public override void RunFunction(Entity entity)
	{
		entity.SkipNextTurn();
	}
}
