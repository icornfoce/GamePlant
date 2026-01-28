using UnityEngine;
using UnityEngine.Playables; 
using TMPro; // เพิ่มเข้ามาเพื่อใช้งาน TextMeshPro
using System.Collections;

public class Unlock : MonoBehaviour
{
    public enum KeyColor { Yellow, Black }
    public KeyColor requiredKey; 

    [Header("References")]
    public ScoreManager scoreManager; 
    public PlayableDirector openDoorTimeline; 
    
    [Header("UI Dialogue Settings")]
    public GameObject dialoguePanel;   // ลาก Panel UI มาใส่
    public TextMeshProUGUI dialogueText; // ลาก Text TMP มาใส่
    public float displayDuration = 3f;

    [Header("Puzzle Status")]
    public bool isLightShining = false; 

    [Header("Audio Settings")]
    public AudioClip lockFailSound; 
    public AudioClip unlockSuccessSound; 

    private bool isPlayerNearby = false;
    private bool isOpened = false;
    private Coroutine dialogueCoroutine;

    void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E) && !isOpened)
        {
            if (!isLightShining || !HasRequiredKey())
            {
                PlayFailSound();
                
                // เปลี่ยน Debug.Log เป็น ShowDialogue
                if (!isLightShining)
                {
                    ShowDialogue("It looks like something's stuck there try shining some light on it.");
                }
                else
                {
                    string colorName = (requiredKey == KeyColor.Yellow) ? "yellow" : "black";
                    ShowDialogue("Looks like required is " + colorName + " key");
                }
            }
            else 
            {
                OpenDoor();
            }
        }
    }

    bool HasRequiredKey()
    {
        if (requiredKey == KeyColor.Yellow) return scoreManager.hasYellowKey;
        if (requiredKey == KeyColor.Black) return scoreManager.hasBlackKey;
        return false;
    }

    void PlayFailSound()
    {
        if (lockFailSound != null)
        {
            AudioSource.PlayClipAtPoint(lockFailSound, transform.position);
        }
    }

    void OpenDoor()
    {
        isOpened = true;

        if (unlockSuccessSound != null)
        {
            AudioSource.PlayClipAtPoint(unlockSuccessSound, transform.position);
        }

        if (openDoorTimeline != null)
        {
            openDoorTimeline.Play();
        }

        if (requiredKey == KeyColor.Yellow) scoreManager.SetYellowKey(false);
        else scoreManager.SetBlackKey(false);

        ShowDialogue("What is that!!");
    }

    // ฟังก์ชันแสดงข้อความบน UI
    public void ShowDialogue(string message)
    {
        if (dialogueCoroutine != null)
        {
            StopCoroutine(dialogueCoroutine);
        }
        dialogueCoroutine = StartCoroutine(DisplayRoutine(message));
    }

    IEnumerator DisplayRoutine(string message)
    {
        if (dialoguePanel != null && dialogueText != null)
        {
            dialoguePanel.SetActive(true);
            dialogueText.text = message;

            yield return new WaitForSeconds(displayDuration);

            dialoguePanel.SetActive(false);
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