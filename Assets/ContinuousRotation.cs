using UnityEngine;

public class ContinuousRotation : MonoBehaviour
{
    public float rotationSpeed = 30f; // 회전 속도 (초당 회전 각도)

    // Update is called once per frame
    void Update()
    {
        // 오브젝트를 회전합니다.
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }
}