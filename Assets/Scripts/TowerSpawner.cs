using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class TowerSpawner : MonoBehaviour
{
    [SerializeField]
    private TowerTemplate[] towerTemplate;        // Ÿ�� ���� ( ���ݷ�, ���ݼӵ� �� ) 

    [SerializeField]
    private PlayerPoint playerPoint;            // Ÿ�� �Ǽ� �� ����Ʈ ����

    [SerializeField]
    private EnemySpawner enemySpawner;          // ���� �ʿ� �����ϴ� �� ����Ʈ ������ ��� ����

    [SerializeField]
    private SystemTextViewer systemTextViewer;  // �� ����, �Ǽ� �Ұ��� ���� �ý��� �޼��� ���

    private bool isOnTowerButton = false;       // Ÿ�� �Ǽ� ��ư�� �������� üũ
    private GameObject followTowerClone = null; // �ӽ� Ÿ�� ��� �Ϸ� �� ������ ���� �����ϴ� ����
    private int towerType;                      // Ÿ�� �Ӽ�

    public void ReadyToSpawnTower(int type)
    {
        towerType = type;

        // ��ư�� �ߺ��ؼ� ������ ���� �����ϱ� ���� �ʿ�
        if ( isOnTowerButton == true )
        {
            return;
        }

        // Ÿ�� �Ǽ� ���� ���� Ȯ��
        // 1. Ÿ���� �Ǽ��� ��ŭ ����Ʈ�� ������ Ÿ�� �Ǽ� x
        if ( towerTemplate[towerType].weapon[0].point > playerPoint.CurrentPoint )
        {
            // ��尡 �����ؼ� Ÿ�� �Ǽ��� �Ұ����ϴٰ� ���
            systemTextViewer.PrintText(SystemType.Money);
            return;
        }

        // Ÿ�� �Ǽ� ��ư�� ������ ���
        isOnTowerButton = true;

        // ���콺�� ����ٴϴ� �ӽ� Ÿ�� ����
        followTowerClone = Instantiate(towerTemplate[towerType].followTowerPrefab);

        // Ÿ�� �Ǽ��� ����� �� �ִ� �ڷ�ƾ �Լ� ����
        StartCoroutine("OnTowerCancelSystem");
    }

    public void SpawnTower(Transform tileTransform)
    {
        // Ÿ�� �Ǽ� ��ư�� ������ ���� Ÿ�� �Ǽ� ����
        if ( isOnTowerButton == false )
        {
            return;
        }

        Tile tile = tileTransform.GetComponent<Tile>();

        // 2. ���� Ÿ���� ��ġ�� �̹� Ÿ���� �Ǽ��Ǿ� ������ Ÿ�� �Ǽ� x
        if ( tile.IsBuildTower == true)
        {
            // ���� ��ġ�� Ÿ�� �Ǽ��� �Ұ����ϴٰ� ���
            systemTextViewer.PrintText(SystemType.Build);
            return;
        }

        // �ٽ� Ÿ�� �Ǽ� ��ư�� ������ Ÿ���� �Ǽ��ϵ��� ���� ����
        isOnTowerButton = false;

        // Ÿ���� �Ǽ��Ǿ� �������� ����
        tile.IsBuildTower = true;

        // Ÿ�� �Ǽ��� �ʿ��� ����Ʈ��ŭ ����
        playerPoint.CurrentPoint -= towerTemplate[towerType].weapon[0].point;

        // ������ Ÿ���� ��ġ�� Ÿ�� �Ǽ� ( Ÿ������ z�� -1�� ��ġ�� ��ġ)
        Vector3 position = tileTransform.position + Vector3.back;
        GameObject clone = Instantiate(towerTemplate[Random.Range(0, 5)].towerPrefab, tileTransform.position, Quaternion.identity);

        // Ÿ�� ���⿡ this, enemySpawner, playerPoint, tile ���� ����
        clone.GetComponent<TowerWeapon>().Setup(this, enemySpawner, playerPoint, tile);

        OnBuffAllBuffTowers();

        Destroy(followTowerClone);

        StopCoroutine("OnTowerCancelSystem");
    }

    // Ÿ�� �Ǽ� ���, �ȷο� Ÿ�� ����
    private IEnumerator OnTowerCancelSystem()
    {
        while (true)
        {
            // ESCŰ �Ǵ� ���콺 ������ ��ư�� ������ �� Ÿ�� �Ǽ� ���
            if ( Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1))
            {
                isOnTowerButton = false;

                // ���콺�� ����ٴϴ� �ӽ� Ÿ�� ����
                Destroy(followTowerClone);
                break;
            }

            yield return null;
        }
    }

    // ���� Ÿ�� �ֺ� ��� ȿ���� ���� �� �ֵ��� ����ȿ�� ����
    public void OnBuffAllBuffTowers()
    {
        GameObject[] towers = GameObject.FindGameObjectsWithTag("Tower");

        for ( int i = 0; i < towers.Length; ++i)
        {
            TowerWeapon weapon = towers[i].GetComponent<TowerWeapon>();

            if ( weapon.WeaponType == WeaponType.Buff )
            {
                weapon.OnBuffAroundTower();
            }
        }
    }

    public void towerUp()
    {
        var ani = GetComponent<Animator>();
        ani.SetTrigger("TowerUpgrade");
    }

}
