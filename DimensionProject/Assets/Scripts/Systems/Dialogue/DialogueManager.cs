using UnityEngine;
using UnityEngine.Playables;
using TMPro;

public class TimelineTrigger : MonoBehaviour
{
    public enum TriggerMode { OnTriggerEnter, OnKeyPress }

    [Header("Mode Settings")]
    public TriggerMode mode = TriggerMode.OnTriggerEnter;
    public KeyCode interactKey = KeyCode.E;
    public float interactRange = 3f;

    [Header("Timeline Settings")]
    public PlayableDirector timeline;
    public bool playOnlyOnce = true;
    
    [Header("UI Interaction")]
    public CanvasGroup interactionCanvasGroup; 
    public TextMeshProUGUI interactionText;    
    public string message = "Press [E] to Watch";

    private Transform _player;
    private bool _hasPlayed = false;
    private bool _inRange = false;

    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) _player = playerObj.transform;

        if (interactionCanvasGroup != null) interactionCanvasGroup.alpha = 0;
    }

    private void Update()
    {
        if (timeline == null || (_hasPlayed && playOnlyOnce)) return;

        // โหมดกดปุ่ม
        if (mode == TriggerMode.OnKeyPress && _player != null)
        {
            float distance = Vector3.Distance(transform.position, _player.position);
            _inRange = distance <= interactRange;

            // จัดการ UI
            if (interactionCanvasGroup != null)
            {
                float targetAlpha = _inRange ? 1f : 0f;
                interactionCanvasGroup.alpha = Mathf.MoveTowards(interactionCanvasGroup.alpha, targetAlpha, Time.deltaTime * 5f);
                if (_inRange && interactionText != null) interactionText.text = message;
            }

            // เช็คการกดปุ่ม
            if (_inRange && Input.GetKeyDown(interactKey))
            {
                PlayTimeline();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // โหมดเดินชน
        if (mode == TriggerMode.OnTriggerEnter && other.CompareTag("Player"))
        {
            PlayTimeline();
        }
    }

    private void PlayTimeline()
    {
        if (playOnlyOnce && _hasPlayed) return;

        if (timeline != null)
        {
            timeline.Play();
            _hasPlayed = true;
            
            // ปิด UI ทันทีเมื่อเล่น
            if (interactionCanvasGroup != null) interactionCanvasGroup.alpha = 0;
            
            Debug.Log($"[Timeline] Playing: {timeline.name}");
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (mode == TriggerMode.OnKeyPress)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, interactRange);
        }
    }
}