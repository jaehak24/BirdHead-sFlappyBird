using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Levels : MonoBehaviour
{
    private const float CAMERA_ORTHO_SIZE = 50f;
    private const float PIPE_WITDH = 7.8f;
    private const float PIPE_HEAD_HEIGHT = 3.75f;
    private const float PIPE_MOVE_SPEED = 10f;
    private const float PIPE_DESTROY_X_POSITION=-100f;
    private const float PIPE_SPAWN_X_POSITION = +100f;

    private List<Pipe> pipeList;

    private float PipeSpawnTimer;
    private float PipeSpawnTimerMax;


    private void Awake()
    {
        pipeList = new List<Pipe>();
        PipeSpawnTimerMax = .5f;//0.5초마다 생산되게끔 함
    }
    private void Start()
    {
        CreatePipe(50f, 20f,true); //카메라 (-50f0,0) 기준으로 높이, x좌표
        CreatePipe(50f, 20f, false);
        CreateGapPipes(50f, 20f, 40f); 
    }

    private void Update()
    {
        HandlePipeMovement();
        HandlePipeSpawning();
    }
    private void HandlePipeSpawning()
    {
        PipeSpawnTimer -= Time.deltaTime;
        if (PipeSpawnTimer < 0)
        {
            //0보다 작으면 파이프를 하나 더 추가해랴 할 때
            PipeSpawnTimer += PipeSpawnTimerMax;//파이프 스폰타임 초기화
            CreateGapPipes(50f, 20f, PIPE_SPAWN_X_POSITION); // 가장 최초 위치서부터 파이프들이 생성됨

        }

    }
    private void HandlePipeMovement()
    {
        for(int i = 0; i < pipeList.Count; i++)
        {
           {
               Pipe pipe = pipeList[i];
               //파이프를 파이프 속도에 따라 왼쪽으로 이동
               pipe.Move();
               //리소스를 잡아 먹지 않기 위해 카메라 앵글을 벗어나면 파이프를 제거하는 것
               if (pipe.GetXPosition() < PIPE_DESTROY_X_POSITION)
               {
                   pipe.DestroySelf();
                   pipeList.Remove(pipe);
                   i--; //i--를 하지 않으면 다음 파이프의 파괴가 스킵되는 경우가 생긴다.
                    //if 문을 통한 예외이기 때문에
               }
           }

        }
        

    }
    private void CreateGapPipes(float gapY, float gapSize, float xPosition)
    {
        CreatePipe(gapY-gapSize*.5f,xPosition,true);//아래쪽 파이프
        CreatePipe(CAMERA_ORTHO_SIZE * 2f - gapY - gapSize * .5f, xPosition, false);
    }
    private void CreatePipe(float height, float xPosition, bool createBottom)//하단 상단 파이프 빌드를 위한 bool 인자 추가
    {
        //파이프 헤드 
        Transform pipeHead = Instantiate(GameAssets.GetInstance().pfPipeHead);
        float pipeheadPostition;
        //파이프 헤드 위치 판별(상하)
        if (createBottom)
        {
            pipeheadPostition = -CAMERA_ORTHO_SIZE + height - PIPE_HEAD_HEIGHT * .38f;
        }
        else
            pipeheadPostition = CAMERA_ORTHO_SIZE - height + PIPE_HEAD_HEIGHT * .38f;
        pipeHead.position = new Vector3(xPosition, pipeheadPostition);
       

        //파이프 바디
        float bodyPosition;
        Transform pipeBody = Instantiate(GameAssets.GetInstance().pfPipeBody);
        if (createBottom)
            bodyPosition = -CAMERA_ORTHO_SIZE;
        else {
            bodyPosition = CAMERA_ORTHO_SIZE;
            pipeBody.localScale = new Vector3(1, -1, 1); //상대 좌표가 y축으로 반전

        }
            
        pipeBody.position = new Vector3(xPosition, bodyPosition);
        



        //파이프 바디 셋업
        SpriteRenderer pipeBodySpriteRenderer=pipeBody.GetComponent<SpriteRenderer>();
        pipeBodySpriteRenderer.size = new Vector2(PIPE_WITDH, height);


        BoxCollider2D pipeBodyBoxCollider = pipeBody.GetComponent<BoxCollider2D>();
        pipeBodyBoxCollider.size = new Vector2(PIPE_WITDH, height);
        pipeBodyBoxCollider.offset = new Vector2(0f, height * .38f);

        Pipe pipe = new Pipe(pipeHead, pipeBody);// 새로운 파이프 선언
        pipeList.Add(pipe);

    }

    /*
     *하나의 파이프를 가리킬 때
     */
    private class Pipe
    {
        private Transform pipeHeadTransform;
        private Transform pipeBodyTransform;

        public Pipe(Transform pipeHeadTransform, Transform pipeBodyTransform)
        {
            this.pipeHeadTransform = pipeHeadTransform;
            this.pipeBodyTransform = pipeBodyTransform;
        }

        public void Move() 
        {
            pipeHeadTransform.position += new Vector3(-1, 0, 0) * PIPE_MOVE_SPEED * Time.deltaTime;
            pipeBodyTransform.position += new Vector3(-1, 0, 0) * PIPE_MOVE_SPEED * Time.deltaTime;
        }

        public float GetXPosition()
        {
            return pipeHeadTransform.position.x;
        }

        public void DestroySelf()
        {
            Destroy(pipeBodyTransform.gameObject);
            Destroy(pipeHeadTransform.gameObject);
        }
    }


    
}
