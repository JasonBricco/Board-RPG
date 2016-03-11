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

public sealed class ExtraMP : Card
{
	public ExtraMP(Sprite sprite) : base(sprite) {}

	public override void RunFunction(Entity entity)
	{
		entity.RemainingMP += 3;
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
	
public sealed class ExtraRandomMP : Card 
{
	public ExtraRandomMP(Sprite sprite) : base(sprite) {}

	public override void RunFunction(Entity entity)
	{
		entity.RemainingMP += Random.Range(1, 7);
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

public sealed class LoseMPCard : Card
{
	public LoseMPCard(Sprite sprite) : base(sprite) {}

	public override void RunFunction(Entity entity)
	{
		entity.AddPending(1, new Data(entity), DoEffect, true);
	}

	private void DoEffect(Data data)
	{
		data.entity.RemainingMP -= 2;
	}
}
