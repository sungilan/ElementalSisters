using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 단일 챔피언의 움직임과 전투를 제어합니다.
/// </summary>
public class ChampionController : MonoBehaviour
{
    public static int TEAMID_PLAYER = 0; //플레이어의 TEAMID = 0
    public static int TEAMID_AI = 1; //AI의 TEAMID = 1


    public GameObject levelupEffectPrefab; // 레벨 업 이펙트 프리팹
    public GameObject projectileStart; // 발사체 시작 지점

    [HideInInspector]
    public int gridType = 0; // 그리드 타입
    [HideInInspector]
    public int gridPositionX = 0; // 그리드 X 좌표
    [HideInInspector]
    public int gridPositionZ = 0; // 그리드 Z 좌표

    [HideInInspector]
    ///팀 ID (플레이어 = 0, 적 = 1)
    public int teamID = 0;

    [HideInInspector]
    public Champion champion; // 챔피언 정보

    [HideInInspector]
    /// 챔피언의 최대 체력
    public float maxHealth = 0;

    [HideInInspector]
    /// 챔피언의 현재 체력
    public float currentHealth = 0;

    [HideInInspector]
    /// 챔피언의 현재 데미지
    public float currentDamage = 0;

    [HideInInspector]
    /// 챔피언의 업그레이드 레벨
    public int lvl = 1;

    private Map map; // 맵
    private GamePlayController gamePlayController; // 게임 플레이 컨트롤러
    private AIopponent aIopponent; // AI 적
    private ChampionAnimation championAnimation; // 챔피언 애니메이션
    private WorldCanvasController worldCanvasController; // 월드 캔버스 컨트롤러

    private NavMeshAgent navMeshAgent; // 네비게이션 메쉬 에이전트

    private Vector3 gridTargetPosition; // 그리드 목표 위치

    private bool _isDragged = false; // 드래그 중인지 여부

    [HideInInspector]
    public bool isAttacking = false; // 공격 중인지 여부

    [HideInInspector]
    public bool isDead = false; // 사망 여부

    private bool isInCombat = false; // 전투 중인지 여부
    private float combatTimer = 0; // 전투 타이머

    private bool isStuned = false; // 기절 상태 여부
    private float stunTimer = 0; // 기절 타이머

    private List<Effect> effects; // 효과 리스트

    /// Start is called before the first frame update
    void Start()
    {
    
    }

    /// <summary>
    /// 챔피언 및 팀 ID가 전달될 때 호출됩니다.(챔피언 초기화 세팅)
    /// </summary>
    /// <param name="_champion"></param>
    /// <param name="_teamID"></param>
    public void Init(Champion _champion, int _teamID)
    {
        champion = _champion;
        teamID = _teamID;

        // "Scripts" 오브젝트에서 스크립트를 찾아서 저장
        map = GameObject.Find("Scripts").GetComponent<Map>();
        aIopponent = GameObject.Find("Scripts").GetComponent<AIopponent>();
        gamePlayController = GameObject.Find("Scripts").GetComponent<GamePlayController>();
        worldCanvasController = GameObject.Find("Scripts").GetComponent<WorldCanvasController>();
        navMeshAgent = this.GetComponent<NavMeshAgent>();
        championAnimation = this.GetComponent<ChampionAnimation>();
       
        // 에이전트 비활성화
        navMeshAgent.enabled = false;

        // 스탯 설정
        maxHealth = champion.health;
        currentHealth = champion.health;
        currentDamage = champion.damage;

        worldCanvasController.AddHealthBar(this.gameObject); //체력바 추가

        effects = new List<Effect>(); //이펙트
    }

