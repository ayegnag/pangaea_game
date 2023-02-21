namespace KOI
{
	public abstract class DogMovementState
	{
		protected Dog _dog;

		public DogMovementState(Dog Dog)
		{
			_dog = Dog;
		}

		public abstract void Tick();
	}
}
