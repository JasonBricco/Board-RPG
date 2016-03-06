using UnityEngine;
using System.Collections.Generic;

public class Card
{
	protected Map boardManager;
	public Sprite sprite;
	public bool allowed = true;

	public Card(Sprite sprite, Map boardManager)
	{
		this.sprite = sprite;
		this.boardManager = boardManager;
	}

	public virtual void RunFunction(Entity entity)
	{
	}
}

public sealed class ForwardFiveCard : Card
{
	public ForwardFiveCard(Sprite sprite, Map manager) : base(sprite, manager) {}

	public override void RunFunction(Entity entity)
	{
		// TODO
	}
}

public sealed class SwapCard : Card 
{
	public SwapCard(Sprite sprite, Map manager) : base(sprite, manager) {}

	public override void RunFunction(Entity entity)
	{
		Entity other = boardManager.GetEntity((entity.EntityID + 1) & 1);

		Vector3 otherPos = other.Position;
		other.SetTo(entity.Position);
		entity.SetTo(otherPos);
	}
}

public sealed class ExtraRollCard : Card 
{
	public ExtraRollCard(Sprite sprite, Map manager) : base(sprite, manager) {}

	public override void RunFunction(Entity entity)
	{
		// TODO
	}
}

public sealed class DoubleRollsCard : Card 
{
	public DoubleRollsCard(Sprite sprite, Map manager) : base(sprite, manager) {}

	public override void RunFunction(Entity entity)
	{
		// TODO
	}
}

public sealed class HalfRollsCard : Card 
{
	public HalfRollsCard(Sprite sprite, Map manager) : base(sprite, manager) {}

	public override void RunFunction(Entity entity)
	{
		// TODO
	}
}

public sealed class SkipTurnCard : Card 
{
	public SkipTurnCard(Sprite sprite, Map manager) : base(sprite, manager) {}

	public override void RunFunction(Entity entity)
	{
		entity.SkipNextTurn();
	}
}
