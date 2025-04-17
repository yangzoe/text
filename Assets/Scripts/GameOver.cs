using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GameOver : MonoBehaviour
{
    public Player player;
    public GameObject gameOver;
    public GameObject gameOverAnimation;
    bool isGameOver = false;
    public GameObject audioSource_gameOver;
    public GameObject audioSource_bgMusic;
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
    }
    void Update()
    {
        if (player.currentState == Player.State.Over && !isGameOver)//ËÀÍö×´Ì¬ÅÐ¶Ï
        {
            gameOverAnimation.SetActive(true);
            audioSource_gameOver.SetActive(true);
            audioSource_bgMusic.SetActive(false);
            
            StartCoroutine(gameOverRoutine());
        }
    }
    IEnumerator gameOverRoutine()
    {
        yield return new WaitForSeconds(2.2f);
        gameOver.SetActive(true);
    }
}