using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public static float moveSpeed = 5.5f;

    private int score = 0;
    public int playerHealth;
    //public string gameover = "Game Over, Try Again";
    //public string congrats = "Congratulations!";

    public TMP_Text scoreText;

    public GameObject menuSet;
    public GameObject startPage;
    public GameObject resultPage;
    //public GameObject endingPage;
    //public GameObject normalText;
    //public GameObject goodText;
    //public GameObject announceText;

    //private bool isEnding = false;

    //public GameObject[] UIHealth;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            // 이미 인스턴스가 존재하면, 중복된 GameManager 객체를 파괴
            Destroy(gameObject);
            return;
        }

        // 게임 매니저가 씬 전환 시에도 파괴되지 않도록 설정
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        resultPage.SetActive(false);
        //endingPage.SetActive(false);
        //goodText.SetActive(false);
        //normalText.SetActive(false);
        UpdateScoreText();
        menuSet.SetActive(false);
        Time.timeScale = 0f;
        startPage.SetActive(true);
        //endText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && startPage.activeSelf)
        {
            GameStart();
        }

        if (Input.GetButtonDown("Cancel") && !startPage.activeSelf)
        {
            if (menuSet.activeSelf)
            {
                GameStart();
            }
            else
            {
                PauseGame();
            }
        }

    }

    public void GameStart()
    {
        Debug.Log("GameStart 메서드 호출됨");
        startPage.SetActive(false);
        menuSet.SetActive(false);
        Time.timeScale = 1f;
    }

    public void PauseGame()
    {
        menuSet.SetActive(true);
        Time.timeScale = 0f;
    }

    public void GameOver()
    {
        resultPage.SetActive(true);
        GameStop();
    }

    /*
    public void ShowEndText()
    {
        endText.gameObject.SetActive(true);
    }
    */

    public void DecreasePlayerHealth()
    {
        if (playerHealth > 0)
        {
            playerHealth--;

            // playerHealth 값이 배열의 범위 내에 있는지 확인

            Debug.Log("Player Health: " + playerHealth);

            if (playerHealth <= 0)
            {
                GameOver();
            }
        }
    }


    public void AddScore(int value)
    {
        score += value;
        UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
            scoreText.text = "Score: "+ score ;
    }

    //public void GameEnding()
    //{
    //    Debug.Log("ending");
    //    isEnding = true;
    //    endingPage.SetActive(true);
    //    if(score<100)
    //        normalText.SetActive(true);
    //    else
    //        goodText.SetActive(true);
    //    Invoke("announceText.SetActive(true)", 4);
    //    if (Input.GetMouseButtonDown(0) && isEnding)
    //    {
    //        GameStop();
    //    }
    //}

    public void GameStop()
    {
        Time.timeScale = 0f;
    }
}