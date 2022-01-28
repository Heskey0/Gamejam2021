//code by 赫斯基皇
//https://space.bilibili.com/455965619
//https://github.com/Heskey0

using System;
using UnityEngine;
using System.Collections;
using Cinemachine;
using Unity.VisualScripting;
using Random = UnityEngine.Random;

public static class UnityExtern
{
    public static T Find<T>(this GameObject parent, string path) where T : class
    {
        var targetObj = parent.transform.Find(path);
        if (targetObj == null) return null;
        return targetObj.GetComponent<T>();
    }

    public static void DestroyAllChildren(this GameObject parent)
    {
        for (int i = 0; i < parent.transform.childCount; i++)
        {
            GameObject.Destroy(parent.transform.GetChild(i).gameObject);
        }
    }

    public static bool FloatEqual(this float a, float b)
    {
        return Mathf.Abs(a - b) < 0.01f;
    }


    public static void Shake(this Transform transform, float duration, float magnitude)
    {
        Transform followTransform = GameObject.FindGameObjectWithTag("Player").transform;

        var brain = transform.GetComponent<CinemachineBrain>();
        brain.ActiveVirtualCamera.Follow = null;
        brain.ActiveVirtualCamera.LookAt = null;

        if (brain != null)
            brain.enabled = false;

        CoroutineMgr.Instance.StartCoroutine(ShakeCoroutine(transform, followTransform, duration, magnitude));
    }

    private static IEnumerator ShakeCoroutine(Transform transform, Transform followTransform, float duration,
        float magnitude)
    {
        Vector3 orignalPosition = transform.position;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;
            transform.position = orignalPosition + new Vector3(x, y, 0);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = orignalPosition;

        var brain = transform.GetComponent<CinemachineBrain>();
        if (brain != null)
        {
            brain.enabled = true;
            //yield return new WaitForSeconds(1);
            yield return null;
            brain.ActiveVirtualCamera.Follow = followTransform;
            brain.ActiveVirtualCamera.LookAt = followTransform;
        }
    }
}
