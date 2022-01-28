//code by 赫斯基皇
//https://space.bilibili.com/455965619
//https://github.com/Heskey0

using System;
using System.Diagnostics;
using DG.Tweening;
using QFramework;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSkillMaster : MonoBehaviour
{
    private SkillManager _manager;

    public static ReactiveProperty<EquipEnum> curEquip = new ReactiveProperty<EquipEnum>(EquipEnum.Pencil);

    private void Start()
    {
        _manager = SkillManager.Instance.SetOwner(GetComponent<Player>());

        UIManager.Instance.SendMsg(EventMsgPool.Instance.Allocate(EquipEnum.Pencil,
            (int) UIEventEnum.Main.ChangeEquip));


        curEquip.Subscribe(value =>
        {
            QMsgCenter.Instance.SendMsg(EventMsgPool.Instance.Allocate(
                ((int) value).ToString()
                , (int) UIEventEnum.Main.ChangeEquip
            ));
            
        });


        this.UpdateAsObservable()
            .Subscribe(_ =>
            {
                _manager.runcd(Time.deltaTime);

                if (!Player.receiveInput && Input.GetKeyDown(GameConfig.Instance.playerAttackKey) &&
                    curEquip.Value == EquipEnum.Death)
                {
                    QMsgCenter.Instance.SendMsg(EventMsgPool.Instance.Allocate(
                        ((int) EquipEnum.DeathEnd).ToString()
                        , (int) GameEventEnum.Skill.ChangeEquip
                    ));

                    QMsgCenter.Instance.SendMsg(EventMsgPool.Instance.Allocate(
                        ((int) EquipEnum.DeathEnd).ToString()
                        , (int) GameEventEnum.Skill.Attack
                    ));
                }

                if (Player.receiveInput && Input.GetKeyDown(GameConfig.Instance.playerAttackKey))
                {
                    QMsgCenter.Instance.SendMsg(EventMsgPool.Instance.Allocate(
                        ((int) curEquip.Value).ToString()
                        , (int) GameEventEnum.Skill.Attack
                    ));
                    //_manager.Cast((int)_curEquip.Value, 0);
                }
            }).AddTo(this);
    }

    public static void ChangeEquip(EquipEnum equip)
    {
        curEquip.Value = equip;
    }
}