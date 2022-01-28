using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SettleEnemyPoint : MonoBehaviour
{
    public static List<Transform> points = new List<Transform>();

    private static float _fullTime = 4f;
    private static float _time = 4f;

    void Start()
    {
        points.Add(transform);
    }

    private void OnDestroy()
    {
        points.Remove(transform);
    }

    public static void Run()
    {
        _time -= Time.deltaTime;
        if (_time < 0)
        {
            _time = Random.Range(_fullTime, _fullTime * 2);
            //_time = _fullTime;
            int index = Random.Range(0, points.Count);
            Debug.Log(index);
            Vector3 point = points[index].position;
            ResourcesMgr.Instance.QfGetInstance("Enemy").transform.position = point;
        }
    }
}