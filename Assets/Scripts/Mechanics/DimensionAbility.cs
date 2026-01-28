using UnityEngine;

using System.Collections;

using System.Collections.Generic;

using TMPro;



namespace DimensionGame.Mechanics

{

    public class DimensionAbility : MonoBehaviour // หรือ BaseSkill

    {

        [Header("Settings")]

        public KeyCode powerKey = KeyCode.R;

        public float range = 7f;

        public LayerMask dimensionLayer;

        public float cooldownTime = 1.0f;

        public string targetLayerName = "Default";



        [Header("Effect")]

        public Vector3 flatScaleMultiplier = new Vector3(1f, 0.001f, 1f);

        public float transitionSpeed = 8f;

        public float duration = 5f;



        [Header("Audio")]

        public AudioSource audioSource; // ตัวนี้ต้องได้รับมาจาก SkillSlot (เป็นตัวที่ 2 ของ Player)

        public AudioClip powerSFX;

        public AudioClip revertSFX;



        [Header("UI Interaction")]

        public CanvasGroup interactionCanvasGroup;

        public TextMeshProUGUI interactionText;    

        public string message = "Press [R]";



        private Dictionary<GameObject, Vector3> originalScales = new Dictionary<GameObject, Vector3>();

        private Dictionary<GameObject, int> originalLayers = new Dictionary<GameObject, int>();

        private Dictionary<GameObject, Vector3> originalColSizes = new Dictionary<GameObject, Vector3>();

        private Dictionary<GameObject, Coroutine> activeCoroutines = new Dictionary<GameObject, Coroutine>();

       

        private float _nextUseTime = 0f;

        private int _targetLayerIndex;

        private Camera _mainCam;

        private bool _canInteract = false;



        void Start() {

            _mainCam = Camera.main;

            _targetLayerIndex = LayerMask.NameToLayer(targetLayerName);

        }



        void Update() {

            CheckForInteractable();

            if (_canInteract && Input.GetKeyDown(powerKey) && Time.time >= _nextUseTime) {

                ExecutePower();

                _nextUseTime = Time.time + cooldownTime;

            }



            if (interactionCanvasGroup != null) {

                float targetAlpha = (_canInteract && Time.time >= _nextUseTime) ? 1f : 0f;

                interactionCanvasGroup.alpha = Mathf.MoveTowards(interactionCanvasGroup.alpha, targetAlpha, Time.deltaTime * 5f);

            }

        }



        private void CheckForInteractable() {

            Ray ray = _mainCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

            _canInteract = Physics.Raycast(ray, out RaycastHit hit, range, dimensionLayer);

        }



        private void ExecutePower() {

            Ray ray = _mainCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

            if (Physics.Raycast(ray, out RaycastHit hit, range, dimensionLayer)) {

                GameObject target = hit.collider.gameObject;

                if (activeCoroutines.ContainsKey(target)) StopCoroutine(activeCoroutines[target]);

                activeCoroutines[target] = StartCoroutine(ToggleDimension(target));

            }

        }



        private IEnumerator ToggleDimension(GameObject target) {

            if (!originalScales.ContainsKey(target)) originalScales[target] = target.transform.localScale;

            if (!originalLayers.ContainsKey(target)) originalLayers[target] = target.layer;

            BoxCollider boxCol = target.GetComponent<BoxCollider>();

            if (boxCol != null && !originalColSizes.ContainsKey(target)) originalColSizes[target] = boxCol.size;



            Vector3 startScale = target.transform.localScale;

            Vector3 endScale = Vector3.Scale(originalScales[target], flatScaleMultiplier);



            // --- เล่นเสียงย่อ (ใช้ PlayOneShot) ---

            if (audioSource && powerSFX) audioSource.PlayOneShot(powerSFX);

            target.layer = _targetLayerIndex;



            float t = 0;

            while (t < 1f) {

                if (target == null) yield break;

                t += Time.deltaTime * transitionSpeed;

                target.transform.localScale = Vector3.Lerp(startScale, endScale, t);

                if (boxCol != null) boxCol.size = Vector3.Lerp(boxCol.size, Vector3.Scale(originalColSizes[target], flatScaleMultiplier), t);

                yield return null;

            }



            yield return new WaitForSeconds(duration);



            // --- เล่นเสียงคืนร่าง (ใช้ PlayOneShot) ---

            if (audioSource && revertSFX) audioSource.PlayOneShot(revertSFX);

           

            t = 0;

            while (t < 1f) {

                if (target == null) yield break;

                t += Time.deltaTime * transitionSpeed;

                target.transform.localScale = Vector3.Lerp(endScale, originalScales[target], t);

                if (boxCol != null) boxCol.size = Vector3.Lerp(boxCol.size, originalColSizes[target], t);

                yield return null;

            }



            target.layer = originalLayers[target];

            activeCoroutines.Remove(target);

        }

    }

}