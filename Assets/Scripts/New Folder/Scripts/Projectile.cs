using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls Arrows, Spells movement from point A to B
/// </summary>
public class Projectile : MonoBehaviour
{
    ///발사체 이동속도
    public float speed;

    ///목표에 도달한 후 이 발사체가 파괴될 때까지 기다리는 시간
    public float duration;

    private GameObject target;

    private bool isMoving = false;

    /// <summary>
    /// 발사체가 생성될 때 호출
    /// </summary>
    /// <param name="_target"></param>
    public void Init(GameObject _target)
    {
        target = _target;
       
        isMoving = true;
    }

    /// Update is called once per frame
    void Update()
    {
      
        if (isMoving) // 발사체가 이동 중인지 확인합니다.
        {
            if (target == null) // 목표가 없으면 발사체를 파괴합니다.
            {
                Destroy(this.gameObject);
                return;
            }

            // 목표로 향하는 벡터를 계산합니다.
            Vector3 relativePos = target.transform.position - transform.position;

            // 목표 방향으로 회전합니다.
            Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);
            this.transform.rotation = rotation;

            // 목표 위치까지 이동합니다.
            Vector3 targetPosition = target.transform.position + new Vector3(0, 1, 0); // 목표 위치를 조정합니다.

            // 이동할 거리를 계산합니다.
            float step = speed * Time.deltaTime; // calculate distance to move
            transform.position = Vector3.MoveTowards(this.transform.position, targetPosition, step);

            // 목표에 도착한 경우
            float distance = Vector3.Distance(this.transform.position, targetPosition);

            if (distance < 0.2f)
            {
                // isMoving = false; // 이동 플래그를 비활성화합니다.

                this.transform.parent = target.transform;


                Destroy(this.gameObject, duration); // 지정된 시간 후에 발사체를 파괴합니다.
            }
           
        }
     

    }
}
