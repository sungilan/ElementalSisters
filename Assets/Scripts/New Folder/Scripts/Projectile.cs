using System.Collections;
using UnityEngine;

/// <summary>
/// Controls Arrows, Spells movement from point A to B
/// </summary>
public class Projectile : MonoBehaviour
{
    /// 발사체 이동속도
    public float speed;

    /// 목표에 도달한 후 이 발사체가 파괴될 때까지 기다리는 시간
    public float duration;

    /// 히트 이펙트가 생성되고 난 후 몇 초 후에 사라질지를 설정하는 변수
    public float hitEffectDuration;

    private GameObject target;
    [SerializeField] private GameObject hiteffectPrefab;
    private bool isMoving = false;
    private bool hasHit = false; // 목표 지점에 도달했는지 여부를 나타내는 플래그

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
        if (isMoving && !hasHit) // 발사체가 이동 중이고 목표 지점에 아직 도달하지 않았을 때만 업데이트 수행
        {
            if (target == null) // 목표가 없으면 발사체를 파괴합니다.
            {
                StartCoroutine(DestroyProjectile());
                return;
            }

            // 목표로 향하는 벡터를 계산합니다.
            Vector3 relativePos = target.transform.position - transform.position;

            // 목표 방향으로 회전합니다.
            Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);
            transform.rotation = rotation;

            // 목표 위치까지 이동합니다.
            Vector3 targetPosition = target.transform.position + Vector3.up; // 목표 위치를 조정합니다.

            // 이동할 거리를 계산합니다.
            float step = speed * Time.deltaTime; // calculate distance to move
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);

            // 목표에 도착한 경우
            float distance = Vector3.Distance(transform.position, targetPosition);
            if (distance < 0.2f)
            {
                hasHit = true; // 목표에 도달했음을 표시

                // 히트 이펙트 생성
                GameObject hiteffect = Instantiate(hiteffectPrefab, targetPosition, Quaternion.identity);
                // 히트 이펙트를 일정 시간 후에 삭제
                Destroy(hiteffect, hitEffectDuration);

                // 발사체 파괴
                StartCoroutine(DestroyProjectile());
            }
        }
    }

    // 발사체를 파괴하는 코루틴
    private IEnumerator DestroyProjectile()
    {
        yield return new WaitForSeconds(duration); // duration만큼 대기합니다.
        Destroy(gameObject); // 발사체를 파괴합니다.
    }
}