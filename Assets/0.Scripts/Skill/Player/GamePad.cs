using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class GamePad : MonoBehaviour
{
    public static bool isAvailable = true;

    private Transform _ownerTrans;
    private Rigidbody2D _rb;

    private float _time = 0.8f;

    void Awake()
    {
        isAvailable = false;
        _rb = gameObject.AddComponent<Rigidbody2D>();
        _rb.gravityScale = 0;
        gameObject.AddComponent<CircleCollider2D>().isTrigger = true;
    }

    private void OnDestroy()
    {
        isAvailable = true;
    }

    void Update()
    {
        _time -= Time.deltaTime;

        if (_time <= 0)
        {
            var dir = (_ownerTrans.position - transform.position).normalized;
            var dis = Vector3.Distance(_ownerTrans.position, transform.position);
            if (dis <= 0.3f)
                Destroy(gameObject);

            _rb.velocity = _rb.velocity * 0.3f + new Vector2(dir.x, dir.y) * 8;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Enemy"))
            return;

        var enemy = other.GetComponent<Enemy>();
        enemy.OnAttack(GameConfig.Instance.gamePadAtk,
            (enemy.transform.position - transform.position).normalized * GameConfig.Instance.gamePadForce);
    }

    public void Init(Transform trans, Vector2 initVelocity, bool isLeftward)
    {
        _ownerTrans = trans;
        _rb.velocity = isLeftward ? -initVelocity : initVelocity;
    }
}