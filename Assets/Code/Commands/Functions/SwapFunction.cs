using UnityEngine;

public class SwapFunction : Function 
{
	public SwapFunction(FunctionLibrary library) : base(library) {}

	public override void Compute(string[] args, Entity entity)
	{
		Entity other = entity.PlayerManager.GetEntity((entity.EntityID + 1) & 1);

		Vector3 otherPos = other.Position;
		other.SetTo(entity.Position);
		entity.SetTo(otherPos);
	}
}
