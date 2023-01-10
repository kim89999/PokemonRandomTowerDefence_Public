using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponType { Cannon = 0, Laser, Slow, Buff }
public enum WeaponState { SearchTarget = 0, TryAttackTower, TryAttackLaser }     // ����� ã��, ����

public class TowerWeapon : MonoBehaviour
{
    [Header("Commons")]
    [SerializeField]
    public TowerTemplate towerTemplate;                          // Ÿ�� ���� (���ݷ�, ���ݼӵ� ��)

    [Header("CannonTower")]
    [SerializeField]
    private GameObject projectilePrefab;                          // �߻�ü ������

    [Header("LaserTower")]
    [SerializeField]
    private LineRenderer lineRenderer;                            // �������� ���Ǵ� ��(LineRenderer)
    [SerializeField]
    private Transform hitEffect;                                  // Ÿ�� ȿ��
    [SerializeField]
    private LayerMask targetLayer;     

    [SerializeField]
    private Transform spawnPoint;          
    private int level = 0; 
    private WeaponState weaponState = WeaponState.SearchTarget; 
    private Transform attackTarget = null;                        // ���� ���
    
    private SpriteRenderer spriteRenderer;                        // Ÿ�� ������Ʈ �̹��� �����
        
    private EnemySpawner enemySpawner; 
    private PlayerPoint playerPoint;   
    public Tile ownerTile;  
    private TowerSpawner towerSpawer;

    private float addedDamage;                          // ������ ���� �߰��� ������
    private int buffLevel;   

    [SerializeField]
    private WeaponType weaponType;                     // ���� �Ӽ� ����

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

    // �ܺο��� ������ �����ϵ���
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
        // ���� ������ �ִ� ���� ã�� ���� ���� �Ÿ��� �ִ��� ũ�� ����
        float closestDistSqr = Mathf.Infinity;

        // EnemySpawner�� EnemyList�� �ִ� ���� �ʿ� �����ϴ� ��� �� �˻�
        for ( int i = 0; i < enemySpawner.EnemyList.Count; ++ i)
        {
            float distance = Vector3.Distance(enemySpawner.EnemyList[i].transform.position, transform.position);

            // ���� ���� �˻����� ������ �Ÿ��� ���ݹ��� ���� �ְ�, ������� �˻��� ������ �Ÿ��� ������
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
        // tartget�� �ִ��� �˻� (�ٸ� �߻�ü�� ���� ����, Goal �������� �̵��� ���� ��)
        if(attackTarget == null)
        {
            return false;
        }

        // target�� ���� ���� �ȿ� �ִ��� �˻� (���� ������ ����� ���ο� �� Ž��)
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

        // ���� �Ӽ��� �⺻, ������ �� ��
        if ( weaponType == WeaponType.Cannon || weaponType == WeaponType.Laser)
        {
            // ���� ���¸� WeaponState.SearchTarget���� ����
            ChangeState(WeaponState.SearchTarget);
        }
    }

    public void ChangeState(WeaponState newState)
    {
        // ������ ������̴� ���� ����
        StopCoroutine(weaponState.ToString());

        // ���� ����
        weaponState = newState;

        // ���ο� ���� ���
        StartCoroutine(weaponState.ToString());
    }

    private void Update()
    {
        if (attackTarget != null)
        {
            RotateToTarget(); // Ÿ���� null�� �ƴϸ� Ÿ���� �ٶ󺸵���
        }
    }

