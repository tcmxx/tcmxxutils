using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TCUtils
{
    public class SpriteButton : MonoBehaviour, IPointerDownHandler, IPointerClickHandler, IPointerUpHandler
    {
        [SerializeField]
        protected SpriteRenderer targetSprite;
        [SerializeField]
        protected TextMeshPro textMesh;

        public Color pressedColor = Color.gray;
        public Color normalColor = Color.white;
        public Color disabledColor = Color.gray;

        public UnityEvent onClicked;

        public bool Interactable { get => interactable;set { interactable = value; SetColors(interactable ? normalColor : pressedColor); } }
        private bool interactable = true;

        public bool interactableOnStart = true;

        private void Start()
        {
            Interactable = interactableOnStart;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if(Interactable)
                onClicked.Invoke();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if(Interactable)
                SetColors(pressedColor);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if(Interactable)
                SetColors(normalColor);
        }

        private void SetColors(Color col)
        {
            if (targetSprite != null)
                targetSprite.color = col;
            if (textMesh != null)
                textMesh.color = col;
        }
    }
}