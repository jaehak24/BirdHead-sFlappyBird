using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey;

public class Bird : MonoBehaviour {
    private const float JUMP_AMOUNT = 100f;
    private Rigidbody2D birdrigidbody2D;

    private static Bird instance;

    public static Bird GetInstance()
    {
        return instance;
    }
    public event EventHandler OnDied;
    public event EventHandler OnStartedPlaying;
    private State state;

    private enum State
    {
        WaitingToStart,
        Playing,
        Dead,
    }
    private void Awake()
    {
        instance = this;
        birdrigidbody2D = GetComponent<Rigidbody2D>();
        birdrigidbody2D.bodyType = RigidbodyType2D.Static;
        state = State.WaitingToStart; // wake 단계에서 시작하기를 기다림
    }

    private void Update()
    {
        switch (state)
        {
            default:
            case State.WaitingToStart:
                if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) //마우스 스페이스바 둘다 가능
                {
                    state = State.Playing;// 스페이스바나, 마우스 클릭을 할 시 게임을 playing 상태로 변경
                    birdrigidbody2D.bodyType = RigidbodyType2D.Dynamic; //dynamic 상태로 변경해줘야 함
                    Jump();
                    if (OnStartedPlaying != null) OnStartedPlaying(this, EventArgs.Empty);
                }
                break;
            case State.Playing:
                if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) //마우스 스페이스바 둘다 가능
                {
                    Jump();
                }
                break;
            case State.Dead:
                break;
        }
        
    }
    private void Jump() {
        birdrigidbody2D.velocity=Vector2.up*JUMP_AMOUNT;// 해당 벡터에 점프 어만트만큼 올라감
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        birdrigidbody2D.bodyType = RigidbodyType2D.Static;
        if (OnDied != null) OnDied(this, EventArgs.Empty);
    }

}
