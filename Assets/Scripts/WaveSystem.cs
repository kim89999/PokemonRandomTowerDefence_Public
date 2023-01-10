using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSystem : MonoBehaviour
{
    [SerializeField]
    private Wave[] waves;                   // 현재 스테이지의 모든 웨이브 정보

    [SerializeField]
    private EnemySpawner enemySpawner;
    private int currentWaveIndex = -1;      // 현재 웨이브 인덱스 

    public void StartWave()
    {

        // 현재 맵에 적이 없고, Wave가 남아있으면
        if(enemySpawner.EnemyList.Count == 0 && currentWaveIndex < waves.Length-1)
        {
            // 인덱스의 시작이 -1이기 때문에 웨이브 인덱스 증가를 제일 먼저 함
            currentWaveIndex++;

            // EnemySpawner의 StartWave() 함수 호출, 현재 웨이브 정보 제공
            enemySpawner.StartWave(waves[currentWaveIndex]);
        }
    }
}

// 메모리 상에 존재하는 오브젝트 정보를 string 또는 byte 데이터 형태로 변형
[System.Serializable]
public struct Wave
{
    public float spawnTime;                 // 현재 웨이브 적 생성 주기
    public int maxEnemyCount;               // 현재 웨이브 적 등장 숫자
    public GameObject[] enemyPrefabs;       // 현재 웨이브 적 등장 종류
}


/*
 * File : WaveSystem.cs
 * Desc
 *  : 현재 스테이지의 웨이브를 관리
 *  
 */