//code by 赫斯基皇
//https://space.bilibili.com/455965619
//https://github.com/Heskey0

using System;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class Bullet : MonoBehaviour
{
    private static Transform _target;
    private float _rotateDeg;
    private bool _arrived = false;

    public void Init(Transform target)
    {
        _target = target;
    }

    private void Awake()
    {
        _rotateDeg = Random.Range(-40, 40);
    }

    private void Update()
    {
        if (_target==null)
        {
            GameObject.Destroy(gameObject, 1f);
        }
        
        transform.position += (Vector3) math.mul(transform.rotation, new float3(1, 0, 0)) * Time.deltaTime *
                              Random.Range(1, 12);
        Track();
    }

    private void Track()
    {
        var dir = _target.position - transform.position;
        float angle = math.degrees(math.atan2(dir.y, dir.x)) + _rotateDeg;

        var tmpRot = transform.rotation;
        tmpRot = Quaternion.Euler(new float3(0, 0, angle));
        transform.rotation = tmpRot;

        var dis = math.length(dir);
        if (dis < 0.3f)
        {
            if (!_arrived)
            {
                Damage();
            }
        }


    }

    private void Damage()
    {
        _arrived = true;
        var enemy = _target.GetComponent<Enemy>();
        enemy.OnAttack(GameConfig.Instance.bigSisterAtk, Vector3.zero, false);
        GameObject.Destroy(gameObject, 1f);
    }
}