    /// Update is called once per frame
    void Update()
    {
        // 드래그 중인 경우
        if (_isDragged)
        {
            // 마우스 클릭 위치에 Ray 생성
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            // 충돌 거리
            float enter = 100.0f;
            if (map.m_Plane.Raycast(ray, out enter))
            {
                // 클릭한 지점 가져오기
                Vector3 hitPoint = ray.GetPoint(enter);

                // 새로운 캐릭터 위치
                Vector3 p = new Vector3(hitPoint.x, 1.0f, hitPoint.z);

                // 챔피언 이동
                this.transform.position = Vector3.Lerp(this.transform.position, p, 0.1f);
            }
        }
        else
        {
            if (gamePlayController.currentGameStage == GameStage.Preparation) //현재 스테이지가 준비단계이면
            {
                // 거리 계산
                float distance = Vector3.Distance(gridTargetPosition, this.transform.position);

                if (distance > 0.25f)
                {
                    this.transform.position = Vector3.Lerp(this.transform.position, gridTargetPosition, 0.1f);
                }
                else
                {
                    this.transform.position = gridTargetPosition;
                }
            }
        }

        
        if (isInCombat && isStuned == false) //전투단계이거나 스턴에 걸린 상태가 아니라면
        {
            if (target == null) //타겟이 null이라면
            {
                combatTimer += Time.deltaTime;
                if (combatTimer > 0.5f)
                {
                    combatTimer = 0;

                    TryAttackNewTarget();
                }
            }


            //combat 
            if (target != null) //타겟이 null이 아니라면
            {
                //rotate towards target
                this.transform.LookAt(target.transform, Vector3.up);

                if (target.GetComponent<ChampionController>().isDead == true) //target champion is alive
                {
                    //remove target if targetchampion is dead 
                    target = null;
                    navMeshAgent.isStopped = true;
                }
                else
                {
                    if (isAttacking == false)
                    {
                        //calculate distance
                        float distance = Vector3.Distance(this.transform.position, target.transform.position);

                        //if we are close enough to attack 
                        if (distance < champion.attackRange)
                        {
                            DoAttack();
                        }
                        else
                        {
                            navMeshAgent.destination = target.transform.position;
                        }
                    }
                }


            }

        }

        // 기절 효과 확인
        if (isStuned) // 기절해있는 상태라면
        {
            stunTimer -= Time.deltaTime; // 스턴타이머 작동

            if(stunTimer < 0) //스턴 타이머가 0보다 작으면
            {
                isStuned = false; //스턴 풀림

                championAnimation.IsAnimated(true); //챔피언 애니메이션 다시 작동

                if(target != null) //타겟이 null이면
                {
                    // 경로 타겟 설정
                    navMeshAgent.destination = target.transform.position;

                    navMeshAgent.isStopped = false;
                }
            }
        }
        

    }

    /// <summary>
    /// 마우스로 챔피언을 이동할 때 드래그 세팅
    /// </summary>
    public bool IsDragged
    {
        get { return _isDragged; }
        set { _isDragged = value;}
    }

    /// <summary>
    /// 전투가 끝난 후 챔피언을 재설정합니다.
    /// </summary>
    public void Reset()
    {
        //set active
        this.gameObject.SetActive(true);

        //reset stats
        maxHealth = champion.health * lvl;
        currentHealth = champion.health * lvl;
        isDead = false;
        isInCombat = false;
        target = null;
        isAttacking = false;

        //reset position
        SetWorldPosition();
        SetWorldRotation();

        //remove all effects
        foreach (Effect e in effects)
        {
            e.Remove();
        }

        effects = new List<Effect>();
    }

    /// <summary>
    /// Assign new grid position
    /// </summary>
    /// <param name="_gridType"></param>
    /// <param name="_gridPositionX"></param>
    /// <param name="_gridPositionZ"></param>
    public void SetGridPosition(int _gridType, int _gridPositionX, int _gridPositionZ)
    {
        gridType = _gridType;
        gridPositionX = _gridPositionX;
        gridPositionZ = _gridPositionZ;


        //set new target when chaning grid position
        gridTargetPosition = GetWorldPosition();
    }

    /// <summary>
    /// 그리드 위치를 월드 위치로 변환
    /// </summary>
    /// <returns></returns>
    public Vector3 GetWorldPosition()
    {
        //월드 포지션을 받아옴
        Vector3 worldPosition = Vector3.zero;

        if (gridType == Map.GRIDTYPE_OWN_INVENTORY) //gridType이 GRIDTYPE_OWN_INVENTORY이면
        {
            worldPosition = map.ownInventoryGridPositions[gridPositionX];
        }
        else if (gridType == Map.GRIDTYPE_HEXA_MAP) //gridType이 GRIDTYPE_HEXA_MAP이면
        {
            worldPosition = map.mapGridPositions[gridPositionX, gridPositionZ];

        }

        return worldPosition;
    }

