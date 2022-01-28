//code by 赫斯基皇
//https://space.bilibili.com/455965619
//https://github.com/Heskey0

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using DG.Tweening;
using QFramework;
using QFramework.Example;
using UniRx.Triggers;
using Unity.VisualScripting;
using UnityEngine;
using UniRx;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

public class SkillAttack : SkillBase
{
    public int SkillId { get; }
    public float Cd { get; set; }

    public float[] fullCds = new float[(int) EquipEnum.End];
    public float[] cds = new float[(int) EquipEnum.End];
    public static IntReactiveProperty[] life = new IntReactiveProperty[(int) EquipEnum.End];

    private GameConfig _config = GameConfig.Instance;
    private Animator _animator;
    public List<TimeLine> timeLines = new List<TimeLine>();

    public SkillAttack(SkillManager manager, Role owner) : base(manager, owner)
    {
        //初始化cd
        fullCds = _config.cds;
        for (int i = 0; i < _config.cds.Length; i++)
        {
            fullCds[i] = _config.cds[i];
            cds[i] = 0;
        }

        //初始化life
        for (int i = 0; i < _config.life.Length; i++)
        {
            life[i] = new IntReactiveProperty(_config.life[i]);
        }

        _animator = owner.GetComponent<Animator>();
        for (int i = 0; i < (int) EquipEnum.End; i++)
        {
            timeLines.Add(new TimeLine());
        }

        InitTimeLine();
    }

    public void RunTimeLine(float dt)
    {
        for (int i = 0; i < timeLines.Count; i++)
        {
            timeLines[i].Loop(dt);
        }
    }


    protected override void ProcessMsg(int eventId, QMsg msg)
    {
        switch (eventId)
        {
            case (int) GameEventEnum.Skill.Attack:
                Cast((EquipEnum) int.Parse((msg as EventMsg).Msg as string));
                break;
        }
    }

    public override void Cast(EquipEnum equip, int param = 0)
    {
        var equipId = (int) equip;
        if (cds[equipId] <= 0)
        {
            cds[equipId] = fullCds[equipId];
            if (equip != EquipEnum.Summer)
            {
                life[equipId].Value--;
            }

            if (life[equipId].Value <= 0)
            {
                Player.receiveInput = false;
                Associate.Do(equip);
                //重置life
                for (int i = 0; i < _config.life.Length; i++)
                {
                    life[i].Value = _config.life[i];
                }

                return;
            }

            timeLines[equipId].Start();
        }
    }

