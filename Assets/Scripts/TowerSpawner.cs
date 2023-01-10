using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class TowerSpawner : MonoBehaviour
{
    [SerializeField]
    private TowerTemplate[] towerTemplate;        // 타워 정보 ( 공격력, 공격속도 등 ) 

    [SerializeField]
    private PlayerPoint playerPoint;            // 타워 건설 시 포인트 감소

    [SerializeField]
    private EnemySpawner enemySpawner;          // 현재 맵에 존재하는 적 리스트 정보를 얻기 위함

    [SerializeField]
    private SystemTextViewer systemTextViewer;  // 돈 부족, 건설 불가와 같은 시스템 메세지 출력

    private bool isOnTowerButton = false;       // 타워 건설 버튼을 눌렀는지 체크
    private GameObject followTowerClone = null; // 임시 타워 사용 완료 시 삭제를 위해 저장하는 변수
    private int towerType;                      // 타워 속성

    public void ReadyToSpawnTower(int type)
    {
        towerType = type;

        // 버튼을 중복해서 누르는 것을 방지하기 위해 필요
        if ( isOnTowerButton == true )
        {
            return;
        }

        // 타워 건설 가능 여부 확인
        // 1. 타워를 건설할 만큼 포인트가 없으면 타워 건설 x
        if ( towerTemplate[towerType].weapon[0].point > playerPoint.CurrentPoint )
        {
            // 골드가 부족해서 타워 건설이 불가능하다고 출력
            systemTextViewer.PrintText(SystemType.Money);
            return;
        }

        // 타워 건설 버튼을 눌렀을 경우
        isOnTowerButton = true;

        // 마우스를 따라다니는 임시 타워 생성
        followTowerClone = Instantiate(towerTemplate[towerType].followTowerPrefab);

        // 타워 건설을 취소할 수 있는 코루틴 함수 시작
        StartCoroutine("OnTowerCancelSystem");
    }

    public void SpawnTower(Transform tileTransform)
    {
        // 타워 건설 버튼을 눌렀을 때만 타워 건설 가능
        if ( isOnTowerButton == false )
        {
            return;
        }

        Tile tile = tileTransform.GetComponent<Tile>();

        // 2. 현재 타일의 위치에 이미 타워가 건설되어 있으면 타워 건설 x
        if ( tile.IsBuildTower == true)
        {
            // 현재 위치에 타워 건설이 불가능하다고 출력
            systemTextViewer.PrintText(SystemType.Build);
            return;
        }

        // 다시 타워 건설 버튼을 눌러서 타워를 건설하도록 변수 설정
        isOnTowerButton = false;

        // 타워가 건설되어 있음으로 설정
        tile.IsBuildTower = true;

        // 타워 건설에 필요한 포인트만큼 감소
        playerPoint.CurrentPoint -= towerTemplate[towerType].weapon[0].point;

        // 선택한 타일의 위치에 타워 건설 ( 타워보다 z축 -1의 위치에 배치)
        Vector3 position = tileTransform.position + Vector3.back;
        GameObject clone = Instantiate(towerTemplate[Random.Range(0, 5)].towerPrefab, tileTransform.position, Quaternion.identity);

        // 타워 무기에 this, enemySpawner, playerPoint, tile 정보 전달
        clone.GetComponent<TowerWeapon>().Setup(this, enemySpawner, playerPoint, tile);

        OnBuffAllBuffTowers();

        Destroy(followTowerClone);

        StopCoroutine("OnTowerCancelSystem");
    }

    // 타워 건설 취소, 팔로우 타워 삭제
    private IEnumerator OnTowerCancelSystem()
    {
        while (true)
        {
            // ESC키 또는 마우스 오른쪽 버튼을 눌렀을 때 타워 건설 취소
            if ( Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1))
            {
                isOnTowerButton = false;

                // 마우스를 따라다니는 임시 타워 삭제
                Destroy(followTowerClone);
                break;
            }

            yield return null;
        }
    }

    // 버프 타워 주변 모두 효과를 받을 수 있도록 버프효과 갱신
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