    /// <summary>
    /// 현재 월드 위치로 이동
    /// </summary>
    public void SetWorldPosition()
    {
        navMeshAgent.enabled = false; //에이전트 비활성화

        //GetWorldPosition()에서 반환된 월드 포지션을 받아옴
        Vector3 worldPosition = GetWorldPosition();

        this.transform.position = worldPosition; //챔피언의 위치를 월드 위치로
        
        gridTargetPosition = worldPosition;
    }

    /// <summary>
    /// 정확한 회전값을 맞춥니다.
    /// </summary>
    public void SetWorldRotation()
    {
        Vector3 rotation = Vector3.zero;

        if (teamID == 0) //teamID가 0(플레이어)이면
        {
            rotation = new Vector3(0, 200, 0);
        }
        else if (teamID == 1) //teamID가 1(AI)이면
        {
            rotation = new Vector3(0, 20, 0);
        }

        this.transform.rotation = Quaternion.Euler(rotation);
    }

    /// <summary>
    /// Upgrade champion lvl
    /// </summary>
    public void UpgradeLevel()
    {
        //레벨 증가
        lvl++;

        //float newSize = 1;
        maxHealth = champion.health;
        currentHealth = champion.health;


        if (lvl == 2) //레벨이 2이면 사이즈 1.5배, 체력, 데미지 2배
        {
            //newSize = 1.5f;
            maxHealth = champion.health * 2;
            currentHealth = champion.health * 2;
            currentDamage = champion.damage * 2;
            Vector3 spawnPosition = transform.position + Vector3.up * 4f; // 챔피언 위치에서 위로 조금 이동
            GameObject starPrefab = Instantiate(gamePlayController.starPrefab, spawnPosition, Quaternion.identity);
            starPrefab.transform.parent = transform;



        }
           
        if (lvl == 3) //레벨이 3이면 사이즈 2배, 체력, 데미지 3배
        {
            //newSize = 2f;
            maxHealth = champion.health * 3;
            currentHealth = champion.health * 3;
            currentDamage = champion.damage * 3;
        }



        //크기 설정(x,y,z축 모두 newsize만큼)
        //this.transform.localScale = new Vector3(newSize, newSize, newSize);

        //레벨업 이펙트 생성
        GameObject levelupEffect = Instantiate(levelupEffectPrefab);

        //레벨업 이펙트의 위치를 챔피언의 위치로 이동
        levelupEffect.transform.position = this.transform.position;

        //1.0f초 뒤 레벨업 이펙트를 제거
        Destroy(levelupEffect, 1.0f);



    }

    private GameObject target;
    /// <summary>
    /// 월드에서 가장 가까운 챔피언을 찾습니다.
    /// </summary>
    /// <returns></returns>
    private GameObject FindTarget()
    {
        GameObject closestEnemy = null;
        float bestDistance = 1000;

        //find enemy
        if (teamID == TEAMID_PLAYER) //teamID가 플레이어일 경우
        {
           
            for (int x = 0; x < Map.hexMapSizeX; x++)
            {
                for (int z = 0; z < Map.hexMapSizeZ / 2; z++) //맵상의 모든 지역을 반복 체크
                {
                    if(aIopponent.gridChampionsArray[x, z] != null) //aIopponent의 챔피언 배열이 null이 아니면(적 챔피언이 존재할 경우)
                    {
                        //aIopponent의 챔피언 배열에서 챔피언 컨트롤러를 받아옵니다.
                        ChampionController championController = aIopponent.gridChampionsArray[x, z].GetComponent<ChampionController>(); 

                        if(championController.isDead == false) //championController의 isDead가 false일 경우(적이 살아 있을 경우)
                        {
                            //본인과 적 챔피언 사이의 거리를 계산합니다.
                            float distance = Vector3.Distance(this.transform.position, aIopponent.gridChampionsArray[x, z].transform.position);

                            //새로운 이 챔피언이 bestDistance보다 더 가까이 있을 경우
                            if (distance < bestDistance)
                            {
                                bestDistance = distance; //이 챔피언과의 거리를 bestDistance로 설정합니다.
                                closestEnemy = aIopponent.gridChampionsArray[x, z]; //이 챔피언을 가장 가까운 적으로 설정합니다.
                            }
                        }

                       
                    }
                }
            }
        }
        else if (teamID == TEAMID_AI) //teamID가 AI일 경우
        {

            for (int x = 0; x < Map.hexMapSizeX; x++)
            {
                for (int z = 0; z < Map.hexMapSizeZ / 2; z++) //맵상의 모든 지역을 반복 체크
                {
                    if (gamePlayController.gridChampionsArray[x, z] != null) //gamePlayController의 챔피언 배열이 null이 아니면(플레이어의 챔피언이 존재할 경우)
                    {
                        //gamePlayController의 챔피언 배열에서 챔피언 컨트롤러를 받아옵니다.
                        ChampionController championController = gamePlayController.gridChampionsArray[x, z].GetComponent<ChampionController>();

                        if (championController.isDead == false) //championController의 isDead가 false일 경우(플레이어의 챔피언이 살아 있을 경우)
                        {
                            //본인과 적 챔피언 사이의 거리를 계산합니다.
                            float distance = Vector3.Distance(this.transform.position, gamePlayController.gridChampionsArray[x, z].transform.position);

                            //새로운 이 챔피언이 bestDistance보다 더 가까이 있을 경우
                            if (distance < bestDistance)
                            {
                                bestDistance = distance; //이 챔피언과의 거리를 bestDistance로 설정합니다.
                                closestEnemy = gamePlayController.gridChampionsArray[x, z]; //이 챔피언을 가장 가까운 적으로 설정합니다.
                            }
                        } 
                    }
                }
            }

        }


        return closestEnemy; //맵상의 모든 지역을 체크하고 그 중 가장 가까운 적을 반환합니다.
    }

