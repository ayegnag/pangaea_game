namespace KOI
{
	public class DogWanderState : DogMovementState
	{
		public DogWanderState(Dog Dog) : base(Dog) { }

		public override void Tick()
		{
			if (_dog.CanAct())
			{
				Direction newDirection = Utils.RandomEnumValue<Direction>();

				_dog.Direction = newDirection;

				if (GameStateManager.Instance.MapSystem.IsPassable(_dog.Position, newDirection))
				{
					_dog.Cooldown = MapConfig.DirectionCosts[newDirection];
					_dog.Position += MapConfig.DirectionVectors[newDirection];

					_dog.UpdateRenderPosition();
				}
				else
				{
					_dog.Cooldown = 4;

					_dog.UpdateRenderDirection();
				}
			}
		}
	}
}
