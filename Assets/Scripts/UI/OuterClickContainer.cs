using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace KOI {
    public class OuterClickContainer : MonoBehaviour, IPointerDownHandler
    {
        public static event EventHandler<EventArgs> OnMouseDownOutsideEvent;

        public void OnPointerDown(PointerEventData eventData)
        {
            OnMouseDownOutsideEvent?.Invoke(this, null);
        }
    }
}