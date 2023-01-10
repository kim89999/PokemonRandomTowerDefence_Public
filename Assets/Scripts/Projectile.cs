using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Movement2D movement2D;
    private Transform target;
    private float damage;

    private Rigidbody myRigidbody;
    //private float speed = 1.0f;
    private void OnEnable()
    {
        myRigidbody = GetComponent<Rigidbody>();
        target = GameObject.FindGameObjectWithTag("Enemy").transform;
        StartCoroutine(LoseTarget());
    }


    public void Setup(Transform target, float damage)             // Ÿ���� �߻�ü�� ������ �� ȣ��
    {
        movement2D = GetComponent<Movement2D>();
        this.target = target;                       // Ÿ���� �������� target
        this.damage = damage;                       // Ÿ���� �������� ���ݷ�
    }

    private void Update()
    {
        if(target != null)      // target�� �����ϸ�
        {
            // �߻�ü�� target�� ��ġ�� �̵�
            Vector3 direction = (target.position - transform.position).normalized;
            movement2D.MoveTo(direction);

            //Vector3 direction = target.position - transform.position;
            //myRigidbody.velocity = direction.normalized * speed;
        }

        else
        {
            // �߻�ü ������Ʈ ����
            Destroy(gameObject);
        }

    }

    IEnumerator LoseTarget()
    {
        yield return new WaitForSeconds(2f);
        target = null;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Enemy")) return;             // ���� �ƴ� ���� �ε�����
        if (collision.transform != target) return;              // ���� target�� ���� �ƴ� ��

        //collision.GetComponent<Enemy>().OnDie();              // �� ��� �Լ� ȣ��
        collision.GetComponent<EnemyHP>().TakeDamage(damage);   // �� ü���� damage��ŭ ����
        Destroy(gameObject);                                    // �߻�ü ������Ʈ ����
    }

}


/*
 * File : ProjectTileAttack.cs
 * Desc
 *  : Ÿ���� �߻��ϴ� �⺻ �߻�ü�� ����
 *  
 * Functions
 *  : Update() - Ÿ���� �����ϸ� Ÿ�� �������� �̵��ϰ�, Ÿ���� �������� ������ �߻�ü ����
 *  : OnTriggerEnter2D() - Ÿ������ ������ ���� �ε����� �� �� �� ����
 */