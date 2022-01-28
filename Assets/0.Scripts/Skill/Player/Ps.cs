//code by 赫斯基皇
//https://space.bilibili.com/455965619
//https://github.com/Heskey0

using System;
using UnityEngine;

public class Ps : MonoBehaviour
{
    private float _lifeTime;
    private float _speed;

    private void Start()
    {
        _lifeTime = GameConfig.Instance.psLifeTime;
    }

    public void Init(bool isLeftward)
    {
        _speed = GameConfig.Instance.psSpeed;
        _speed = isLeftward ? -_speed : _speed;

        //Debug.Log("ps:isLeft:"+isLeftward+":"+_speed);
    }

    private void Update()
    {
        transform.position += Time.deltaTime * Vector3.right * _speed;
        _lifeTime -= Time.deltaTime;
        if (_lifeTime < 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            var dir = (other.transform.position - transform.position).normalized;
            other.GetComponent<Enemy>().OnAttack(GameConfig.Instance.psAtk, GameConfig.Instance.psForce * dir);
        }
    }
}