using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CodeMonkey.Utils;

public class GameOverWindow : MonoBehaviour
{
    private Text scoreText;

    private void Awake()
    {
        
        scoreText = transform.Find("ScoreText").GetComponent<Text>();
        transform.Find("RetryBtn").GetComponent<Button_UI>().ClickFunc = () =>
          { UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene"); }; // 액션을 모노비헤이비어로 갖고 오는 함수
    }
    private void Start()
    {
        Hide();
        Bird.GetInstance().OnDied += Bird_OnDied;
        
    }
    private void Bird_OnDied(object sender, System.EventArgs e)
    {
        scoreText.text= Levels.GetInstance().getPipespassedCount().ToString();

        //새가 죽은면
        Show();
    }
    private void Hide()
    {
        gameObject.SetActive(false);
    }
    private void Show()
    {
        gameObject.SetActive(true);
    }




}
