﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class GameUI : MonoBehaviour
{

    public Image fadeCanvas;
    public GameObject gameOverUI;
    // Start is called before the first frame update
    void Awake()
    {
       FindObjectOfType<Player>().OnDeath += OnGameOver;

    }

   void OnGameOver()
    {
        StartCoroutine(Fade(Color.clear, Color.black, 1));
        gameOverUI.SetActive(true);
    }

    IEnumerator Fade(Color from, Color to, float time)
    {
        float speed = 1 / time;
        float percent = 0;

        while(percent < 1)
        {
            percent += Time.deltaTime * speed;
            fadeCanvas.color = Color.Lerp(from, to, percent);
            yield return null;
        }
    }

    //UI(Button)

    public void StartNewGame()
    {
        SceneManager.LoadScene(0);
    }
}
