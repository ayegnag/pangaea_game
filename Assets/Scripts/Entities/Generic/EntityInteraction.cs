using System;
using UnityEngine;

namespace KOI
{
    public class EntityInteraction : MonoBehaviour {
        
		public static event EventHandler<OnDogMouseEventArgs> OnMouseEnterEvent;
		public static event EventHandler<OnDogMouseEventArgs> OnMouseExitEvent;
		public static event EventHandler<OnDogMouseEventArgs> OnMouseDownEvent;

        private BoxCollider2D pointerCollider;

        private void Awake()
		{
        }

        private void Start() {
            pointerCollider = GetComponent<BoxCollider2D>();
        }

        private void OnMouseDown() {
            OnMouseDownEvent?.Invoke(this, new OnDogMouseEventArgs { Name = pointerCollider.name });
        }
        
        private void OnMouseOver() {
            OnMouseEnterEvent?.Invoke(this, new OnDogMouseEventArgs { Name = pointerCollider.name });
        }
        
        private void OnMouseExit() {
            OnMouseExitEvent?.Invoke(this, new OnDogMouseEventArgs { Name = pointerCollider.name });
        }
 
	}
}
