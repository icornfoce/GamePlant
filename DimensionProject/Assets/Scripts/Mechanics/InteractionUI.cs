using UnityEngine;
using TMPro;

namespace DimensionGame.UI
{
    public class InteractionUI : MonoBehaviour
    {
        public CanvasGroup canvasGroup;
        public TextMeshProUGUI promptText;

        private void Awake()
        {
            if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
            if (promptText == null && canvasGroup != null) promptText = canvasGroup.GetComponentInChildren<TextMeshProUGUI>();
        }

        public void SetPrompt(string text)
        {
            if (promptText != null) promptText.text = text;
        }

        public void SetVisible(bool visible)
        {
            if (canvasGroup != null)
            {
                // Simple alpha toggle (logic can be moved here from Ability if desired, 
                // but for now just exposing the properties is enough)
                // Actually DimensionAbility handles the fading logic in Update, so we just need this class as a handle.
            }
        }
    }
}
