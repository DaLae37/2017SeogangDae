﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour {

    public static GameManager instance;
    public GameObject chocoImage;
    public GameObject mintImage;

    public GameObject runImage;

    public GameObject mintPrefab;
    public GameObject chocoPrefab;

    public GameObject Win;
    public GameObject Lose;

    //public Image myHpBar;
    //public Image opposeHpBar;
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        NetworkManager.instance.EmitGameLoad();
    }
    private void Update()
    {
        if (PlayerDataManager.instance.my.controller != null && PlayerDataManager.instance.my.controller.isDone)
        {
            if (PlayerDataManager.instance.my.controller.isWin)
            {
                Win.SetActive(true);
            }
            else
            {
                Lose.SetActive(true);
            }
        }
    }
    public void mainScene()
    {
        if (PlayerDataManager.instance.my.controller != null && PlayerDataManager.instance.my.controller.isDone)
        {
            SceneManager.LoadScene("mainScene");
        }
    }
    public void StartGame() {
        for (int i = 0; i < NetworkManager.instance.userList.Count; i++) {
            GameObject g;
            if (NetworkManager.instance.userList[i].isCat)
            {
                g = Instantiate(chocoPrefab);
                g.transform.position = new Vector3(Random.Range(-17, 17), 10, Random.Range(-17, -13));
            }
            else
            {
                g = Instantiate(mintPrefab);
                g.transform.position = new Vector3(Random.Range(-17, 17), 10, Random.Range(17, 13));
            }
            g.name = NetworkManager.instance.userList[i].name;
            NetworkManager.instance.userList[i].controller = g.GetComponent<PlayerController>();
            if (PlayerDataManager.instance.my.name == NetworkManager.instance.userList[i].name) {
                g.GetComponent<PlayerController>().SetLocal(true);
                if (PlayerDataManager.instance.my.isCat)
                {
                    chocoImage.SetActive(true);
                }
                else
                {
                    mintImage.SetActive(true);
                }
            }
        }

        NetworkManager.instance.SetAllUserInput(true);
    }
    public void shoot()
    {
        if(PlayerDataManager.instance.my.name == PlayerPrefs.GetString("name"))
            PlayerDataManager.instance.my.controller.SendBulllet();     
    }
}