    /// <summary>
    /// 공격할 새로운 대상을 찾습니다.
    /// </summary>
    private void TryAttackNewTarget()
    {
        //가장 근접한 적을 찾습니다.
        target = FindTarget(); //FindTarget()을 통해 도출한 가장 가까운 적을 타겟으로 설정

        //새로운 타겟을 찾으면
        if (target != null)
        {
            //에이전트의 목적지를 타겟의 위치로 설정합니다.
            navMeshAgent.destination = target.transform.position;
            

            navMeshAgent.isStopped = false; //에이전트 활성화
        }
    }

    /// <summary>
    /// 스테이지의 공격단계가 시작될 때 호출됩니다.
    /// </summary>
    public void OnCombatStart()
    {
        IsDragged = false;

        this.transform.position = gridTargetPosition;
       

        //in combat grid
        if (gridType == Map.GRIDTYPE_HEXA_MAP)
        {
            isInCombat = true;

            navMeshAgent.enabled = true; //에이전트 활성화

            TryAttackNewTarget(); //가장 가까운 공격 대상을 찾습니다.

        }
      
    }

   
    /// <summary>
    /// 적 챔피언을 공격할 때 호출됩니다.
    /// </summary>
    private void DoAttack()
    {
        Debug.Log("공격실행");
        isAttacking = true;

        ///공격할 동안 에이전트 비활성화
        navMeshAgent.isStopped = true;

        championAnimation.DoAttack(true); //공격 애니메이션 실행

       
    }

    /// <summary>
    /// 공격 애니메이션이 끝났을 때 호출됩니다.
    /// </summary>
    public void OnAttackAnimationFinished()
    {
        isAttacking = false;

        if (target != null)
        {
         
            //공격 대상 챔피언의 챔피언 컨트롤러와 활성화보너스를 가져옵니다.
            ChampionController targetChamoion = target.GetComponent<ChampionController>();

            List<ChampionBonus> activeBonuses = null;

            if (teamID == TEAMID_PLAYER) //teamID가 플레이어일 경우
                activeBonuses = gamePlayController.activeBonusList; //gamePlayController의 활성화 보너스 리스트를 가져옵니다.
            else if (teamID == TEAMID_AI) //teamID가 AI일 경우
                activeBonuses = aIopponent.activeBonusList; //aIopponent의 활성화 보너스 리스트를 가져옵니다.

      
            float d = 0; // 이 챔피언에 적용 중인 총 보너스 효과
            foreach (ChampionBonus b in activeBonuses) //활성화 보너스 리스트를 순회하며 
            {
                d += b.ApplyOnAttack(this, targetChamoion);
            }

            //d + currentDamage만큼 대상에게 데미지를 입힙니다.
            bool isTargetDead = targetChamoion.OnGotHit(d + currentDamage);

  
            //공격 대상이 죽었을 경우
            if (isTargetDead)
                TryAttackNewTarget(); //새로운 공격 대상을 찾습니다.


            //create projectile if have one
            if(champion.attackProjectile != null && projectileStart != null) //공격 발사체와 projectileStart 지점이 존재한다면
            {
                GameObject projectile = Instantiate(champion.attackProjectile); //발사체 프리팹 생성
                projectile.transform.position = projectileStart.transform.position; //발사체의 위치를 projectileStart의 위치로 설정

                projectile.GetComponent<Projectile>().Init(target);


            }
        }
    }

