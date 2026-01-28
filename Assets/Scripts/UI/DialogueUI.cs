using UnityEngine;
using TMPro;
using System.Collections;

namespace DimensionGame.UI
{
    public class DialogueUI : MonoBehaviour
    {
        [Header("UI References")]
        public CanvasGroup canvasGroup;
        public TextMeshProUGUI dialogueText;

        [Header("Settings")]
        public float fadeSpeed = 5f;

        private void Awake()
        {
            if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
            // Init as hidden
            if (canvasGroup != null) 
            {
                canvasGroup.alpha = 0f;
                canvasGroup.blocksRaycasts = false;
            }
        }

        public void ShowText(string text)
        {
            if (dialogueText != null) dialogueText.text = text;
            StopAllCoroutines();
            StartCoroutine(FadeUI(1f));
        }

        public void Hide()
        {
            StopAllCoroutines();
            StartCoroutine(FadeUI(0f));
        }

        private IEnumerator FadeUI(float targetAlpha)
        {
            if (canvasGroup == null) yield break;

            while (Mathf.Abs(canvasGroup.alpha - targetAlpha) > 0.01f)
            {
                canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, targetAlpha, Time.deltaTime * fadeSpeed);
                yield return null;
            }
            canvasGroup.alpha = targetAlpha;
            canvasGroup.blocksRaycasts = (targetAlpha > 0.5f);
        }
    }
}
