using UnityEngine;
namespace KOI
{
	public class DogAttributes: MonoBehaviour
	{
		public float Health { get; set; }
		public float Strength { get; set; }
		public float Speed { get; set; }
		public float Awareness { get; set; }
		public Vector2[] DirectionsToAvoid { get; set; }
	}
}
