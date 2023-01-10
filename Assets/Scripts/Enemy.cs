using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyDestroyType { Kill = 0, Arrive }

public class Enemy : MonoBehaviour
{
    private int wayPointCount;              // 이동 경로 개수
    private Transform[] wayPoints;          // 이동 경로 정보
    private int currentIndex = 0;           // 현재 목표지점 인덱스
    private Movement2D movement2D;     
    private EnemySpawner enemySpawner;      // 적의 삭제를 EnemySpawner에 알려서 삭제

    [SerializeField]
    private int point = 10;                 // 적 사망 시 획득 포인트
    public bool boss = false;

    public int front1 = 4;
    public int front2 = 8;
    public int back1 = 3;
    public int back2 = 7;
    public int count = 1;

    public void Setup(EnemySpawner enemySpawner, Transform[] wayPoints)
    {
        movement2D = GetComponent<Movement2D>();
        this.enemySpawner = enemySpawner;

        // 적 이동 경로 wayPoints 정보 설정
        wayPointCount = wayPoints.Length;
        this.wayPoints = new Transform[wayPointCount];
        this.wayPoints = wayPoints;

        // 적의 위치를 첫번째 wayPoint 위치로 설정
        transform.position = wayPoints[currentIndex].position;

        // 적 이동/목표지점 설정 코루틴 함수 시작
        StartCoroutine("OnMove");
    }

    private IEnumerator OnMove()
    {
        // 다음 이동 방향 설정
        NextMoveTo();
        
        while (true)
        {
            if (Vector3.Distance(transform.position, wayPoints[currentIndex].position) < 0.02f * movement2D.MoveSpeed)
            {
                if (count  == front1)
                {
                    var ani = GetComponent<Animator>();
                    ani.SetTrigger("front");
                }
                else if (count  == back1)
                {
                    var ani = GetComponent<Animator>();
                    ani.SetTrigger("back");
                }
                else if (count == back2)
                {
                    var ani = GetComponent<Animator>();
                    ani.SetTrigger("back");
                }
                else if (count == front2)
                {
                    var ani = GetComponent<Animator>();
                    ani.SetTrigger("front");
                }

                // 다음 이동 방향 설정
                NextMoveTo();
                count++;
            }

            yield return null;
        }
    }

    private void NextMoveTo()
    {
        // 아직 이동할 wayPoints가 남아있다면
        if (currentIndex < wayPointCount - 1)
        {
            // 적의 위치를 정확하게 목표 위치로 설정
            transform.position = wayPoints[currentIndex].position;
            // 이동 방향 설정 => 다음 목표지점(wayPoints)
            currentIndex++;
            Vector3 direction = (wayPoints[currentIndex].position - transform.position).normalized;
            movement2D.MoveTo(direction);
        }
        // 현재 위치가 마지막 wayPoints이면
        else
        {
            // 목표지점에 도달해서 사망할 때는 포인트를 주지 않도록
            point = 0;

            OnDie(EnemyDestroyType.Arrive);
        }
    }

    public void OnDie(EnemyDestroyType type)
    {
        enemySpawner.DestroyEnemy(type, this, point);
        // this == 나 자신 (EnemyComponent)
    }

}
