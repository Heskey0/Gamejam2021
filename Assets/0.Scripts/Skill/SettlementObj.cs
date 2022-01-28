//code by 赫斯基皇
//https://space.bilibili.com/455965619
//https://github.com/Heskey0

using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

/// <summary>
/// 伤害结算物
/// </summary>
public class SettlementObj : MonoBehaviour
{
    List<Transform> _enemyTransforms = new List<Transform>();


    private Rigidbody2D _rb;

    private Subject<Collider2D> _onHitS = new Subject<Collider2D>();
    public IObservable<Collider2D> OnHitCallback => _onHitS;

    public Action onDestroyCallback;

    public void Init(float radius, float settleTime, Action<Collider2D> hitCallback)
    {
        OnHitCallback.Subscribe(hitCallback);
        //增加刚体
        _rb = gameObject.AddComponent<Rigidbody2D>();
        _rb.gravityScale = 0;
        //加碰撞器
        var c = gameObject.AddComponent<CircleCollider2D>();
        c.isTrigger = true;
        c.radius = radius;


        GameObject.Destroy(gameObject, settleTime);
    }

    public Transform ClosestEnemy()
    {
        if (_enemyTransforms.Count==0)
            return null;
        
        Transform outTrans = _enemyTransforms[0];
        for (int i = 0; i < _enemyTransforms.Count; i++)
        {
            outTrans = Vector3.Distance(transform.position, _enemyTransforms[i].position) <
                       Vector3.Distance(transform.position, outTrans.position)
                ? _enemyTransforms[i]
                : outTrans;
        }

        return outTrans;
    }
    
    private void Update()
    {

        
    }

    private void OnDestroy()
    {
        onDestroyCallback?.Invoke();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //transforms.Add();
        if (other.CompareTag("Enemy"))
        {
            _enemyTransforms.Add(other.transform);
        }
        
        _onHitS.OnNext(other);
    }
}