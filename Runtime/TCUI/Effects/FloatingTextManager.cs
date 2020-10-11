using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

namespace TCUtils
{
    public class FloatingTextManager : MonoBehaviour
    {
        [SerializeField] protected Text textPrefab;
        [SerializeField] protected Canvas canvas;

        public float defaultMoveTime = 1.5f;
        public float defaultFadeTimeFactor = 0.2f;
        public Vector2 defaultVelocity = Vector3.up*100;

        private ComponentPool<Text> pool = new ComponentPool<Text>();

        private void Awake()
        {
            pool.Prefab = textPrefab;
        }

        private void OnDestroy()
        {
            pool.Clear();
        }

        public void ShowText(Vector3 worldPosition, Camera worldCamera, string text)
        {
            ShowText(worldPosition, worldCamera, text, textPrefab.color, defaultMoveTime, defaultVelocity, defaultFadeTimeFactor);
        }

        public void ShowText(Vector3 worldPosition, Camera worldCamera, string text, Color textColor, float moveTime, Vector3 moveVelocity, float fadeTimeFactor)
        {
            var textUI = GetTextObject();
            textUI.text = text;
            textUI.color = textColor;

            Vector3 startPOs = WorldToScreenCanvasPos(worldCamera, worldPosition);
            textUI.transform.localPosition = startPOs;

            Vector3 endPos = startPOs + moveVelocity * moveTime;

            Sequence sequenceAlpha = DOTween.Sequence();
            sequenceAlpha.AppendInterval(moveTime * (1 - fadeTimeFactor)).Append(textUI.DOFade(0, moveTime * fadeTimeFactor));

            var moveTween = textUI.transform.DOLocalMove(endPos, moveTime).onComplete += () => ReleaseTextObject(textUI);
        }

        private Text GetTextObject()
        {
            var text = pool.Get();
            text.transform.SetParent(transform);
            text.transform.localScale = Vector3.one;
            return text;
        }

        private void ReleaseTextObject(Text obj)
        {
            pool.Release(obj);
        }

        private Vector2 WorldToScreenCanvasPos(Camera objectWorldCamera, Vector3 worldPosition)
        {
            Vector2 pos;
            var canvasCam = canvas.worldCamera;
            if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
                canvasCam = null;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent as RectTransform,
                RectTransformUtility.WorldToScreenPoint(objectWorldCamera, worldPosition),
                canvasCam,
                out pos);
            return pos;
        }
    }
}