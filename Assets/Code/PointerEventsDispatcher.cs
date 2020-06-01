using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Code
{
    public sealed class PointerEventsDispatcher : MonoBehaviour, IPointerClickHandler, IPointerUpHandler, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public event Action<PointerEventData> onEnter;
        public event Action<PointerEventData> onExit;
        public event Action<PointerEventData> onClick;
        public event Action<PointerEventData> onDown;
        public event Action<PointerEventData> onUp;

        public void OnPointerEnter(PointerEventData eventData)
        {
            onEnter?.Invoke(eventData);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            onExit?.Invoke(eventData);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            onClick?.Invoke(eventData);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            onDown?.Invoke(eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            onUp?.Invoke(eventData);
        }
    }
}