    //**********************************************************************//
    //-----------------------------  装备效果  
    //**********************************************************************//
    private void InitTimeLine()
    {
        #region 0-4

        ///////////////////////////////////////////////////////////////////////////////////////////
        //pencil
        var pencilAtk = _config.pencilAtk;
        var pencilForce = _config.pencilForce;
        var pencilRadius = _config.pencilRadius;
        var pencilSettleTime = _config.pencilSettleTime;
        timeLines[(int) EquipEnum.Pencil].AddEvent(0, 0, id => Debug.Log("Pencil"));
        timeLines[(int) EquipEnum.Pencil].AddEvent(0, 1, id => _animator.SetTrigger("PencilAtk"));
        timeLines[(int) EquipEnum.Pencil].AddEvent(0, 2, id =>
        {
            var colliderObj = ResourcesMgr.Instance.QfGetInstance("SettlementObj");
            colliderObj.transform.position = owner.isLeftward
                ? owner.transform.position + pencilRadius * Vector3.left
                : owner.transform.position - pencilRadius * Vector3.left;
            var settlement = colliderObj.AddComponent<SettlementObj>();

            settlement.Init(pencilRadius, 0.1f, c =>
            {
                if (!c.CompareTag("Enemy"))
                    return;
                var enemy = c.GetComponent<Enemy>();

                var dir = (c.transform.position - owner.transform.position).normalized;
                enemy.OnAttack(pencilAtk, pencilForce * dir);
            });
        });
        timeLines[(int) EquipEnum.Pencil].AddEvent(0f, 1, id => _animator.SetTrigger("pencilBT"));

        //paper
        var paperTime = _config.paperTime;
        timeLines[(int) EquipEnum.Paper].AddEvent(0, 0, id => Debug.Log("Paper"));
        timeLines[(int) EquipEnum.Paper].AddEvent(0, 1, id => _animator.SetTrigger("PaperAtk"));
        timeLines[(int) EquipEnum.Paper].AddEvent(0, 2, id => owner.invincible = true); //无敌
        timeLines[(int) EquipEnum.Paper].AddEvent(paperTime, 3, id => owner.invincible = false); //解除无敌
        timeLines[(int) EquipEnum.Paper].AddEvent(0, 4, id => _animator.SetTrigger("paperBT"));

        //artKnife
        var artKnifeAtk = GameConfig.Instance.artKnifeAtk;
        var artKnifeForce = GameConfig.Instance.artKnifeForce;
        var artKnifeRadius = GameConfig.Instance.artKnifeRadius;
        var artKnifeSettleTime = GameConfig.Instance.artKnifeSettleTime;
        timeLines[(int) EquipEnum.ArtKnife].AddEvent(0, 0, id => Debug.Log("ArtKnife"));
        timeLines[(int) EquipEnum.ArtKnife].AddEvent(0, 1, id => _animator.SetTrigger("ArtKnifeAtk"));
        timeLines[(int) EquipEnum.ArtKnife].AddEvent(0, 2, id =>
        {
            var colliderObj = ResourcesMgr.Instance.QfGetInstance("SettlementObj");
            colliderObj.transform.position = owner.isLeftward
                ? owner.transform.position + artKnifeRadius * Vector3.left
                : owner.transform.position - artKnifeRadius * Vector3.left;
            var settlement = colliderObj.AddComponent<SettlementObj>();

            settlement.Init(artKnifeRadius, artKnifeSettleTime, c =>
            {
                if (!c.CompareTag("Enemy"))
                    return;
                var enemy = c.GetComponent<Enemy>();

                var dir = (c.transform.position - owner.transform.position).normalized;
                enemy.OnAttack(artKnifeAtk, artKnifeForce * dir);
            });
        });
        timeLines[(int) EquipEnum.ArtKnife].AddEvent(0, 1, id => _animator.SetTrigger("artKnifeBT"));

        //paperFan
        var paperFanAtk = _config.paperFanAtk;
        var paperFanForce = _config.paperFanForce;
        var paperFanRadius = _config.paperFanRadius;
        var paperFanSettleTime = _config.paperFanSettleTime;
        timeLines[(int) EquipEnum.PaperFan].AddEvent(0, 0, id => Debug.Log("PaperFan"));
        timeLines[(int) EquipEnum.PaperFan].AddEvent(0, 1, id => _animator.SetTrigger("PaperFanAtk"));
        timeLines[(int) EquipEnum.PaperFan].AddEvent(0, 2, id =>
        {
            var colliderObj = ResourcesMgr.Instance.QfGetInstance("SettlementObj");
            colliderObj.transform.position = owner.isLeftward
                ? owner.transform.position + paperFanRadius * Vector3.left
                : owner.transform.position - paperFanRadius * Vector3.left;
            var settlement = colliderObj.AddComponent<SettlementObj>();

            settlement.Init(paperFanRadius, paperFanSettleTime, c =>
            {
                if (!c.CompareTag("Enemy"))
                    return;
                var enemy = c.GetComponent<Enemy>();

                var dir = (c.transform.position - owner.transform.position).normalized;
                enemy.OnAttack(paperFanAtk, paperFanForce * dir);
            });
        });
        timeLines[(int) EquipEnum.PaperFan].AddEvent(0, 1, id => _animator.SetTrigger("paperFanBT"));

        #endregion

        #region 5-8

        ///////////////////////////////////////////////////////////////////////////////////////////
        //camera
        var cameraTime = _config.cameraTime;
        var cameraSettlement = _config.cameraSettlement;
        var cameraRadius = _config.cameraRadius;
        timeLines[(int) EquipEnum.Camera].AddEvent(0, 0, id => Debug.Log("Camera"));
        timeLines[(int) EquipEnum.Camera].AddEvent(0, 1, id => _animator.SetTrigger("CameraAtk"));
        timeLines[(int) EquipEnum.Camera].AddEvent(0, 1, id =>
        {
            var colliderObj = ResourcesMgr.Instance.QfGetInstance("SettlementObj");
            colliderObj.transform.position = owner.isLeftward
                ? owner.transform.position + cameraRadius * Vector3.left
                : owner.transform.position - cameraRadius * Vector3.left;
            var settlement = colliderObj.AddComponent<SettlementObj>();

            settlement.Init(cameraRadius, cameraSettlement, c =>
            {
                if (!c.CompareTag("Enemy"))
                    return;
                var enemy = c.GetComponent<Enemy>();


                enemy.Sleep(cameraTime);
            });
        });
        timeLines[(int) EquipEnum.Camera].AddEvent(0, 1, id => _animator.SetTrigger("cameraBT"));

        //death
        timeLines[(int) EquipEnum.Death].AddEvent(0, 0, id => Debug.Log("Death"));
        timeLines[(int) EquipEnum.Death].AddEvent(0, 1, id => _animator.SetTrigger("DeathAtk")); //倒下
        timeLines[(int) EquipEnum.Death].AddEvent(0, 1, id =>
        {
            Player.receiveInput = false;
            owner.invincible = true; //不受伤害
            owner.isKinematic = true; //不受力
        });
        timeLines[(int) EquipEnum.DeathEnd].AddEvent(0, 0, id => Debug.Log("DeathEnd"));
        timeLines[(int) EquipEnum.DeathEnd].AddEvent(0, 1, id => _animator.SetTrigger("deathBT"));
        timeLines[(int) EquipEnum.DeathEnd].AddEvent(0, 1, id =>
        {
            Player.receiveInput = true;
            owner.invincible = false; //不受伤害
            owner.isKinematic = false; //不受力

            QMsgCenter.Instance.SendMsg(EventMsgPool.Instance.Allocate(
                ((int) EquipEnum.Death).ToString()
                , (int) GameEventEnum.Skill.ChangeEquip
            ));
        });

        //chickenKnife
        var chickenKnifeAtk = _config.chickenKnifeAtk;
        var chickenKnifeForce = _config.chickenKnifeForce;
        var chickenKnifeRadius = _config.chickenKnifeRadius;
        var chickenKnifeSettleTime = _config.chickenKnifeSettleTime;
        timeLines[(int) EquipEnum.ChickenKnife].AddEvent(0, 0, id => Debug.Log("chickenKnife"));
        timeLines[(int) EquipEnum.ChickenKnife].AddEvent(0, 1, id => _animator.SetTrigger("ChickenKnifeAtk"));
        timeLines[(int) EquipEnum.ChickenKnife].AddEvent(0, 2, id =>
        {
            var colliderObj = ResourcesMgr.Instance.QfGetInstance("SettlementObj");
            colliderObj.transform.position = owner.isLeftward
                ? owner.transform.position + chickenKnifeRadius * Vector3.left
                : owner.transform.position - chickenKnifeRadius * Vector3.left;
            var settlement = colliderObj.AddComponent<SettlementObj>();

            settlement.Init(chickenKnifeRadius, chickenKnifeSettleTime, c =>
            {
                if (!c.CompareTag("Enemy"))
                    return;
                var enemy = c.GetComponent<Enemy>();

                var dir = (c.transform.position - owner.transform.position).normalized;
                enemy.OnAttack(chickenKnifeAtk, chickenKnifeForce * dir);
            });
        });
        timeLines[(int) EquipEnum.ChickenKnife].AddEvent(0, 1, id => _animator.SetTrigger("deathBT"));
        //timeLines[(int) EquipEnum.ChickenKnife].AddEvent(0, 1, id => _animator.SetTrigger("chickenKnifeBT"));

        //HpBar
        var hpBarAtk = _config.hpBarAtk;
        var hpBarForce = _config.hpBarForce;
        var hpBarRadius = _config.hpBarRadius;
        var hpBarSettleTime = _config.hpBarSettleTime;
        timeLines[(int) EquipEnum.HpBar].AddEvent(0, 0, id => Debug.Log("HpBar"));
        timeLines[(int) EquipEnum.HpBar].AddEvent(0, 1, id => _animator.SetTrigger("HpBarAtk"));
        timeLines[(int) EquipEnum.HpBar].AddEvent(0, 1, id =>
        {
            owner.invincible = true; //不受伤害
            owner.isKinematic = true; //不受力

            var colliderObj = ResourcesMgr.Instance.QfGetInstance("SettlementObj");
            colliderObj.transform.position = owner.isLeftward
                ? owner.transform.position + hpBarRadius * Vector3.left
                : owner.transform.position - hpBarRadius * Vector3.left;
            var settlement = colliderObj.AddComponent<SettlementObj>();

            settlement.Init(chickenKnifeRadius, chickenKnifeSettleTime, c =>
            {
                if (!c.CompareTag("Enemy"))
                    return;
                var enemy = c.GetComponent<Enemy>();

                var dir = (c.transform.position - owner.transform.position).normalized;
                enemy.OnAttack(chickenKnifeAtk, chickenKnifeForce * dir);
            });
        });
        timeLines[(int) EquipEnum.HpBar].AddEvent(0, 1, id => _animator.SetTrigger("hpBarBT"));

        #endregion

        #region 9-12

        ///////////////////////////////////////////////////////////////////////////////////////////
        //katana
        var katanaAtk = _config.katanaAtk;
        var katanaForce = _config.katanaForce;
        var katanaRadius = _config.katanaRadius;
        var katanaSettleTime = _config.katanaSettleTime;
        timeLines[(int) EquipEnum.Katana].AddEvent(0, 0, id => Debug.Log("Katana"));
        timeLines[(int) EquipEnum.Katana].AddEvent(0, 1, id => _animator.SetTrigger("KatanaAtk"));
        timeLines[(int) EquipEnum.Katana].AddEvent(0, 2, id =>
        {
            var colliderObj = ResourcesMgr.Instance.QfGetInstance("SettlementObj");
            colliderObj.transform.position = owner.isLeftward
                ? owner.transform.position + katanaRadius * Vector3.left
                : owner.transform.position - katanaRadius * Vector3.left;
            var settlement = colliderObj.AddComponent<SettlementObj>();

            settlement.Init(katanaRadius, katanaSettleTime, c =>
            {
                if (!c.CompareTag("Enemy"))
                    return;
                var enemy = c.GetComponent<Enemy>();

                var dir = (c.transform.position - owner.transform.position).normalized;
                enemy.OnAttack(katanaAtk, katanaForce * dir);
            });
        });
        timeLines[(int) EquipEnum.Katana].AddEvent(0, 1, id => _animator.SetTrigger("katanaBT"));

        //japan
        timeLines[(int) EquipEnum.Japan].AddEvent(0, 0, id => Debug.Log("Japan")); //TODO:Japan动画
        timeLines[(int) EquipEnum.Japan]
            .AddEvent(0, 0, id => { CoroutineMgr.Instance.StartCoroutine(JapanCoroutine()); });


        //ACG
        var acgAtk = _config.acgAtk;
        timeLines[(int) EquipEnum.ACG].AddEvent(0, 0, id => Debug.Log("ACG"));
        timeLines[(int) EquipEnum.ACG].AddEvent(0, 1, id => _animator.SetTrigger("deathBT"));
        timeLines[(int) EquipEnum.ACG].AddEvent(0, 0, id =>
        {
            var colliderObj = ResourcesMgr.Instance.QfGetInstance("SettlementObj");
            colliderObj.transform.position = owner.transform.position;
            var settlement = colliderObj.AddComponent<SettlementObj>();

            settlement.Init(_config.acgRadius, 0.5f, c =>
            {
                if (!c.CompareTag("Enemy"))
                    return;
                var enemy = c.GetComponent<Enemy>();
                enemy.Sleep(1f);

                //var enemyObjs = GameObject.FindGameObjectsWithTag("Enemy");
                var ACGObj = ResourcesMgr.Instance.QfGet<GameObject>("ACG");
                var acgInstance = ACGObj.Instantiate();
                acgInstance.transform.position = enemy.transform.position + new Vector3(1, 1, 0);
                TimerMgr.Instance.CreateTimer(1, 1, () =>
                {
                    acgInstance.transform.DOScale(0, 0.2f)
                        .SetEase(Ease.OutCirc);
                    Destroy(acgInstance, 0.2f);
                }).Start();


                TimerMgr.Instance.CreateTimer(2, 1, () =>
                {
                    enemy.OnAttack(acgAtk, Vector3.zero); //对全屏造成伤害
                }).Start();
            });
        });

        //cgCollection
        timeLines[(int) EquipEnum.CgCollection].AddEvent(0, 0, id => Debug.Log("CgCollection")); //TODO:cg动画
        timeLines[(int) EquipEnum.CgCollection].AddEvent(0, 1, id => { SceneMgr.Instance.Load(0, null); });

        #endregion

        #region 13-16

        ///////////////////////////////////////////////////////////////////////////////////////////
        //summer
        var summerRadius = _config.summerRadius;
        var summerForce = _config.summerForce;
        var summerTime = _config.cds[(int) EquipEnum.Summer];
        timeLines[(int) EquipEnum.Summer].AddEvent(0, 0, id => Debug.Log("Summer"));
        timeLines[(int) EquipEnum.Summer].AddEvent(0, 1, id => _animator.SetTrigger("summerBT")); //无敌
        timeLines[(int) EquipEnum.Summer].AddEvent(0, 2, id =>
        {
            owner.invincible = true; //不受伤害
            owner.isKinematic = true; //不受力

            var colliderObj = ResourcesMgr.Instance.QfGetInstance("SettlementObj");
            colliderObj.transform.position = owner.transform.position;
            var settlement = colliderObj.AddComponent<SettlementObj>();

            owner.gameObject.UpdateAsObservable().Subscribe(_ =>
            {
                colliderObj.transform.position = owner.transform.position;
            }).AddTo(colliderObj);
            settlement.Init(summerRadius, summerTime, c =>
            {
                if (!c.CompareTag("Enemy"))
                    return;
                var enemy = c.GetComponent<Enemy>();

                var dir = (c.transform.position - owner.transform.position).normalized;
                enemy.OnAttack(1000, summerForce * dir);
            });
        });
        timeLines[(int) EquipEnum.Summer].AddEvent(summerTime, 3, id => { Associate.Do(EquipEnum.Summer); });

        //bigSister
        var bigSisterRadius = _config.bigSisterRadius;
        var bigSisterAmount = _config.bigSisterAmount;
        timeLines[(int) EquipEnum.BigSister].AddEvent(0, 0, id => Debug.Log("BigSister")); //TODO:bigsister动画
        timeLines[(int) EquipEnum.BigSister].AddEvent(0, 1, id =>
        {
            var colliderObj = ResourcesMgr.Instance.QfGetInstance("SettlementObj");
            colliderObj.transform.position = owner.transform.position;
            var settlement = colliderObj.AddComponent<SettlementObj>();

            settlement.Init(bigSisterRadius, 0.1f, c =>
            {
                if (!c.CompareTag("Enemy"))
                    return;
            });
            Transform target;
            settlement.onDestroyCallback += () =>
            {
                target = settlement.ClosestEnemy();
                if (target == null)
                    return;
                _animator.SetTrigger("bigSister");
                CoroutineMgr.Instance.StartCoroutine(Shoot(target, bigSisterAmount));
            };
        });

        //gamePad
        timeLines[(int) EquipEnum.GamePad].AddEvent(0, 0, id => Debug.Log("GamePad"));
        timeLines[(int) EquipEnum.GamePad].AddEvent(0, 1, id => _animator.SetTrigger("deathBT"));
        timeLines[(int) EquipEnum.GamePad].AddEvent(0, 1,
            id =>
            {
                if (!GamePad.isAvailable)
                    return;
                var pad = ResourcesMgr.Instance.QfGetInstance("GamePad").GetComponent<GamePad>();
                pad.transform.position = owner.transform.position;
                pad.Init(owner.transform, new Vector2(10, 0) * _config.gamePadInitForce, owner.isLeftward);
            });

        //Mother
        timeLines[(int) EquipEnum.Mother].AddEvent(0, 0, id => Debug.Log("Mother"));
        timeLines[(int) EquipEnum.Mother].AddEvent(0, 1, id =>
        {
            SceneMgr.Instance.Load(2, () =>
            {
                UIKit.CloseAllPanel();
                UIKit.OpenPanel<EndPanel>(new EndPanelData("YouWin"));
            });
        });

        #endregion

        #region 17-

        //key
        timeLines[(int) EquipEnum.Key].AddEvent(0, 1, id => Debug.Log("Key"));
        timeLines[(int) EquipEnum.Key].AddEvent(0, 1, id =>
        {
            int newEquipId = Random.Range(0, (int) EquipEnum.End - 1);
            QMsgCenter.Instance.SendMsg(EventMsgPool.Instance.Allocate(
                newEquipId.ToString()
                , (int) GameEventEnum.Skill.ChangeEquip
            ));
        });

        //ps
        timeLines[(int) EquipEnum.Ps].AddEvent(0, 0, id => Debug.Log("Ps"));
        timeLines[(int) EquipEnum.Ps].AddEvent(0, 1,
            id =>
            {
                var Ps = ResourcesMgr.Instance.QfGetInstance("Ps").GetComponent<Ps>();
                Ps.Init(owner.isLeftward);
                Ps.transform.position = owner.transform.position;
            });

        #endregion
    }

