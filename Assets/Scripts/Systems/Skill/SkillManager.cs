using UnityEngine;

namespace DimensionGame.Core
{
    public class SkillManager : MonoBehaviour
    {
        public static SkillManager Instance { get; private set; }
        public GameObject playerObject;
        private GameObject _currentSkillObject;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            if (playerObject == null) playerObject = gameObject;
        }

        // ชื่อฟังก์ชันต้องเป็นชื่อนี้ และรับ GameObject
        public void SelectSkillPrefab(GameObject skillPrefab)
        {
            if (playerObject == null || skillPrefab == null) return;

            if (_currentSkillObject != null) Destroy(_currentSkillObject);

            // สร้างก๊อปปี้จาก Prefab มาไว้ที่ตัว Player
            _currentSkillObject = Instantiate(skillPrefab, playerObject.transform);
            
            Debug.Log($"[Manager] ติดตั้งสกิล: {skillPrefab.name} พร้อมค่าดั้งเดิมเรียบร้อย");
        }

        public void SetSkillLayer(LayerMask newLayer)
        {
            // Support both prefab-child model and component-on-player model
            
            // 1. Check child object
            if (_currentSkillObject != null)
            {
                var ability = _currentSkillObject.GetComponent<DimensionGame.Mechanics.DimensionAbility>();
                if (ability != null)
                {
                    ability.dimensionLayer = newLayer;
                    Debug.Log($"[Manager] Changed Dimension Layer to: {newLayer.value} (Prefab)");
                    return;
                }
            }

            // 2. Check component on player
            if (playerObject != null)
            {
                var ability = playerObject.GetComponent<DimensionGame.Mechanics.DimensionAbility>();
                if (ability != null)
                {
                    ability.dimensionLayer = newLayer;
                    Debug.Log($"[Manager] Changed Dimension Layer to: {newLayer.value} (Component)");
                }
            }
        }

        public void SelectSkillByClassName(string className)
        {
            if (playerObject == null || string.IsNullOrEmpty(className)) return;

            // 1. Cleanup old prefab-based skill
            if (_currentSkillObject != null)
            {
                Destroy(_currentSkillObject);
                _currentSkillObject = null;
            }

            // 2. Cleanup old component-based skills (Assuming they inherit BaseSkill)
            var oldSkills = playerObject.GetComponents<BaseSkill>();
            foreach (var s in oldSkills)
            {
                Destroy(s);
            }

            // 3. Add new component
            System.Type type = System.Type.GetType(className);
            if (type != null)
            {
                playerObject.AddComponent(type);
                Debug.Log($"[Manager] Added component skill: {className}");
            }
            else
            {
                Debug.LogError($"[Manager] Class not found: {className}");
            }
        }
    }
}