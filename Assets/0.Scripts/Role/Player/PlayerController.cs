//code by 赫斯基皇
//https://space.bilibili.com/455965619
//https://github.com/Heskey0

using System;
using UnityEngine;
using System.Collections;
using UniRx;
using UniRx.Triggers;

public class PlayerController : MonoBehaviour
{
    [HideInInspector] public bool isInAir = false;


    [HideInInspector] public float speed;
    [HideInInspector] public float jumpForce;
    [HideInInspector] public int jump;

    [HideInInspector] public int _jump = 2; //剩下的跳跃次数
    private Rigidbody2D _rb;
    private Animator _animator;
    private Player _player;

    /*************************************************************************/
    /******************************  Start  ******************************/
    /*************************************************************************/
    void Start()
    {
        speed = GameConfig.Instance.playerSpeed;
        jumpForce = GameConfig.Instance.playerJumpForce;
        jump = GameConfig.Instance.playerJump;

        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _player = GetComponent<Player>();
        _jump = jump;

        transform.Find("foot")
            .OnTriggerEnter2DAsObservable()
            .Where(c => c.gameObject.CompareTag("Floor"))
            .Subscribe(c =>
            {
                isInAir = false;
                _jump = jump;
            })
            .AddTo(this);
        transform.Find("foot")
            .OnTriggerExit2DAsObservable()
            .Where(c => c.gameObject.CompareTag("Floor"))
            .Subscribe(c =>
            {
                isInAir = true;
                _jump = jump - 1;
                StartCoroutine(jumpEnumerator());
            })
            .AddTo(this);


        this.UpdateAsObservable()
            .Where(_ => Player.receiveInput && Input.GetKeyDown(GameConfig.Instance.playerJumpKey))
            .Where(_ => !isInAir)
            .Subscribe(_ => { Jump(); })
            .AddTo(this);

        this.FixedUpdateAsObservable()
            .Subscribe(_ => { Movement(); })
            .AddTo(this);
    }

    /*************************************************************************/
    /******************************  Function  ******************************/
    /*************************************************************************/

    void Movement()
    {
        float h = Input.GetAxisRaw("Horizontal");
        if (!Player.receiveInput)
        {
            _rb.velocity = new Vector2(0, _rb.velocity.y);
            return;
        }

        _animator.SetFloat("MoveSpeed", Mathf.Abs(h));
        if (h != 0)
        {
            _player.isLeftward = h < 0;
        }

        transform.localScale = _player.isLeftward ? new Vector3(-9, 9, 1) : new Vector3(9, 9, 1);
        _rb.velocity = new Vector2(h * speed, _rb.velocity.y);
    }

    void Jump()
    {
        _rb.velocity = new Vector2(_rb.velocity.x, 0);
        //********************************************
        SoundController.instance.JumpAudio();
        //**********************************************
        _rb.AddForce(new Vector2(0, jumpForce));
    }


    /*************************************************************************/
    /******************************  协程  ******************************/
    /*************************************************************************/
    IEnumerator jumpEnumerator()
    {
        yield return null;
        while (!Input.GetKeyDown(GameConfig.Instance.playerJumpKey)) //无输入
        {
            if (!isInAir) //落到地面，则退出协程
            {
                _jump = 0;
                break;
            }

            yield return null;
        }

        if (Player.receiveInput)
        {
            //在空中按下空格
            _jump--;

            if (_jump >= 0)
            {
                Jump();
                StartCoroutine(jumpEnumerator());
            }
        }
    }
}