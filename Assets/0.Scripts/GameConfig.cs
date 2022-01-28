//code by 赫斯基皇
//https://space.bilibili.com/455965619
//https://github.com/Heskey0

using System;
using UniRx;
using UnityEngine;

public class GameConfig : Singleton<GameConfig>
{
    public GameConfig()
    {
        //初始化使用寿命
        for (int i = 0; i < (int) EquipEnum.End; i++)
        {
            life[i] = 10;
        }

        life[(int) EquipEnum.Paper] = 5;
        life[(int) EquipEnum.Summer] = 2;
        life[(int) EquipEnum.ACG] = 5;
        life[(int) EquipEnum.Katana] = 10;
        life[(int) EquipEnum.Japan] = 5;
    }

    public float cameraShakeDuration = 0.15f;
    public float cameraShakeMagnitude = 0.4f;

    //**********************************************************************//
    //-----------------------------  Role  
    //**********************************************************************//
    public float playerHp = 100;
    public float enemyHp = 30;

    public float enemyAtk = 10;

    //**********************************************************************//
    //-----------------------------  PlayerController  
    //**********************************************************************//
    public float playerSpeed = 6; //移动速度
    public float playerJumpForce = 650; //跳跃力
    public int playerJump = 2; //跳跃次数

    public KeyCode playerJumpKey = KeyCode.Space;

    //**********************************************************************//
    //-----------------------------  EnemyController  
    //**********************************************************************//
    public float enemySpeed = 3;
    public float enemyTargetNear = 2; //最近巡逻点
    public float enemyTargetFar = 4; //巡逻点最远
    public float enemyIdleTimeMax = 5; //站立时间
    public float enemyIdleTimeMin = 3; //站立最短时间

    //**********************************************************************//
    //-----------------------------  PlayerSkill  
    //**********************************************************************//
    //cd
    public int[] life = new int[(int) EquipEnum.End];

    public float[] cds = new float[]
    {
        0.3f //pencil
        ,
        1f //paper
        ,
        0.5f //artKnife
        ,
        1f //paperFan
        ,

        1f //camera
        ,
        0f //death
        ,
        0f //deathEnd
        ,
        0.3f //chickenKnife
        ,
        0.5f //hpBar
        ,


        2f //katana
        ,
        6f //Japan
        ,
        6f //ACG
        ,
        0f //cgCollection

        ,
        8 //summer
        ,
        3 //bigSister
        ,
        4 //GamePad
        ,
        0 //Mother
        ,
        0 //Key
        ,
        0.5f //Ps
    };

    //atk
    public float pencilAtk = 3; //铅笔

    //纸
    public float artKnifeAtk = 2; //美工刀
    public float paperFanAtk = 0; //纸扇

    //相机
    //死亡
    public float chickenKnifeAtk = 3; //菜刀
    public float hpBarAtk = 1; //血条

    public float katanaAtk = 8; //武士刀

    public float japanAtk = 8; //日本

    public float acgAtk = 20; //二次元
    //cg集

    //夏天
    public float bigSisterAtk = 0.2f; //大姐姐

    public float gamePadAtk = 1f; //游戏机
    //妈妈

    public float psAtk = 1f; //ps


    //force
    public float pencilForce = 0;
    public float artKnifeForce = 0;
    public float paperFanForce = 5 * 50;

    public float chickenKnifeForce = 0;
    public float hpBarForce = 0;

    public float katanaForce = 1 * 50;

    public float gamePadForce = 1 * 50;
    public float summerForce = 1 * 50;

    public float psForce = 1 * 50;

    //range
    public float pencilRadius = 1.5f;
    public float artKnifeRadius = 1f;
    public float paperFanRadius = 3f;

    public float cameraRadius = 3f;
    public float chickenKnifeRadius = 1f;
    public float hpBarRadius = 2f;

    public float katanaRadius = 1f;
    public float acgRadius = 100;
    public float japanRadius = 1f;

    public float summerRadius = 2f;
    public float bigSisterRadius = 18f;

    //settleTime结算物结算时间
    public float pencilSettleTime = 0.5f;
    public float artKnifeSettleTime = 0.5f;
    public float paperFanSettleTime = 0.5f;

    public float cameraSettlement = 0.5f;
    public float chickenKnifeSettleTime = 0.5f;
    public float hpBarSettleTime = 0.5f;

    public float japanSettleTime = 0.5f;

    public float katanaSettleTime = 0.5f;


    //other其它配置
    public float paperTime = 3f; //盾牌格挡时间
    public float cameraTime = 3; //相机使静止时间
    public float japanSpeed = 8; //Japan移动速度
    public float gamePadInitSpeed = 8;
    public float gamePadInitForce = 1;
    public float psSpeed = 5;
    public float psLifeTime = 2;
    public int bigSisterAmount = 100;
    public KeyCode playerAttackKey = KeyCode.J;
}