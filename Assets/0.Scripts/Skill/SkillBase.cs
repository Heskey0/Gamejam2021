//code by 赫斯基皇
//https://space.bilibili.com/455965619
//https://github.com/Heskey0

using System;
using QFramework;
using UnityEngine;

public class SkillBase : QMonoBehaviour
{
    public override IManager Manager => SkillManager.Instance;


    public int SkillId { get; }
    public Role owner;

    /// <summary>
    /// 0---1
    /// </summary>
    public float Cd { get; set; }

    public float fullCd;
    public SkillManager manager;

    public SkillBase(SkillManager manager, Role owner)
    {
        this.manager = manager;
        this.owner = owner;
    }

    public virtual void Cast(EquipEnum equip, int param = 0)
    {
    }
}