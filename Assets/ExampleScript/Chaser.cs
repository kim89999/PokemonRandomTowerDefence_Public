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
        // 추격할 타겟 오브젝트 이름을 넣는다.
        targetObject = GameObject.Find(targetObjectName);
        rbody = GetComponent<Rigidbody2D>();
        rbody.gravityScale = 0; // 중력 무시
        rbody.constraints = RigidbodyConstraints2D.FreezeRotation; // Z 축 회전 무시
    }

    void FixedUpdate()
    {
        // 추격할 타겟 오브젝트의 방향을 구하기. (normalized : 방향은 유지하고 길이가 1인벡터 생성. 일정한 속도로 쫓아감)
        // normalized 가 없을때는 두 오브젝트간의 거리를 계산하기에
        // 거리가 멀어질 땐 값이 커지고, 가까울 떈 작아진다
        Vector3 dir = (targetObject.transform.position - this.transform.position).normalized;

        float vx = dir.x * speed;
        float vy = dir.y * speed;

        // 해당 방향으로의 속도를 세팅
        rbody.velocity = new Vector2(vx, vy);

        // x축을 넘어가면 반전하기
        this.GetComponent<SpriteRenderer>().flipX = (vx < 0);
    }
}
