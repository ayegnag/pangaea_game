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

			CreateDogs();
		}

		private void SetupEvents()
		{
			GameStateManager.OnTick += Tick;

			// Interface.OnUpdateMovementState += UpdateMovementState;
		}

		private void CreateDogs()
		{
			_dogList = new List<Dog>(EntityConfig.TotalDogs);

			for (int i = 0; i < EntityConfig.TotalDogs; i++)
			{
				var newDog = new Dog()
				{   
					Pack = Utils.RandomEnumValue<Pack>(),
					Direction = Utils.RandomEnumValue<Direction>(),
					Position = GameStateManager.Instance.MapSystem.GetOpenCellPosition()
				};

				_dogList.Add(newDog);

				OnCreateDog?.Invoke(this, new OnDogEventArgs { Dog = newDog });
			}
		}

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
	}
}
