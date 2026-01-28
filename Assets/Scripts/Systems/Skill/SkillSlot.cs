using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DimensionGame.Core;
using DimensionGame.Mechanics;

namespace DimensionGame.UI
{
    public class SkillSlot : MonoBehaviour
    {
        [Header("Skill Settings")]
        public string skillClassName = "DimensionGame.Mechanics.DimensionAbility";
        public LayerMask targetLayer;
        public float cooldownTime = 1.0f;
        public float range = 7f;
        public float duration = 5f;

        [Header("Skill Assets")]
        public AudioClip clickSFX;         // <--- เพิ่มช่องสำหรับใส่เสียงกดปุ่ม UI
        public AudioClip abilitySFX;       // เสียงตอนใช้พลัง
        public AudioClip abilityRevertSFX; // เสียงตอนคืนร่าง
        public string uiMessage = "Press [R]";
        
        [Header("UI References")]
        public CanvasGroup interactionCanvasGroup;
        public TMPro.TextMeshProUGUI interactionText;

        private AudioSource _buttonAudio;

        private void Start()
        {
            // เตรียม AudioSource สำหรับเสียงปุ่ม
            _buttonAudio = GetComponent<AudioSource>();
            if (_buttonAudio == null) 
            {
                _buttonAudio = gameObject.AddComponent<AudioSource>();
            }
            
            // ตั้งค่าพื้นฐานสำหรับเสียง UI
            _buttonAudio.playOnAwake = false;
            _buttonAudio.spatialBlend = 0f; // เป็นเสียง 2D

            GetComponent<Button>()?.onClick.AddListener(OnClicked);
        }

        private void OnClicked()
        {
            // 0. เล่นเสียงกดปุ่มทันที
            if (clickSFX != null && _buttonAudio != null)
            {
                // ใช้ PlayOneShot เพื่อให้เสียงเล่นซ้อนกันได้ถ้ากดรัวๆ และไม่ตัดเสียงเดิม
                _buttonAudio.PlayOneShot(clickSFX);
            }

            // 1. บอก SkillManager ให้เปลี่ยน Skill
            SkillManager.Instance.SelectSkillByClassName(skillClassName);
            
            // 2. เริ่มส่งค่า Assets ต่างๆ ไปยังตัวละคร
            StartCoroutine(ApplySettings());
        }

        private IEnumerator ApplySettings()
        {
            yield return null; 
            
            GameObject player = SkillManager.Instance.playerObject;
            if (player == null) yield break;

            DimensionAbility ability = player.GetComponent<DimensionAbility>();
            
            if (ability != null)
            {
                // จัดการเรื่อง AudioSource ของตัวละคร (แยกจากเสียงเดิน)
                AudioSource[] sources = player.GetComponents<AudioSource>();
                if (sources.Length > 1) 
                    ability.audioSource = sources[1];
                else 
                    ability.audioSource = player.AddComponent<AudioSource>(); 

                ability.audioSource.spatialBlend = 0f; 

                // ส่งค่า Config
                ability.powerSFX = abilitySFX;
                ability.revertSFX = abilityRevertSFX;
                ability.dimensionLayer = targetLayer;
                ability.cooldownTime = cooldownTime;
                ability.range = range;
                ability.duration = duration;
                ability.message = uiMessage;

                if (interactionCanvasGroup != null) ability.interactionCanvasGroup = interactionCanvasGroup;
                if (interactionText != null) ability.interactionText = interactionText;

                Debug.Log($"[SkillSlot] ติดตั้ง {skillClassName} พร้อมเสียงกด {clickSFX?.name}");
            }
        }
    }
}