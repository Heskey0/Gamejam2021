//code by 赫斯基皇
//https://space.bilibili.com/455965619
//https://github.com/Heskey0

using System;
using QFramework;
using UnityEngine;

public class EventMsg : QMsg
{
    public object Msg = null;

    public EventMsg() { }
    public EventMsg(object msg, int eventid) : base(eventid)
    {
        this.Msg = msg;
    }
}
