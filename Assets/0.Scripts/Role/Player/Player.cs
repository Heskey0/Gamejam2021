//code by 赫斯基皇
//https://space.bilibili.com/455965619
//https://github.com/Heskey0

using System;
using QFramework;
using QFramework.Example;
using UniRx.Triggers;
using Unity.VisualScripting;
using UnityEngine;
using UniRx;

public class Player : Role
{
    
    public static bool receiveInput = true;

    private void Awake()
    {

        
        
        fullHp = GameConfig.Instance.playerHp;
        curHp.Value = fullHp;

        tag = "Player";

        animator = GetComponent<Animator>();
        MyInpulse = GetComponent<Cinemachine.CinemachineCollisionImpulseSource>();
        this.AddComponent<PlayerController>();
        this.AddComponent<PlayerSkillMaster>();

        curHp.Subscribe(value =>
        {
            if (value<=0)
            {
                //TODO:
                //Debug.Log("主角死亡");
                SceneMgr.Instance.Load(2, () =>
                {
                    UIKit.CloseAllPanel();
                    UIKit.OpenPanel<EndPanel>(new EndPanelData("YouDie"));
                });
            }
        });

    }

    /// <summary>
    /// 受到攻击
    /// </summary>
    public void OnAttack(float damage, Vector3 force)
    {
        if (invincible)
            damage = 0;
        if (isKinematic)
            force = Vector3.zero;

        Debug.Log("角色受到伤害：" + damage + "||||受力：" + force);
        curHp.Value -= damage;
        GetComponent<Rigidbody2D>().AddForce(force);

        Shake();
        //Camera.main.transform.Shake(GameConfig.Instance.cameraShakeDuration, GameConfig.Instance.cameraShakeMagnitude);
    }

    private void Shake()
    {
        //********************************************
        SoundController.instance.BehitAudio();
        //**********************************************
        MyInpulse.GenerateImpulse();
    }
}