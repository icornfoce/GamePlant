using UnityEngine;
using UnityEngine.Playables;

public class DoorController : MonoBehaviour
{
    public PlayableDirector openDirector;
    public PlayableDirector closeDirector;
    
    private bool isOpen = false;
    private bool isPlayerNearby = false;

    void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E))
        {
            // ตรวจสอบว่าไม่มี Timeline ตัวไหนกำลังเล่นอยู่ (เปิดทางให้กดได้เฉพาะตอนหยุดนิ่ง)
            if (openDirector.state != PlayState.Playing && closeDirector.state != PlayState.Playing)
            {
                ToggleDoor();
            }
        }
    }

    void ToggleDoor()
    {
        if (!isOpen)
        {
            openDirector.Play();
            isOpen = true;
        }
        else
        {
            closeDirector.Play();
            isOpen = false;
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