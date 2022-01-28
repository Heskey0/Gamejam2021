//code by 赫斯基皇
//https://space.bilibili.com/455965619
//https://github.com/Heskey0

using System;
using System.Collections.Generic;
using QFramework;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;
using ISingleton = QFramework.ISingleton;

public class SkillManager : QMgrBehaviour, ISingleton
{
    #region Singleton

    private SkillManager()
    {
    }

    public void OnSingletonInit()
    {
    }

    public static SkillManager Instance
    {
        get => MonoSingletonProperty<SkillManager>.Instance;
    }

    #endregion

    public override int ManagerId => QMgrID.Game;


    public Role owner;
    public EquipEnum equip = EquipEnum.None;

    /// <summary>
    /// 技能完成施放
    /// </summary>
    private Subject<EquipEnum> _onSkillCompleted = new Subject<EquipEnum>();

    public IObservable<EquipEnum> OnSkillCompleted => _onSkillCompleted;


    private SkillAttack _atk;
    private List<SkillBase> _skills = new List<SkillBase>();

    public SkillManager SetOwner(Role owner)
    {
        _atk = new SkillAttack(this, owner);
        this.owner = owner;
        //TODO:增加技能
        //_skills.Add(new SkillAttack(this)); //0号技能

        return Instance;
    }

    public void runcd(float dt)
    {
        for (int i = 0; i < _skills.Count; i++)
        {
            _skills[i].Cd -= Time.deltaTime;
            // if (i == (int) equip)//当前装备
            // {
            //     UIManager.Instance.SendMsg(EventMsgPool.Instance.Allocate(
            //         _skills[i].Cd / _skills[i].fullCd
            //         , (int) UIEventEnum.Main.RunCd
            //     ));
            // }
        }

        for (int j = 0; j < _atk.cds.Length; j++)
        {
            _atk.cds[j] -= Time.deltaTime;
        }
        
        _atk.RunTimeLine(dt);
    }

    protected override void ProcessMsg(int eventId, QMsg msg)
    {
        var equip = (EquipEnum) int.Parse((msg as EventMsg).Msg as string);
        switch (eventId)
        {
            case (int) GameEventEnum.Skill.Attack:
                if (equip == EquipEnum.None)
                    return;
                _atk.Cast(equip);
                break;
            case (int) GameEventEnum.Skill.ChangeEquip:
                if (equip == EquipEnum.None)
                    return;
                PlayerSkillMaster.ChangeEquip(equip);
                break;
        }
    }

    public void Cast(int equipId, int param = 0)
    {
        _skills[equipId].Cast(equip, param);

        _onSkillCompleted.OnNext(equip);
    }

    public SkillBase this[int id] => _skills[id];
}