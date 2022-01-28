//code by 赫斯基皇
//https://space.bilibili.com/455965619
//https://github.com/Heskey0

using System;
using System.Collections;
using Cinemachine;
using DG.Tweening;
using QFramework;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Enemy : Role
{
    public EnemyStateMachine fsm = new EnemyStateMachine();

    private Player _player;
    private float _time;
    private float _fullTime = 2f;

    private Rigidbody2D _rb;
    private GameObject _hpCanvas;
    private GameObject _hpBar;
    private GameObject _hpDeltaBar;

    private bool movable = true;

    public float enemySpeed;
    [Header("最近巡逻点")] public float enemyTargetNear;
    [Header("最远巡逻点")] public float enemyTargetFar;
    [Header("最大站立时间")] public float enemyIdleTimeMax;
    [Header("最小站立时间")] public float enemyIdleTimeMin;

    private float _lastDir = 1; //第一次往左走的概率大

    private void Awake()
    {
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        enemySpeed = GameConfig.Instance.enemySpeed;
        enemyTargetNear = GameConfig.Instance.enemyTargetNear;
        enemyTargetFar = GameConfig.Instance.enemyTargetFar;
        enemyIdleTimeMax = GameConfig.Instance.enemyIdleTimeMax;
        enemyIdleTimeMin = GameConfig.Instance.enemyIdleTimeMin;
        fullHp = GameConfig.Instance.enemyHp;
        curHp.Value = fullHp;

        _time = _fullTime;

        tag = "Enemy";

        _hpCanvas = transform.parent.Find("hpCanvas").gameObject;
        _hpBar = transform.parent.Find("hpCanvas/hpBar").gameObject;
        _hpDeltaBar = transform.parent.Find("hpCanvas/hpDeltaBar").gameObject;

        animator = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody2D>();
        MyInpulse = GetComponent<Cinemachine.CinemachineCollisionImpulseSource>();

        // transform.parent.Find("DamageRange").gameObject.OnTriggerEnter2DAsObservable()
        //     .Subscribe(c =>
        //     {
        //         Debug.Log("敌人碰到东西");
        //         if (c.CompareTag("Player"))
        //         {
        //             c.GetComponent<Player>().OnAttack(GameConfig.Instance.enemyAtk, Vector3.zero);
        //         }
        //     });
        this.UpdateAsObservable()
            .Subscribe(_ =>
            {
                _time -= Time.deltaTime;
                if (Vector3.Distance(_player.transform.position, transform.position) < 0.3f && _time < 0)
                {
                    _time = _fullTime;
                    _player.OnAttack(GameConfig.Instance.enemyAtk, Vector3.zero);
                }
            });


        curHp.Subscribe(value =>
        {
            if (value <= 0)
            {
                //TODO:死亡
                ResourcesMgr.Instance.GetInstance("PS").transform.position = transform.position;
                Destroy(transform.parent.gameObject);
            }
        });
    }

    private void Start()
    {
        InitStateMachine();

        curHp
            .Subscribe(value =>
            {
                var delta = value / fullHp;
                _hpBar.transform.DOScaleX(delta, 0.3f)
                    .SetEase(Ease.OutCirc);
            });
        curHp.Throttle(TimeSpan.FromMilliseconds(800))
            .Subscribe(value =>
            {
                var delta = value / fullHp;
                _hpDeltaBar.transform.DOScaleX(delta, 1.8f)
                    .SetEase(Ease.OutCirc);
            });
    }

    private void Update()
    {
        var enemyScale = transform.localScale;
        enemyScale.x = isLeftward ? -2.908031f : 2.908031f;
        transform.localScale = enemyScale;

        _hpCanvas.transform.position = transform.position;
    }

    #region FSMTranslation

    private void InitStateMachine()
    {
        fsm.AddState("Idle");
        fsm.AddState("Move");

        fsm.AddTranslation("Idle", "MoveTo", "Move", a =>
        {
            animator.SetTrigger("Move"); //TODO:动画
            float dir;
            if (_lastDir > 0)
                dir = Random.Range(-1, 0.3f);
            else
                dir = Random.Range(-0.3f, 1);

            float target;
            if (dir < 0)
                target = Random.Range(transform.position.x - enemyTargetFar,
                    transform.position.x - enemyTargetNear);
            else
                target = Random.Range(transform.position.x + enemyTargetNear,
                    transform.position.x + enemyTargetFar);

            CoroutineMgr.Instance.StartCoroutine(MoveTo(target));

            _lastDir = dir;
            //Debug.Log("MoveTo:" + target);
        });
        fsm.AddTranslation("Move", "Wait", "Idle", a =>
        {
            animator.SetTrigger("Idle"); //TODO:动画
            float t = Random.Range(enemyIdleTimeMin, enemyIdleTimeMax);

            CoroutineMgr.Instance.StartCoroutine(Wait(t));

            //Debug.Log("Wait:" + t);
        });

        fsm.Start("Idle");
        fsm.HandleEvent("MoveTo");
    }

    private IEnumerator MoveTo(float target)
    {
        float dis = 10;
        float t = 3;
        while (!dis.FloatEqual(0f))
        {
            if (target - transform.position.x > 0)
                dis = Mathf.Clamp(target - transform.position.x, 1, 2);
            else
                dis = Mathf.Clamp(target - transform.position.x, -2, -1);

            _rb.velocity = new Vector2(dis, _rb.velocity.y);

            t -= Time.deltaTime;
            if (t <= 0)
            {
                //Debug.Log("超时");
                break;
            }

            while (!movable)
            {
                yield return null;
            }

            yield return null;
        }

        //Debug.Log("到达目标");
        fsm.HandleEvent("Wait");
    }

    private IEnumerator Wait(float t)
    {
        while (t > 0)
        {
            t -= Time.deltaTime;
            while (!movable)
            {
                yield return null;
            }

            yield return null;
        }

        //Debug.Log("等待完毕");
        fsm.HandleEvent("MoveTo");
    }

    #endregion

    /// <summary>
    /// 受到攻击
    /// </summary>
    public void OnAttack(float damage, Vector3 force, bool shake = true)
    {
        curHp.Value = Mathf.Clamp(curHp.Value - damage, 0, fullHp);
        Debug.Log("敌人受到伤害:" + damage + "剩余血量:" + curHp.Value + "||||受力:" + force);

        GetComponent<Rigidbody2D>().AddForce(force);


        if (shake)
        {
            //********************************************
            SoundController.instance.HitAudio();
            //**********************************************
            Camera.main.transform.Shake(GameConfig.Instance.cameraShakeDuration,
                GameConfig.Instance.cameraShakeMagnitude);
        }
    }

    /// <summary>
    /// 静止不动
    /// </summary>
    public void Sleep(float t)
    {
        CoroutineMgr.Instance.StartCoroutine(SleepCoroutine(t));
    }

    private IEnumerator SleepCoroutine(float t)
    {
        movable = false;
        animator.speed = 0;
        fsm.HandleEvent("Wait");
        yield return new WaitForSeconds(t);

        movable = true;
        animator.speed = 1;
    }
}