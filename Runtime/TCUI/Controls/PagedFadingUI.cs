using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace TCUtils
{
    public class PagedFadingUI : MonoBehaviour
    {
        [SerializeField] protected Image imageRef;
        [SerializeField] protected Text mainTextRef;
        [SerializeField] protected Text textForImageRef;
        [SerializeField] protected CanvasGroup backgroundGroupRef;
        [SerializeField] protected CanvasGroup contentGroupRef;

        public float backgroundInTime = 1;
        public float backgroundOutTime = 1;
        public float inTime = 2;
        public float outTime = 1.5f;
        public float stayTime = 3;
        public bool autoContinue = true;
        public bool allowClickingForward = true;

        [System.Serializable]
        public struct PageContent
        {
            public string text;
            public Sprite image;
        }

        private Coroutine currentCoroutine = null;

        public event Action onShowingDone;

        public void ShowPageSequence(IEnumerable<PageContent> contents, bool startImmediately, bool endNotDisappear)
        {
            gameObject.SetActive(true);
            if (currentCoroutine != null)
                StopCoroutine(currentCoroutine);
            currentCoroutine = StartCoroutine(ShowPageSequenceCoroutine(contents, startImmediately, endNotDisappear));
        }

        protected IEnumerator ShowPageSequenceCoroutine(IEnumerable<PageContent> contents, bool startImmediately, bool endNotDisappear)
        {
            backgroundGroupRef.alpha = 0;
            contentGroupRef.alpha = 0;

            //background in
            float t = 0;
            while(t < backgroundInTime && !Input.GetMouseButtonUp(0) && !startImmediately)
            {
                backgroundGroupRef.alpha = t / backgroundInTime;
                t += Time.deltaTime;
                yield return null;
            }
            backgroundGroupRef.alpha = 1;

            
            foreach (var c in contents)
            {
                SetContent(c);

                //showing up
                yield return null;
                contentGroupRef.alpha = 0;
                t = 0;
                while (t < inTime && !Input.GetMouseButtonUp(0))
                {
                    contentGroupRef.alpha = t / inTime;
                    t += Time.deltaTime;
                    yield return null;
                }
                contentGroupRef.alpha = 1;

                //for for stay time
                yield return null;
                t = 0;
                while (t < stayTime && !Input.GetMouseButtonUp(0))
                {
                    t += Time.deltaTime;
                    yield return null;
                }

                while (!autoContinue && !Input.GetMouseButtonUp(0)) { yield return null; }

                //ending
                yield return null;
                t = 0;
                while (t < stayTime && !Input.GetMouseButtonUp(0))
                {
                    contentGroupRef.alpha = 1 - t / inTime;
                    t += Time.deltaTime;
                    yield return null;
                }
                contentGroupRef.alpha = 0;
            }

            //background out
            if (!endNotDisappear)
            {
                t = 0;
                while (t < backgroundOutTime && !Input.GetMouseButtonUp(0))
                {
                    backgroundGroupRef.alpha = 1 - t / backgroundInTime;
                    t += Time.deltaTime;
                    yield return null;
                }
                backgroundGroupRef.alpha = 0;
                gameObject.SetActive(false);
            }

            onShowingDone?.Invoke();
        }

        protected void SetContent(PageContent content)
        {
            if (content.image != null)
            {
                imageRef.sprite = content.image;
                imageRef.enabled = true;
                textForImageRef.text = content.text;
                mainTextRef.text = null;
            }
            else
            {
                imageRef.enabled = false;
                imageRef.sprite = null;
                textForImageRef.text = null;
                mainTextRef.text = content.text;
            }
        }
    }
}