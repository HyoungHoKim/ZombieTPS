using UnityEngine;

public class Rotator : MonoBehaviour
{
    public float rotationSpeed = 60f; // 초당 회전 속도

    private void Update()
    {
        // y축으로 1초에 60도 회전
        transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);
    }
}