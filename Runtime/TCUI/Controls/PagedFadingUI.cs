using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace TCUtils {
    public class PagedFadingUI : MonoBehaviour {
        [SerializeField]
        protected Image imageRef;
        [SerializeField]
        protected Text mainTextRef;
        [SerializeField]
        protected Text textForImageRef;
        [SerializeField]
        protected CanvasGroup backgroundGroupRef;
        [SerializeField]
        protected CanvasGroup contentGroupRef;

        public float backgroundInTime = 1;
        public float backgroundOutTime = 1;
        public float inTime = 2;
        public float outTime = 1.5f;
        public float stayTime = 3;
        public bool autoContinue = true;
        public bool allowClickingForward = true;

        [System.Serializable]
        public struct PageContent {
            [TextArea]
            public string text;
            public Sprite image;
        }

        private Coroutine currentCoroutine = null;

        public event Action onShowingDone;

        public void ShowPageSequence(IEnumerable<PageContent> contents, bool startImmediately, bool endNotDisappear) {
            gameObject.SetActive(true);
            if (currentCoroutine != null)
                StopCoroutine(currentCoroutine);
            currentCoroutine = StartCoroutine(ShowPageSequenceCoroutine(contents, startImmediately, endNotDisappear));
        }

        protected IEnumerator ShowPageSequenceCoroutine(IEnumerable<PageContent> contents, bool startImmediately, bool endNotDisappear) {
            backgroundGroupRef.alpha = 0;
            contentGroupRef.alpha = 0;

            //background in
            float t = 0;
            while (t < backgroundInTime && !Input.GetMouseButtonUp(0) && !startImmediately) {
                backgroundGroupRef.alpha = t / backgroundInTime;
                t += Time.deltaTime;
                yield return null;
            }

            backgroundGroupRef.alpha = 1;

            var contentsArray = contents.ToArray();
            for (var i = 0; i < contentsArray.Length; ++i) {
                SetContent(contentsArray[i]);

                //showing up
                yield return null;
                contentGroupRef.alpha = 0;
                t = 0;
                while (t < inTime && !GetContinueInput()) {
                    contentGroupRef.alpha = t / inTime;
                    t += Time.deltaTime;
                    yield return null;
                }

                contentGroupRef.alpha = 1;

                //for for stay time
                yield return null;
                t = 0;
                while (t < stayTime && !GetContinueInput()) {
                    t += Time.deltaTime;
                    yield return null;
                }

                while (!autoContinue && !GetContinueInput()) {
                    yield return null;
                }

                //ending
                yield return null;
                t = 0;
                if (i != contentsArray.Length - 1 || !endNotDisappear) {
                    while (t < stayTime && !GetContinueInput()) {
                        contentGroupRef.alpha = 1 - t / inTime;
                        t += Time.deltaTime;
                        yield return null;
                    }

                    contentGroupRef.alpha = 0;
                }
            }

            //background out
            if (!endNotDisappear) {
                t = 0;
                while (t < backgroundOutTime && !GetContinueInput()) {
                    backgroundGroupRef.alpha = 1 - t / backgroundInTime;
                    t += Time.deltaTime;
                    yield return null;
                }

                backgroundGroupRef.alpha = 0;
                gameObject.SetActive(false);
            }

            onShowingDone?.Invoke();
        }

        protected bool GetContinueInput() {
            return Input.GetMouseButtonUp(0) && allowClickingForward;
        }

        protected void SetContent(PageContent content) {
            if (content.image != null) {
                imageRef.sprite = content.image;
                imageRef.enabled = true;
                textForImageRef.text = content.text;
                mainTextRef.text = null;
            } else {
                imageRef.enabled = false;
                imageRef.sprite = null;
                textForImageRef.text = null;
                mainTextRef.text = content.text;
            }
        }
    }
}