using UnityEngine;

public class ContinuousRotation : MonoBehaviour
{
    public float rotationSpeed = 30f; // ȸ�� �ӵ� (�ʴ� ȸ�� ����)

    // Update is called once per frame
    void Update()
    {
        // ������Ʈ�� ȸ���մϴ�.
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }
}