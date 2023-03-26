using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

namespace KOI
{
	public class DogWanderState : DogMovementState
	{
		public DogWanderState(Dog Dog) : base(Dog) { }

		public override void Tick()
		{
			if (_dog.CanAct())
			{
				bool directionFound = false;
				// if(GameStateManager.Instance.MapSystem.EatingFood(_dog.Position)){
				// 	return;
				// }
				List<Direction> escapeDirections = GameStateManager.Instance.MapSystem.DetermineEscapeDirections(_dog.Position, _dog.Attributes.Awareness);
				if(escapeDirections.Count == 0){ // if surrounded by predators in all directions
					_dog.Cooldown = 4;
					_dog.UpdateRenderDirection();
					return;
				}
			
				List<Direction> foodDirections = GameStateManager.Instance.MapSystem.DetermineFoodDirections(_dog.Position, _dog.Attributes.Awareness);
				Debug.Log("food directions: " + foodDirections);
				foreach( var x in foodDirections) {
					Debug.Log("fooddirection: " + x.ToString());
				}
			
				Direction newDirection = Utils.RandomValueFromList<Direction>(foodDirections.Count == 0 ? escapeDirections : escapeDirections.Intersect(foodDirections).ToList());
				Debug.Log("fooddirection newDirection: " + newDirection);
				do{
					// newDirection = Utils.RandomEnumValue<Direction>();
					newDirection = Utils.RandomValueFromList<Direction>(foodDirections.Count == 0 ? escapeDirections : escapeDirections.Intersect(foodDirections).ToList());
					if (GameStateManager.Instance.MapSystem.IsPassable(_dog.Position, newDirection)){
						directionFound = true;
					}
				}while(!directionFound);
				
				_dog.Direction = newDirection;
				_dog.Cooldown = MapConfig.DirectionCosts[newDirection];
				_dog.Position += MapConfig.DirectionVectors[newDirection];

				_dog.UpdateRenderPosition();
			}
		}
	}
}
