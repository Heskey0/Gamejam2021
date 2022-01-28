//code by 赫斯基皇
//https://space.bilibili.com/455965619
//https://github.com/Heskey0

using System;
using QFramework;
using UnityEngine;

public class Test : MonoBehaviour
{
    //TimeLine _timeLine = new TimeLine();

    public EquipEnum curEquip=EquipEnum.ACG;
    private void Start()
    {
        // _timeLine.AddEvent(1f, 1, id => Debug.Log(id));
        // _timeLine.AddEvent(2f, 2, id => Debug.Log(id));
        //
        // _timeLine.Start();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            QMsgCenter.Instance.SendMsg(EventMsgPool.Instance.Allocate(
                ((int)curEquip).ToString()
                , (int) GameEventEnum.Skill.ChangeEquip
            ));
        }
        
        //_timeLine.Loop(Time.deltaTime);
    }
}