    // Ÿ���� ���� �ٶ󺸵���
    private void RotateToTarget()
    {
        float dx = attackTarget.position.x - transform.position.x;
        float dy = attackTarget.position.y - transform.position.y;

        // x,y �������� �������� ���� ���ϱ�
        // ������ radian �����̱� ������ Mathf.Rad2Deg�� ���� �� ������ ����
        // Mathf.Rad2Deg --> radian���� degree�� ��ȯ �¼�
        float degree = Mathf.Atan2(dy, dx) * Mathf.Rad2Deg;

        // ���� �ִ� ������ �ٶ󺸰� ����
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
            // ���� Ÿ���� ���� ������ �ִ� ���� ���(��) Ž��
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
            // tartget�� �����ϴ°� �������� �˻�
            if(IsPossibleToAttackTarget() == false)
            {
                ChangeState(WeaponState.SearchTarget);
                break;
            }

            // 3. attackRate �ð���ŭ ���
            yield return new WaitForSeconds(towerTemplate.weapon[level].rate);

            // 4. ���� (�߻�ü ����)
            SpawnProjectile();
        }
    }

    private IEnumerator TryAttackLaser()
    {
        // ������, ������ Ÿ�� ȿ�� Ȱ��ȭ
        EnableLaser();

        while (true)
        {
            // target�� �����ϴ°� �������� �˻�
            if(IsPossibleToAttackTarget() == false)
            {
                // ������, ������ Ÿ�� ȿ�� ��Ȱ��ȭ
                DisableLaser();
                ChangeState(WeaponState.SearchTarget);
                break;
            }

            // ������ ����
            SpawnLaser();

            yield return null;
        }
    }
    // Enable, DisablecLaser == ������, ������ Ÿ�� ȿ�� Ȱ�� ��Ȱ��
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

        // ���� �������� ���� ���� ������ ���� �� �� ���� attackTarget�� ������ ������Ʈ�� ����
        for (int i = 0; i < hit.Length; ++i)
        {
            if (hit[i].transform == attackTarget)
            {
                // ���� ��������
                lineRenderer.SetPosition(0, spawnPoint.position);
                // ���� ��ǥ����
                lineRenderer.SetPosition(1, new Vector3(hit[i].point.x, hit[i].point.y, 0) + Vector3.back);
                // Ÿ�� ȿ�� ��ġ ����
                hitEffect.position = hit[i].point;

                // ���ݷ� = Ÿ�� �⺻ ���ݷ� + ������ ���� �߰��� ���ݷ�
                float damage = towerTemplate.weapon[level].damage + AddedDamage;
                attackTarget.GetComponent<EnemyHP>().TakeDamage(damage * Time.deltaTime);
            }
        }
    }

    private void SpawnProjectile()
    {
        GameObject clone = Instantiate(projectilePrefab, spawnPoint.position, Quaternion.identity);

        // ���ݷ� = Ÿ�� �⺻ ���ݷ� + ������ ���� �߰��� ���ݷ�
        float damage = towerTemplate.weapon[level].damage + AddedDamage;
        clone.GetComponent<Projectile>().Setup(attackTarget, damage);
    }

    public bool Upgrade()
    {
        Debug.Log(Name);
        // Ÿ�� ���׷��̵忡 �ʿ��� ����Ʈ�� ������� �˻�
        if (playerPoint.CurrentPoint < towerTemplate.weapon[level+1].point)
        {
            return false;
        }
        // 3�� ���� �� ���׷��̵�
        TowerUpgradeCondition obj = GameObject.Find("TowerUpgradeCondition").GetComponent<TowerUpgradeCondition>();
        obj.towerInit();
        if (obj.TowerList.Count != 0)
        {
            if (obj.mCnt >= 3 && towerTemplate.ToString().Contains("Magikarp"))
            {
                // Ÿ�� ���� ����
                level++;
                // Ÿ�� ���� ���� (Sprite)
                spriteRenderer.sprite = towerTemplate.weapon[level].sprite;
                // Ÿ�� ���׷��̵� �ִϸ��̼� ����
                var ani = GetComponent<Animator>();
                ani.SetTrigger("Upgrade");
                obj.removeTower1();
                obj.listReset();

                return true;
            }
            else if (obj.pCnt >= 3 && towerTemplate.ToString().Contains("Pikachu"))
            {
                // Ÿ�� ���� ����
                level++;
                // Ÿ�� ���� ���� (Sprite)
                spriteRenderer.sprite = towerTemplate.weapon[level].sprite;
                // Ÿ�� ���׷��̵� �ִϸ��̼� ����
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

        // ���� �Ӽ��� �������̸�
        if (weaponType == WeaponType.Laser)
        {
            // ������ ���� �������� ���� ����
            lineRenderer.startWidth = 0.05f + level * 0.5f;
            lineRenderer.endWidth = 0.05f;
        }

        towerSpawer.OnBuffAllBuffTowers();

        return true;
    }

    // �ֺ� Ÿ���� ���ݷ��� �����ִ� ���� �Լ�
    public void OnBuffAroundTower()
    {
        // ���� ���� ��ġ�� "Tower" �±׸� ���� ��� ������Ʈ Ž��
        GameObject[] towers = GameObject.FindGameObjectsWithTag("Tower");

        for ( int i = 0; i < towers.Length; ++ i)
        {
            TowerWeapon weapon = towers[i].GetComponent<TowerWeapon>();

            // �̹� ������ �ް� �ְ�, ���� ���� Ÿ���� �������� ���� �����̸� �н�
            if ( weapon.BuffLevel > Level)
            {
                continue;
            }

            // ���� ���� Ÿ���� �ٸ� Ÿ���� �Ÿ��� �˻��ؼ� ���� �ȿ� Ÿ���� ������
            if ( Vector3.Distance(weapon.transform.position, transform.position) <= towerTemplate.weapon[level].range)
            {
                // ������ ������ ĳ��, ������ Ÿ���̸�
                if ( weapon.WeaponType == WeaponType.Cannon || weapon.WeaponType == WeaponType.Laser)
                {
                    // ������ ���� ���ݷ� ����
                    weapon.AddedDamage = weapon.Damage * (towerTemplate.weapon[level].buff);

                    // Ÿ���� �ް� �ִ� ���� ���� ����
                    weapon.BuffLevel = Level;
                }
            }
        }
    }

    // Ÿ�� ��ġ��
    public void Combine()
    {
        // ���� Ÿ�Ͽ� �ٽ� Ÿ�� �Ǽ��� �����ϵ��� ����
        ownerTile.IsBuildTower = false;

        Destroy(gameObject);
    }

    // Ÿ�� �Ǹ�
    public void Sell()
    {
        // ����Ʈ ����
        playerPoint.CurrentPoint += towerTemplate.weapon[level].sell;

        ownerTile.IsBuildTower = false;

        Destroy(gameObject);
    }
}