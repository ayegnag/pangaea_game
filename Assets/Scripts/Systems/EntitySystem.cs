using System;
using System.Collections.Generic;
using UnityEngine;

namespace KOI
{
	public class EntitySystem : GameSystem
	{
		public static event EventHandler<OnDogEventArgs> OnCreateDog;

		private List<Dog> _dogList;

		public override void Init()
		{
			SetupEvents();
			_dogList = new List<Dog>(EntityConfig.TotalEntities);
			CreateDogs();
			CreateDeers();
		}

		private void SetupEvents()
		{
			GameStateManager.OnTick += Tick;
			// Interface.OnUpdateMovementState += UpdateMovementState;
		}

		private void CreateDogs()
		{
			for (int i = 0; i < EntityConfig.TotalDogs; i++)
			{
				var newDog = new Dog()
				{   
					EntityType = EntityType.Dog,	// Segregate this logic into how the entities are chosen
					Direction = Utils.RandomEnumValue<Direction>(),
					Position = GameStateManager.Instance.MapSystem.GetOpenCellPosition()
				};

				_dogList.Add(newDog);

				OnCreateDog?.Invoke(this, new OnDogEventArgs { Dog = newDog });
			}
		}
		private void CreateDeers()
		{
			for (int i = 0; i < EntityConfig.TotalDeers; i++)
			{
				var newDog = new Dog()
				{   
					EntityType = EntityType.Deer,
					Direction = Utils.RandomEnumValue<Direction>(),
					Position = GameStateManager.Instance.MapSystem.GetOpenCellPosition()
				};

				_dogList.Add(newDog);

				OnCreateDog?.Invoke(this, new OnDogEventArgs { Dog = newDog });
			}
		}

		// EntitySytem Receives Tick Event from the GameStageManager and passes that tick event call to all Dogs in the list.
		protected override void Tick(object sender, OnTickArgs eventArgs)
		{
			foreach (Dog dog in _dogList)
			{
				dog.Tick();
			}
		}

		public override void Quit()
		{
			GameStateManager.OnTick -= Tick;

			// Interface.OnUpdateMovementState -= UpdateMovementState;
		}

		private void UpdateMovementState(object sender, OnUpdateDogMovementStateArgs eventArgs)
		{
			foreach (Dog dog in _dogList)
			{
				dog.SetMovementState(eventArgs.DogMovementStateType);
			}
		}

		public DogAttributes GetDogAttributes(int dogId)
		{
			// Debug.Log($"[{string.Join(",", _dogList)}]");
			int num = dogId - 1;
			return _dogList[num].Attributes;
		}
	}
}