    private IEnumerator Shoot(Transform target, int amount)
    {
        Camera.main.transform.Shake(GameConfig.Instance.cameraShakeDuration * 5,
            GameConfig.Instance.cameraShakeMagnitude / 2);

        GameObject bulletObj = ResourcesMgr.Instance.QfGet<GameObject>("Bullet");
        Bullet bullet = bulletObj.GetComponent<Bullet>();
        bullet.Init(target);
        bullet.transform.position = owner.transform.position;
        for (int i = 0; i < amount; i++)
        {
            GameObject.Instantiate(bullet);
            bullet.transform.position = owner.transform.position;
            yield return null;
            yield return null;
        }
    }

    private IEnumerator JapanCoroutine()
    {
        Player.receiveInput = false;

        GameObject circle = ResourcesMgr.Instance.QfGetInstance("JapanCircle");
        GameObject square = ResourcesMgr.Instance.QfGetInstance("JapanSquare");
        circle.transform.position = owner.transform.position + Vector3.up * 3;
        square.transform.position = owner.transform.position + Vector3.up * 3;


        float japanSpeed = _config.japanSpeed;
        float japanRadius = _config.japanRadius;
        float japanSettleTime = _config.japanSettleTime;

        while (true)
        {
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");

            circle.transform.Translate(h * Time.deltaTime * japanSpeed, v * Time.deltaTime * japanSpeed, 0);
            yield return null;
            if (Input.GetKeyDown(GameConfig.Instance.playerAttackKey))
            {
                var colliderObj = ResourcesMgr.Instance.QfGetInstance("SettlementObj"); //TODO:伤害结算物
                colliderObj.transform.position = circle.transform.position;
                var settlement = colliderObj.AddComponent<SettlementObj>();

                settlement.Init(japanRadius, japanSettleTime, c =>
                {
                    if (!c.CompareTag("Enemy"))
                        return;
                    var enemy = c.GetComponent<Enemy>();
                    //TODO:处理红色圈内的敌人
                    enemy.OnAttack(_config.japanAtk, Vector3.zero);
                });


                Player.receiveInput = true;
                Debug.Log("结束japan");
                GameObject.Destroy(circle);
                GameObject.Destroy(square);
                break;
            }
        }
    }
}