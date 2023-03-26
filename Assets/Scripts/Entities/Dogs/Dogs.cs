using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

namespace KOI
{
	public class Dog: MonoBehaviour
	{
		private static int _nextDogId = 1;

		public static event EventHandler<OnDogEventArgs> OnUpdateDogRenderDirection;
		public static event EventHandler<OnDogEventArgs> OnUpdateDogRenderPosition;

		public int Id { get; private set; }
		public Direction Direction { get; set; }
		public int2 Position { get; set; }
		public Pack Pack { get; set; }

		public int Cooldown { get; set; }

		public DogAttributes Attributes { get; set; }

		private readonly Dictionary<DogMovementStateType, DogMovementState> _movementStates;

		private DogMovementState _currentMovementState;

		public Dog()
		{
			Id = _nextDogId++;

			Cooldown = Utils.RandomRange(2, 8);
			Attributes = new DogAttributes
			{
				Health = 1,
				Strength = 1,
				Speed = 1,
				Awareness = 5,
				DirectionsToAvoid = new Vector2[]{new Vector2(0,1),new Vector2(1,0)}
			};

			_movementStates = new Dictionary<DogMovementStateType, DogMovementState>
			{
				[DogMovementStateType.Idle] = new DogIdleState(this),
				[DogMovementStateType.Wander] = new DogWanderState(this)
			};

			_currentMovementState = _movementStates[DogMovementStateType.Wander];
		}

		public void Tick()
		{
			Cooldown--;

			_currentMovementState.Tick();
		}

		public bool CanAct()
		{
			return Cooldown <= 0;
		}

		public void SetMovementState(DogMovementStateType DogMovementStateType)
		{
			_currentMovementState = _movementStates[DogMovementStateType];
		}

		public void UpdateRenderDirection()
		{
			OnUpdateDogRenderDirection?.Invoke(this, new OnDogEventArgs { Dog = this });
		}

		public void UpdateRenderPosition()
		{
			OnUpdateDogRenderPosition?.Invoke(this, new OnDogEventArgs { Dog = this });
		}
	}
}
