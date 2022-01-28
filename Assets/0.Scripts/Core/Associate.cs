//code by 赫斯基皇
//https://space.bilibili.com/455965619
//https://github.com/Heskey0

using System;
using MiscUtil;
using QFramework;
using QFramework.Example;
using UnityEngine;

public static class Associate
{
    public static void Do(EquipEnum equip)
    {
        Debug.Log("associate");
        Time.timeScale = 0;
        int equipId = (int) equip;

        switch (equip)
        {
            case EquipEnum.Pencil:
                Choose(EquipEnum.ArtKnife, EquipEnum.Paper);
                break;
            case EquipEnum.ArtKnife:
                Choose(EquipEnum.ChickenKnife, EquipEnum.Death);
                break;
            case EquipEnum.Paper:
                Choose(EquipEnum.Camera, EquipEnum.PaperFan);
                break;
            case EquipEnum.Death:
                Choose(EquipEnum.HpBar, EquipEnum.Ps);
                break;

            case EquipEnum.ChickenKnife:
                Choose(EquipEnum.Mother, EquipEnum.Death);
                break;
            case EquipEnum.Camera:
                Choose(EquipEnum.Ps, EquipEnum.Summer);
                break;
            case EquipEnum.PaperFan:
                Choose(EquipEnum.Summer, EquipEnum.Japan);
                break;
            case EquipEnum.Summer:
                Choose(EquipEnum.BigSister, EquipEnum.GamePad);
                break;

            case EquipEnum.Japan:
                Choose(EquipEnum.ACG, EquipEnum.Katana);
                break;
            case EquipEnum.Katana:
                Choose(EquipEnum.Death, EquipEnum.ChickenKnife);
                break;
            case EquipEnum.ACG:
                Choose(EquipEnum.Death, EquipEnum.Pencil);
                break;
            case EquipEnum.BigSister:
                Choose(EquipEnum.Mother, EquipEnum.ACG);
                break;

            case EquipEnum.HpBar:
                Choose(EquipEnum.Key, EquipEnum.GamePad);
                break;
            case EquipEnum.Key:
                break;
            case EquipEnum.GamePad:
                Choose(EquipEnum.Mother, EquipEnum.ACG);
                break;
            case EquipEnum.Ps:
                Choose(EquipEnum.GamePad, EquipEnum.Key);
                break;
        }
    }

    private static void Choose(EquipEnum ea, EquipEnum eb)
    {
        UIKit.OpenPanel<PromptPanel>(new PromptPanelData(ea, eb));
    }
}