using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chaser : MonoBehaviour
{
    public string targetObjectName;
    public float speed = 1;

    GameObject targetObject;
    Rigidbody2D rbody;

    void start()
    {
        // �߰��� Ÿ�� ������Ʈ �̸��� �ִ´�.
        targetObject = GameObject.Find(targetObjectName);
        rbody = GetComponent<Rigidbody2D>();
        rbody.gravityScale = 0; // �߷� ����
        rbody.constraints = RigidbodyConstraints2D.FreezeRotation; // Z �� ȸ�� ����
    }

    void FixedUpdate()
    {
        // �߰��� Ÿ�� ������Ʈ�� ������ ���ϱ�. (normalized : ������ �����ϰ� ���̰� 1�κ��� ����. ������ �ӵ��� �Ѿư�)
        // normalized �� �������� �� ������Ʈ���� �Ÿ��� ����ϱ⿡
        // �Ÿ��� �־��� �� ���� Ŀ����, ����� �� �۾�����
        Vector3 dir = (targetObject.transform.position - this.transform.position).normalized;

        float vx = dir.x * speed;
        float vy = dir.y * speed;

        // �ش� ���������� �ӵ��� ����
        rbody.velocity = new Vector2(vx, vy);

        // x���� �Ѿ�� �����ϱ�
        this.GetComponent<SpriteRenderer>().flipX = (vx < 0);
    }
}
