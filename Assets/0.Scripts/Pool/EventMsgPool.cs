//code by 赫斯基皇
//https://space.bilibili.com/455965619
//https://github.com/Heskey0

using System;
using QFramework;
using UnityEngine;

public class EventMsgPool : Pool<EventMsg>
{
    #region Singleton

    private EventMsgPool() { }

    private static EventMsgPool _instance;

    public static EventMsgPool Instance
    {
        get
        {
            _instance.IsNull().Do(() => _instance = new EventMsgPool());
            return _instance;
        }
        
    }

    #endregion
    
    public EventMsg Allocate(object msg,int eventId)
    {
        //TODO:对象池无法分配
        //EventMsg eventMsg = Allocate();
        EventMsg eventMsg = new EventMsg();
        
        eventMsg.Msg = msg;
        eventMsg.EventID = eventId;
        return eventMsg;
    }

    public override bool Recycle(EventMsg obj)
    {
        mCacheStack.Push(obj);
        return true;
    }

}