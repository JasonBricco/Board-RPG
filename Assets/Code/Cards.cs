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
		// TODO
	}
}

public sealed class SwapCard : Card 
{
	public SwapCard(Sprite sprite) : base(sprite) {}

	public override void RunFunction(Entity entity)
	{
		EventManager.Notify("SwapEntities", new Data(entity));
	}
}
	
public sealed class ExtraRollCard : Card 
{
	public ExtraRollCard(Sprite sprite) : base(sprite) {}

	public override void RunFunction(Entity entity)
	{
		// TODO
	}
}

public sealed class DoubleRollsCard : Card 
{
	public DoubleRollsCard(Sprite sprite) : base(sprite) {}

	public override void RunFunction(Entity entity)
	{
		// TODO
	}
}

public sealed class HalfRollsCard : Card 
{
	public HalfRollsCard(Sprite sprite) : base(sprite) {}

	public override void RunFunction(Entity entity)
	{
		// TODO
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
