using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyDestroyType { Kill = 0, Arrive }

public class Enemy : MonoBehaviour
{
    private int wayPointCount;              // �̵� ��� ����
    private Transform[] wayPoints;          // �̵� ��� ����
    private int currentIndex = 0;           // ���� ��ǥ���� �ε���
    private Movement2D movement2D;     
    private EnemySpawner enemySpawner;      // ���� ������ EnemySpawner�� �˷��� ����

    [SerializeField]
    private int point = 10;                 // �� ��� �� ȹ�� ����Ʈ
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

        // �� �̵� ��� wayPoints ���� ����
        wayPointCount = wayPoints.Length;
        this.wayPoints = new Transform[wayPointCount];
        this.wayPoints = wayPoints;

        // ���� ��ġ�� ù��° wayPoint ��ġ�� ����
        transform.position = wayPoints[currentIndex].position;

        // �� �̵�/��ǥ���� ���� �ڷ�ƾ �Լ� ����
        StartCoroutine("OnMove");
    }

    private IEnumerator OnMove()
    {
        // ���� �̵� ���� ����
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

                // ���� �̵� ���� ����
                NextMoveTo();
                count++;
            }

            yield return null;
        }
    }

    private void NextMoveTo()
    {
        // ���� �̵��� wayPoints�� �����ִٸ�
        if (currentIndex < wayPointCount - 1)
        {
            // ���� ��ġ�� ��Ȯ�ϰ� ��ǥ ��ġ�� ����
            transform.position = wayPoints[currentIndex].position;
            // �̵� ���� ���� => ���� ��ǥ����(wayPoints)
            currentIndex++;
            Vector3 direction = (wayPoints[currentIndex].position - transform.position).normalized;
            movement2D.MoveTo(direction);
        }
        // ���� ��ġ�� ������ wayPoints�̸�
        else
        {
            // ��ǥ������ �����ؼ� ����� ���� ����Ʈ�� ���� �ʵ���
            point = 0;

            OnDie(EnemyDestroyType.Arrive);
        }
    }

    public void OnDie(EnemyDestroyType type)
    {
        enemySpawner.DestroyEnemy(type, this, point);
        // this == �� �ڽ� (EnemyComponent)
    }

}
