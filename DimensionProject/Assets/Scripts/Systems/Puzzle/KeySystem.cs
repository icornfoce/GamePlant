using UnityEngine;

public class KeyPickup : MonoBehaviour
{
    public enum KeyColor { Yellow, Black }
    
    [Header("Key Settings")]
    public KeyColor keyColor; // เลือกสีกุญแจใน Inspector
    public ScoreManager scoreManager; // ลาก ScoreManager มาใส่

    [Header("Audio Settings")]
    public AudioClip collectSound; 
    [Range(0, 1)] public float volume = 1f;

    private bool isPlayerNearby = false;

    void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E))
        {
            CollectKey();
        }
    }

    void CollectKey()
    {
        // 1. เล่นเสียง ณ จุดที่เก็บ
        if (collectSound != null)
        {
            AudioSource.PlayClipAtPoint(collectSound, transform.position, volume);
        }

        // 2. ส่งค่าไปที่ ScoreManager ตามสีที่เลือก
        if (scoreManager != null)
        {
            if (keyColor == KeyColor.Yellow)
            {
                scoreManager.SetYellowKey(true);
            }
            else if (keyColor == KeyColor.Black)
            {
                scoreManager.SetBlackKey(true);
            }
        }
        else
        {
            Debug.LogError("กรุณาลาก ScoreManager มาใส่ในสคริปต์กุญแจด้วย!");
        }

        // 3. ทำลายกุญแจทิ้ง
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
            Debug.Log("กด E เพื่อเก็บกุญแจสี " + keyColor);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
        }
    }
}