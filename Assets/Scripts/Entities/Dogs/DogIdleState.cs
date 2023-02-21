namespace KOI
{
	public class DogIdleState : DogMovementState
	{
		public DogIdleState(Dog Dog) : base(Dog) { }

		public override void Tick()
		{
			if (_dog.CanAct())
			{
				_dog.Cooldown = Utils.RandomRange(4, 16);
				_dog.Direction = Utils.RandomEnumValue<Direction>();

				_dog.UpdateRenderDirection();
			}
		}
	}
}
