using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHP : MonoBehaviour
{
    [SerializeField]
    private float maxHP;                // 최대 체력
    private float currentHP;            // 현재 체력
    private bool isDie = false;         // 적이 사망하면 isDie를 true로 변경
    private Enemy enemy;
    private SpriteRenderer spriteRenderer;

    // 적의 현재체력, 최대체력 정보를 외부 클래스에서 확인할 수 있도록 프로퍼티 생성
    public float MaxHP => maxHP;
    public float CurrentHP => currentHP;

    private void Awake()
    {
        currentHP = maxHP;              // 현재 체력을 최대 체력과 같게 설정
        enemy = GetComponent<Enemy>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void TakeDamage(float damage)
    {
        // 현재 적의 상태가 사망 상태이면 아래 코드를 실행하지 않는다.
        if (isDie == true) return;

        // 현재 체력을 damage 만큼 감소
        currentHP -= damage;

        StopCoroutine("HitAlphaAnimation");
        StartCoroutine("HitAlphaAnimation");

        // 체력이 0이하 = 적 캐릭터 사망
        if(currentHP <= 0)
        {
            isDie = true;
            // 적 캐릭터 사망
            enemy.OnDie(EnemyDestroyType.Kill);
        }
    }

    private IEnumerator HitAlphaAnimation()
    {
        // 현재 적의 색상을 color 변수에 저장
        Color color = spriteRenderer.color;

        // 적의 투명도를 40%로 설정
        color.a = 0.4f;
        spriteRenderer.color = color;

        // 0.05초 동안 대기
        yield return new WaitForSeconds(0.05f);

        // 적의 투명도를 100%로 설정
        color.a = 1.0f;
        spriteRenderer.color = color;
    }

}
