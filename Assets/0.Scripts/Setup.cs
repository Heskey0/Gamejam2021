//code by 赫斯基皇
//https://space.bilibili.com/455965619
//https://github.com/Heskey0

using System;
using QFramework;
using QFramework.Example;
using UnityEngine;

public class Setup : MonoBehaviour
{
    private void Awake()
    {
        ResKit.Init();
        GameMgr.Instance.Init();

        UIKit.OpenPanel<StartPanel>();
    }
}
