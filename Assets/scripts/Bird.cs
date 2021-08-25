using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey;

public class Bird : MonoBehaviour {
    private const float JUMP_AMOUNT = 100f;
    private Rigidbody2D birdrigidbody2D;

    private void Awake()
    {
        birdrigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)||Input.GetMouseButtonDown(0)) //마우스 스페이스바 둘다 가능
        {
            Jump();
        }
    }
    private void Jump() {
        birdrigidbody2D.velocity=Vector2.up*JUMP_AMOUNT;// 해당 벡터에 점프 어만트만큼 올라감
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        CMDebug.TextPopupMouse("Dead!");
    }

}
