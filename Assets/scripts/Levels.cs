using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey;

public class Levels : MonoBehaviour
{
    private const float CAMERA_ORTHO_SIZE = 50f;
    private const float PIPE_WITDH = 7.8f;
    private const float PIPE_HEAD_HEIGHT = 3.75f;
    private const float PIPE_MOVE_SPEED = 30f;
    private const float PIPE_DESTROY_X_POSITION=-100f;
    private const float PIPE_SPAWN_X_POSITION = +100f;
    private const float BIRD_X_POSTION = 0f;

    private static Levels instance;// 다른 스크립트에서 호출할 때 사용할 인스턴스
    public static Levels GetInstance()
    {
        return instance;
    }

    private List<Pipe> pipeList;
    private int pipesPassedCount;
    private int pipeSpawned; // 파이프가 생산된 횟수
    private float PipeSpawnTimer;
    private float PipeSpawnTimerMax;
    private float gapSize;
    private State state;

    //난이도 조절
    public enum Difficulty
    {
        Easy, 
        Medium,
        Hard,
        Impossible,
        Hell,
    }
    public enum State
    {
        WaitingToStart,
        Playing,
        birdDead,
    }
    private void Awake()
    {
        instance = this;
        pipeList = new List<Pipe>();
        PipeSpawnTimerMax = 1f;//1초마다 생산되게끔 함
        SetDifficulty(Difficulty.Easy);
        state = State.WaitingToStart; 
    }
    private void Start()
    {
        //새가 죽었을 때의 event 처리
        Bird.GetInstance().OnDied += Bird_OnDied;
        Bird.GetInstance().OnStartedPlaying += Level_OnStartedPlaying;
    }
    private void Level_OnStartedPlaying(object sender, System.EventArgs e)
    {
        state = State.Playing;
    }
    private void Bird_OnDied(object sender, System.EventArgs e)
    {
        //CMDebug.TextPopupMouse("Dead!");
        state = State.birdDead;

    } // 1초 후에 게임 다시 시작

    private void Update()
    {
        if (state == State.Playing)
        {
            HandlePipeMovement();
            HandlePipeSpawning();
        }
               
    }
    private void HandlePipeSpawning()
    {
        PipeSpawnTimer -= Time.deltaTime;
        if (PipeSpawnTimer < 0)
        {
            //0보다 작으면 파이프를 하나 더 추가해랴 할 때
            PipeSpawnTimer += PipeSpawnTimerMax;//파이프 스폰타임 초기화
            float HeightEdgeLimit = 10f;//갭에다가 더할 최소 가중치
            float minHeight = gapSize *.5f+ HeightEdgeLimit;//최소 사이즈=최소 지정 사이즈+여백+1/2
            float TotlaHeight = CAMERA_ORTHO_SIZE * 2f;
            float MaxHegiht = TotlaHeight - gapSize * .5f-HeightEdgeLimit;
            float height = Random.Range(minHeight, MaxHegiht);
            CreateGapPipes(height, gapSize, PIPE_SPAWN_X_POSITION); // 가장 최초 위치서부터 파이프들이 생성됨
            

        }

    }
    private void HandlePipeMovement()
    {
        for(int i = 0; i < pipeList.Count; i++)
        {
           {
               Pipe pipe = pipeList[i];
                //파이프를 파이프 속도에 따라 왼쪽으로 이동
                bool isTotheRightoftheBird = pipe.GetXPosition() > BIRD_X_POSTION;
                pipe.Move();
                if(isTotheRightoftheBird&&pipe.GetXPosition() <= BIRD_X_POSTION&&pipe.IsBottom())
                {
                    //bird pass the pipe
                    pipesPassedCount++;
                }
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
    private void SetDifficulty(Difficulty difficulty)
    {
        switch (difficulty)
        {
            case Difficulty.Easy:
                gapSize = 50f;
                PipeSpawnTimerMax = 1.2f;
                break;
            case Difficulty.Medium:
                gapSize = 40f;
                PipeSpawnTimerMax = 1.1f;
                break;
            case Difficulty.Hard:
                gapSize = 30f;
                Debug.Log("Hard");
                PipeSpawnTimerMax = 1.0f;
                break;
            case Difficulty.Impossible:                
                gapSize = 23f;
                PipeSpawnTimerMax = 0.9f;
                Debug.Log("Impossible");
                break;
            case Difficulty.Hell:
                gapSize = 19f;
                PipeSpawnTimerMax = 0.85f;
                Debug.Log("HEll");
                break;
        }
    }
    private Difficulty GetDifficulty()
    {
        if (pipeSpawned >= 40) return Difficulty.Hell;
        if (pipeSpawned >= 30) return Difficulty.Impossible;
        if (pipeSpawned >= 20) return Difficulty.Hard;
        if (pipeSpawned >= 10) return Difficulty.Medium;
        else
            return Difficulty.Easy;


    }
    private void CreateGapPipes(float gapY, float gapSize, float xPosition)
    {
        CreatePipe(gapY-gapSize*.5f,xPosition,true);//아래쪽 파이프
        CreatePipe(CAMERA_ORTHO_SIZE * 2f - gapY - gapSize * .5f, xPosition, false);
        pipeSpawned++;
        SetDifficulty(GetDifficulty()); // 파이프를 소환할 때마다 setDifficulty를 통해서 난이도를 확인
    }
    private void CreatePipe(float height, float xPosition, bool isBottom)//하단 상단 파이프 빌드를 위한 bool 인자 추가
    {
        //파이프 헤드 
        Transform pipeHead = Instantiate(GameAssets.GetInstance().pfPipeHead);
        float pipeheadPostition;
        //파이프 헤드 위치 판별(상하)
        if (isBottom)
        {
            pipeheadPostition = -CAMERA_ORTHO_SIZE + height - PIPE_HEAD_HEIGHT * .38f;
        }
        else
            pipeheadPostition = CAMERA_ORTHO_SIZE - height + PIPE_HEAD_HEIGHT * .38f;
        pipeHead.position = new Vector3(xPosition, pipeheadPostition);
       

        //파이프 바디
        float bodyPosition;
        Transform pipeBody = Instantiate(GameAssets.GetInstance().pfPipeBody);
        if (isBottom)
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

        Pipe pipe = new Pipe(pipeHead, pipeBody, isBottom);// 새로운 파이프 선언
        pipeList.Add(pipe);

    }
    public int GetPipeSpawned()
    {
        return pipeSpawned;
    }
    public int getPipespassedCount()
    {
        return pipesPassedCount;
    }
    /*
     *싱글 파이프 클래스
     */
    private class Pipe
    {
        private Transform pipeHeadTransform;
        private Transform pipeBodyTransform;
        private bool isBottom;

        public Pipe(Transform pipeHeadTransform, Transform pipeBodyTransform, bool isBottom)
        {
            this.pipeHeadTransform = pipeHeadTransform;
            this.pipeBodyTransform = pipeBodyTransform;
            this.isBottom = isBottom;
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
        public bool IsBottom()
        {
            return isBottom;
        }
        public void DestroySelf()
        {
            Destroy(pipeBodyTransform.gameObject);
            Destroy(pipeHeadTransform.gameObject);
        }
    }


    
}
