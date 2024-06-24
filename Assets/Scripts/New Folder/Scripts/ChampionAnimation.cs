using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 챔피언 애니메이션을 제어합니다.
/// </summary>
public class ChampionAnimation : MonoBehaviour
{
    private GameObject characterModel; // 캐릭터 모델
    private Animator animator; // 애니메이터
    private ChampionController championController; // 챔피언 컨트롤러

    private Vector3 lastFramePosition; // 마지막 프레임의 위치

    /// Start is called before the first frame update
    void Start()
    {
        // 캐릭터 모델 가져오기
        characterModel = this.transform.Find("character").gameObject;

        // 애니메이터 가져오기
        animator = characterModel.GetComponent<Animator>();
        championController = this.transform.GetComponent<ChampionController>();
    }

    /// Update is called once per frame
    void Update()
    {
        // 속도 계산
        float movementSpeed = (this.transform.position - lastFramePosition).magnitude / Time.deltaTime;

        // 애니메이터 컨트롤러에 이동 속도 설정
        animator.SetFloat("movementSpeed", movementSpeed);

        // 마지막 프레임 위치 저장
        lastFramePosition = this.transform.position;
    }

    /// <summary>
    /// 공격을 시작하거나 멈춥니다.
    /// </summary>
    /// <param name="b"></param>
    public void DoAttack(bool b)
    {
        animator.SetBool("isAttacking", b);
    }

    /// <summary>
    /// 공격 애니메이션이 끝날 때 호출됩니다.
    /// </summary>
    public void OnAttackAnimationFinished()
    {
        animator.SetBool("isAttacking", false);

        championController.OnAttackAnimationFinished();

        //Debug.Log("OnAttackAnimationFinished");

    }

    /// <summary>
    /// 애니메이션 상태를 설정합니다.
    /// </summary>
    /// <returns></returns>
    public void IsAnimated(bool b)
    {
        animator.enabled = b;
    }
}