    /// <summary>
    /// 이 챔피언이 데미지를 입었을 때 호출됩니다.
    /// </summary>
    /// <param name="damage"></param>
    public bool OnGotHit(float damage)
    {
        List<ChampionBonus> activeBonuses = null;

        if (teamID == TEAMID_PLAYER) //teamID가 플레이어일 경우
            activeBonuses = gamePlayController.activeBonusList; //gamePlayController의 활성화 보너스 리스트를 가져옵니다.
        else if (teamID == TEAMID_AI) //teamID가 AI일 경우
            activeBonuses = aIopponent.activeBonusList; //aIopponent의 활성화 보너스 리스트를 가져옵니다.

        // 활성 보너스가 있는 경우, 각 보너스에 대해 적용합니다.
        foreach (ChampionBonus b in activeBonuses)
        {
            damage = b.ApplyOnGotHit(this, damage);
        }
       
        currentHealth -= damage; // 받은 데미지만큼 체력을 감소시킵니다.

        
        //death
        if(currentHealth <= 0) //현재 체력이 0보다 작거나 같으면
        {
            // 챔피언을 비활성화하고 사망 상태를 설정합니다.
            this.gameObject.SetActive(false);
            isDead = true;

            // 챔피언 사망 이벤트를 호출합니다.
            aIopponent.OnChampionDeath();
            gamePlayController.OnChampionDeath();
        }

        //데미지 텍스트를 띄웁니다.
        worldCanvasController.AddDamageText(this.transform.position + new Vector3(0, 2.5f, 0), damage); //챔피언의 위치에서 y축으로 2.5f만큼 위에 띄웁니다.

        return isDead; // 챔피언의 사망 상태를 반환합니다.
    }

    /// <summary>
    /// 이 챔피언이 기절했을 때 호출됩니다.
    /// </summary>
    /// <param name="duration">기절 지속시간입니다.</param>
    public void OnGotStun(float duration)
    {
        // 챔피언을 기절 상태로 설정합니다.
        isStuned = true;
        // 기절 지속 시간을 설정합니다.
        stunTimer = duration;

        // 챔피언 애니메이션을 정지합니다.
        championAnimation.IsAnimated(false);

        // 네비게이션 에이전트를 정지합니다.
        navMeshAgent.isStopped = true;
    }

    /// <summary>
    /// 이 챔피언이 치유되면 호출됩니다.
    /// </summary>
    /// <param name="f">치유량입니다.</param>
    public void OnGotHeal(float f)
    {
        // 현재 체력에 치유량을 추가합니다.
        currentHealth += f;
    }



    /// <summary>
    /// 이 챔피언에 효과 추가
    /// </summary>
    /// /// <param name="effectPrefab">효과의 프리팹입니다.</param>
    /// <param name="duration">효과의 지속 시간입니다.</param>
    public void AddEffect(GameObject effectPrefab, float duration)
    {
        // 효과 프리팹이 없는 경우 함수를 종료합니다.
        if (effectPrefab == null)
            return;

        // 이미 존재하는 효과인지 검사합니다.
        bool foundEffect = false;
        foreach (Effect e in effects)
        {
            if(effectPrefab == e.effectPrefab)
            {
                // 이미 존재하는 효과인 경우, 효과의 지속 시간을 갱신합니다.
                e.duration = duration;
                foundEffect = true;
            }
        }

        // 존재하지 않는 효과인 경우, 새로운 효과를 추가합니다.
        if(foundEffect == false)
        {
            // 효과를 초기화하고 리스트에 추가합니다.
            Effect effect = this.gameObject.AddComponent<Effect>();
            effect.Init(effectPrefab, this.gameObject, duration);
            effects.Add(effect); 
        }
       
    }

    /// <summary>
    /// 효과가 만료되면 제거합니다.
    /// </summary>
    public void RemoveEffect(Effect effect)
    {
        // 효과를 리스트에서 제거하고 제거 메서드를 호출합니다.
        effects.Remove(effect);
        effect.Remove();
    }

}
