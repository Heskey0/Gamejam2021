//code by 赫斯基皇
//https://space.bilibili.com/455965619
//https://github.com/Heskey0

using System;
using UniRx;
using UnityEngine;

public class Role : MonoBehaviour
{
    [HideInInspector] public bool isLeftward;
    [HideInInspector] public float fullHp;
    [HideInInspector] public FloatReactiveProperty curHp;
    [HideInInspector] public bool invincible = false;    //是否不受伤害
    [HideInInspector] public bool isKinematic = false;   //是否不受力

    [HideInInspector] public Animator animator;
    
    protected Cinemachine.CinemachineCollisionImpulseSource MyInpulse;

}