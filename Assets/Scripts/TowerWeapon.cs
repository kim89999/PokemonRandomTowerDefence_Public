using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponType { Cannon = 0, Laser, Slow, Buff }
public enum WeaponState { SearchTarget = 0, TryAttackTower, TryAttackLaser }     // 대상을 찾고, 공격

public class TowerWeapon : MonoBehaviour
{
    [Header("Commons")]
    [SerializeField]
    public TowerTemplate towerTemplate;                          // 타워 정보 (공격력, 공격속도 등)

    [Header("CannonTower")]
    [SerializeField]
    private GameObject projectilePrefab;                          // 발사체 프리팹

    [Header("LaserTower")]
    [SerializeField]
    private LineRenderer lineRenderer;                            // 레이저로 사용되는 선(LineRenderer)
    [SerializeField]
    private Transform hitEffect;                                  // 타격 효과
    [SerializeField]
    private LayerMask targetLayer;     

    [SerializeField]
    private Transform spawnPoint;          
    private int level = 0; 
    private WeaponState weaponState = WeaponState.SearchTarget; 
    private Transform attackTarget = null;                        // 공격 대상
    
    private SpriteRenderer spriteRenderer;                        // 타워 오브젝트 이미지 변경용
        
    private EnemySpawner enemySpawner; 
    private PlayerPoint playerPoint;   
    public Tile ownerTile;  
    private TowerSpawner towerSpawer;

    private float addedDamage;                          // 버프에 의해 추가된 데미지
    private int buffLevel;   

    [SerializeField]
    private WeaponType weaponType;                     // 무기 속성 설정

    public Sprite TowerSprite       => towerTemplate.weapon[level].sprite;
    public string Name              => towerTemplate.weapon[level].name;
    public float Damage             => towerTemplate.weapon[level].damage;
    public float Rate               => towerTemplate.weapon[level].rate;
    public float Range              => towerTemplate.weapon[level].range;
    public int UpgradePoint         => Level < MaxLevel ? towerTemplate.weapon[level + 1].point : 0;
    public int SellPoint            => towerTemplate.weapon[level].sell;
    public int Level                => level + 1;
    public int MaxLevel             => towerTemplate.weapon.Length;
    public float Slow               => towerTemplate.weapon[level].slow;
    public float Buff               => towerTemplate.weapon[level].buff;
    public WeaponType WeaponType    => weaponType;

    // 외부에서 수정이 가능하도록
    public float AddedDamage
    {
        set => addedDamage = Mathf.Max(0, value);
        get => addedDamage;
    }
    public int BuffLevel
    {
        set => buffLevel = Mathf.Max(0, value);
        get => buffLevel;
    }

    private Transform FindClosestAttackTarget()
    {
        // 제일 가까이 있는 적을 찾기 위해 최초 거리를 최대한 크게 설정
        float closestDistSqr = Mathf.Infinity;

        // EnemySpawner의 EnemyList에 있는 현재 맵에 존재하는 모든 적 검사
        for ( int i = 0; i < enemySpawner.EnemyList.Count; ++ i)
        {
            float distance = Vector3.Distance(enemySpawner.EnemyList[i].transform.position, transform.position);

            // 현재 적이 검사중인 적과의 거리가 공격범위 내에 있고, 현재까지 검사한 적보다 거리가 가까우면
            if ( distance <= towerTemplate.weapon[level].range && distance <= closestDistSqr)
            {
                closestDistSqr = distance;
                attackTarget = enemySpawner.EnemyList[i].transform;
            }
        }

        return attackTarget;
    }

    private bool IsPossibleToAttackTarget()
    {
        // tartget이 있는지 검사 (다른 발사체에 의해 제거, Goal 지점까지 이동해 삭제 등)
        if(attackTarget == null)
        {
            return false;
        }

        // target이 공격 범위 안에 있는지 검사 (공격 범위를 벗어나면 새로운 적 탐색)
        float distance = Vector3.Distance(attackTarget.position, transform.position);
        if(distance > towerTemplate.weapon[level].range)
        {
            attackTarget = null;
            return false;
        }

        return true;
    }

    public void Setup(TowerSpawner towerSpawner, EnemySpawner enemySpawner, PlayerPoint playerPoint, Tile ownerTile)
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        this.towerSpawer = towerSpawer;
        this.enemySpawner = enemySpawner;
        this.playerPoint = playerPoint;
        this.ownerTile = ownerTile;

