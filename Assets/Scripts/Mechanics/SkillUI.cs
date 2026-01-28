using UnityEngine;

namespace DimensionGame.UI
{
    public class SkillUI : MonoBehaviour
    {
        public GameObject uiPanel;
        public KeyCode toggleKey = KeyCode.Tab;
        private bool _isOpen = false;

        private void Start()
        {
            if (uiPanel != null) uiPanel.SetActive(false);
        }

        private void Update()
        {
            if (Input.GetKeyDown(toggleKey))
            {
                _isOpen = !_isOpen;
                uiPanel.SetActive(_isOpen);
                
                // จัดการเมาส์
                Cursor.lockState = _isOpen ? CursorLockMode.None : CursorLockMode.Locked;
                Cursor.visible = _isOpen;
            }
        }
    }
}