//code by 赫斯基皇
//https://space.bilibili.com/455965619
//https://github.com/Heskey0

using System;
using QFramework;
using UnityEngine;

public static class UIEventEnum
{
    public enum Setup
    {
        Start = QMgrID.UI,
        End
    }

    public enum Main
    {
        Start = Setup.End,
        RunCd,
        ChangeEquip,
        End
    }
}