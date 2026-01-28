using UnityEngine;
using TMPro; // อย่าลืมติดตั้ง TextMeshPro
using System.Collections;

public class InteractionDialogue : MonoBehaviour
{
    [Header("UI Settings")]
    public TextMeshProUGUI textUI;        // ลาก Text TMP มาใส่
    public GameObject textBackground;    // (ถ้ามี) พื้นหลังข้อความ

    [Header("Content")]
    [TextArea(3, 10)]
    public string message = "...";
    public AudioClip voiceSound;         // เสียงพูดหรือเสียงเอฟเฟกต์
    public float displayDuration = 3f;    // แสดงข้อความนานกี่วินาที

    private bool isPlayerNearby = false;
    private bool isDisplaying = false;

    void Start()
    {
        if (textUI != null) textUI.text = "";
        if (textBackground != null) textBackground.SetActive(false);
    }

    void Update()
    {
        // เมื่ออยู่ใกล้ และกด E และตอนนี้ข้อความยังไม่แสดงอยู่
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E) && !isDisplaying)
        {
            StartCoroutine(ShowDialogue());
        }
    }

    IEnumerator ShowDialogue()
    {
        isDisplaying = true;

        // 1. เล่นเสียง
        if (voiceSound != null)
        {
            AudioSource.PlayClipAtPoint(voiceSound, transform.position);
        }

        // 2. แสดงข้อความ
        if (textUI != null)
        {
            textUI.text = message;
            if (textBackground != null) textBackground.SetActive(true);
        }

        // 3. รอตามเวลาที่กำหนด
        yield return new WaitForSeconds(displayDuration);

        // 4. ลบข้อความออก
        if (textUI != null)
        {
            textUI.text = "";
            if (textBackground != null) textBackground.SetActive(false);
        }

        isDisplaying = false;
    }

    public void Interact()
    {
        if (!isDisplaying)
        {
            StartCoroutine(ShowDialogue());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) isPlayerNearby = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) isPlayerNearby = false;
    }
}