        // 무기 속성이 기본, 레이저 일 때
        if ( weaponType == WeaponType.Cannon || weaponType == WeaponType.Laser)
        {
            // 최초 상태를 WeaponState.SearchTarget으로 설정
            ChangeState(WeaponState.SearchTarget);
        }
    }

    public void ChangeState(WeaponState newState)
    {
        // 이전에 재생중이던 상태 종료
        StopCoroutine(weaponState.ToString());

        // 상태 변경
        weaponState = newState;

        // 새로운 상태 재생
        StartCoroutine(weaponState.ToString());
    }

    private void Update()
    {
        if (attackTarget != null)
        {
            RotateToTarget(); // 타켓이 null이 아니면 타켓을 바라보도록
        }
    }

    // 타워가 적을 바라보도록
    private void RotateToTarget()
    {
        float dx = attackTarget.position.x - transform.position.x;
        float dy = attackTarget.position.y - transform.position.y;

        // x,y 변위값을 바탕으로 각도 구하기
        // 각도가 radian 단위이기 때문에 Mathf.Rad2Deg를 곱해 도 단위를 구함
        // Mathf.Rad2Deg --> radian에서 degree로 변환 승수
        float degree = Mathf.Atan2(dy, dx) * Mathf.Rad2Deg;

        // 적이 있는 각도로 바라보게 만듦
        if (degree < 0 && degree >= -180)
        {
            var ani = GetComponent<Animator>();
            ani.SetTrigger("back");
        }
        else if (degree <= 180 && degree >= 0)
        {
            var ani = GetComponent<Animator>();
            ani.SetTrigger("front");
        }
    }

    private IEnumerator SearchTarget()
    {
        while (true)
        {
            // 현재 타워에 가장 가까이 있는 공격 대상(적) 탐색
            attackTarget = FindClosestAttackTarget();

            if (attackTarget != null)
            {
                if (weaponType == WeaponType.Cannon)
                {
                    ChangeState(WeaponState.TryAttackTower);
                }
                else if (weaponType == WeaponType.Laser)
                {
                    ChangeState(WeaponState.TryAttackLaser);
                }
            }
            yield return null;
        }
    }

    private IEnumerator TryAttackTower()
    {
        while (true)
        {
            // tartget을 공격하는게 가능한지 검사
            if(IsPossibleToAttackTarget() == false)
            {
                ChangeState(WeaponState.SearchTarget);
                break;
            }

            // 3. attackRate 시간만큼 대기
            yield return new WaitForSeconds(towerTemplate.weapon[level].rate);

            // 4. 공격 (발사체 생성)
            SpawnProjectile();
        }
    }

    private IEnumerator TryAttackLaser()
    {
        // 레이저, 레이저 타격 효과 활성화
        EnableLaser();

        while (true)
        {
            // target을 공격하는게 가능한지 검사
            if(IsPossibleToAttackTarget() == false)
            {
                // 레이저, 레이저 타격 효과 비활성화
                DisableLaser();
                ChangeState(WeaponState.SearchTarget);
                break;
            }

            // 레이저 공격
            SpawnLaser();

            yield return null;
        }
    }
    // Enable, DisablecLaser == 레이저, 레이저 타격 효과 활성 비활성
    private void EnableLaser()
    {
        lineRenderer.gameObject.SetActive(true);
        hitEffect.gameObject.SetActive(true);
    }
    private void DisableLaser()
    {
        lineRenderer.gameObject.SetActive(false);
        hitEffect.gameObject.SetActive(false);
    }

    private void SpawnLaser()
    {
        Vector3 direction = attackTarget.position - spawnPoint.position;
        RaycastHit2D[] hit = Physics2D.RaycastAll(spawnPoint.position, direction,
                                                towerTemplate.weapon[level].range, targetLayer);

        // 같은 방향으로 여러 개의 광선을 쏴서 그 중 현재 attackTarget과 동일한 오브젝트를 검출
        for (int i = 0; i < hit.Length; ++i)
        {
            if (hit[i].transform == attackTarget)
            {
                // 선의 시작지점
                lineRenderer.SetPosition(0, spawnPoint.position);
                // 선의 목표지점
                lineRenderer.SetPosition(1, new Vector3(hit[i].point.x, hit[i].point.y, 0) + Vector3.back);
                // 타격 효과 위치 설정
                hitEffect.position = hit[i].point;

                // 공격력 = 타워 기본 공격력 + 버프에 의해 추가된 공격력
                float damage = towerTemplate.weapon[level].damage + AddedDamage;
                attackTarget.GetComponent<EnemyHP>().TakeDamage(damage * Time.deltaTime);
            }
        }
    }

    private void SpawnProjectile()
    {
        GameObject clone = Instantiate(projectilePrefab, spawnPoint.position, Quaternion.identity);

        // 공격력 = 타워 기본 공격력 + 버프에 의해 추가된 공격력
        float damage = towerTemplate.weapon[level].damage + AddedDamage;
        clone.GetComponent<Projectile>().Setup(attackTarget, damage);
    }

    public bool Upgrade()
    {
        Debug.Log(Name);
        // 타워 업그레이드에 필요한 포인트가 충분한지 검사
        if (playerPoint.CurrentPoint < towerTemplate.weapon[level+1].point)
        {
            return false;
        }
        // 3개 모일 시 업그레이드
        TowerUpgradeCondition obj = GameObject.Find("TowerUpgradeCondition").GetComponent<TowerUpgradeCondition>();
        obj.towerInit();
        if (obj.TowerList.Count != 0)
        {
            if (obj.mCnt >= 3 && towerTemplate.ToString().Contains("Magikarp"))
            {
                // 타워 레벨 증가
                level++;
                // 타워 외형 변경 (Sprite)
                spriteRenderer.sprite = towerTemplate.weapon[level].sprite;
                // 타워 업그레이드 애니메이션 변경
                var ani = GetComponent<Animator>();
                ani.SetTrigger("Upgrade");
                obj.removeTower1();
                obj.listReset();

                return true;
            }
            else if (obj.pCnt >= 3 && towerTemplate.ToString().Contains("Pikachu"))
            {
                // 타워 레벨 증가
                level++;
                // 타워 외형 변경 (Sprite)
                spriteRenderer.sprite = towerTemplate.weapon[level].sprite;
                // 타워 업그레이드 애니메이션 변경
                var ani = GetComponent<Animator>();
                ani.SetTrigger("Upgrade");
                obj.removeTower2();
                obj.listReset();
                return true;
            }
            else
            {
                obj.listReset();
                return false;
            }
        }

        // 무기 속성이 레이저이면
        if (weaponType == WeaponType.Laser)
        {
            // 레벨에 따라 레이저의 굵기 설정
            lineRenderer.startWidth = 0.05f + level * 0.5f;
            lineRenderer.endWidth = 0.05f;
        }

        towerSpawer.OnBuffAllBuffTowers();

        return true;
    }

    // 주변 타워의 공격력을 높여주는 버프 함수
    public void OnBuffAroundTower()
    {
        // 현재 옆에 배치된 "Tower" 태그를 가진 모든 오브젝트 탐색
        GameObject[] towers = GameObject.FindGameObjectsWithTag("Tower");

        for ( int i = 0; i < towers.Length; ++ i)
        {
            TowerWeapon weapon = towers[i].GetComponent<TowerWeapon>();

            // 이미 버프를 받고 있고, 현재 버프 타워의 레벨보다 높은 버프이면 패스
            if ( weapon.BuffLevel > Level)
            {
                continue;
            }

            // 현재 버프 타워와 다른 타워의 거리를 검사해서 범위 안에 타워가 있으면
            if ( Vector3.Distance(weapon.transform.position, transform.position) <= towerTemplate.weapon[level].range)
            {
                // 공격이 가능한 캐논, 레이저 타워이면
                if ( weapon.WeaponType == WeaponType.Cannon || weapon.WeaponType == WeaponType.Laser)
                {
                    // 버프에 의해 공격력 증가
                    weapon.AddedDamage = weapon.Damage * (towerTemplate.weapon[level].buff);

                    // 타워가 받고 있는 버프 레벨 설정
                    weapon.BuffLevel = Level;
                }
            }
        }
    }

    // 타워 합치기
    public void Combine()
    {
        // 현재 타일에 다시 타워 건설이 가능하도록 설정
        ownerTile.IsBuildTower = false;

        Destroy(gameObject);
    }

    // 타워 판매
    public void Sell()
    {
        // 포인트 증가
        playerPoint.CurrentPoint += towerTemplate.weapon[level].sell;

        ownerTile.IsBuildTower = false;

        Destroy(gameObject);
    }
}