using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public int waterCount = 0;
    public bool hasYellowKey = false;
    public bool hasBlackKey = false;

    // ระบบจัดการน้ำ
    public void drink_002(int amount)
    {
        waterCount += amount;
        Debug.Log("คลังน้ำปัจจุบัน: " + waterCount);
    }

    // ระบบจัดการกุญแจเหลือง
    public void SetYellowKey(bool state)
    {
        hasYellowKey = state;
        Debug.Log("กุญแจเหลือง: " + (state ? "เก็บแล้ว" : "ใช้ไปแล้ว"));
    }

    // ระบบจัดการกุญแจดำ
    public void SetBlackKey(bool state)
    {
        hasBlackKey = state;
        Debug.Log("กุญแจดำ: " + (state ? "เก็บแล้ว" : "ใช้ไปแล้ว"));
    }
}