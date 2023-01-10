using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackControl : MonoBehaviour
{
    GameObject target;

    [SerializeField]
    GameObject explosion;

    [SerializeField]
    float speed = 2f, rotSpeed = 2f;

    Quaternion rotTarget;
    Vector3 dir;
    Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        target = GameObject.Find("Enemy");
    }

    void Update()
    {
        GuidedAttack();
    }

    void GuidedAttack()
    {
        dir = (target.transform.position - transform.position).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        rotTarget = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotTarget, Time.deltaTime * rotSpeed);
        rb.velocity = new Vector2(dir.x * speed, dir.y * speed);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        Instantiate(explosion, transform.position, Quaternion.identity);
        Destroy(this.gameObject);
    }
}
