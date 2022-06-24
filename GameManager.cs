using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public GameObject playTime;
    public GameObject[] playerHp = new GameObject[3];
    public int HP = 2;
    public GameObject playerExp;
    TextMeshProUGUI playTimeText;
    public GameObject gameoverDisplay;
    bool isBattle = true;
    float realplaytime;
    public static GameManager inst;
    float exp = 0;
    float oneExp = 0;
    float defultExp = 0.1f;
    public GameObject[] SkillPanel = new GameObject[3];
    public GameObject skillPanelOn;
    public List<SkillCard> skillCard = new List<SkillCard>();
    SkillCard[] viewSkillCard = new SkillCard[3];
    public TextMeshProUGUI[] skillTitle = new TextMeshProUGUI[3];
    public TextMeshProUGUI[] skillMain = new TextMeshProUGUI[3];
    public TextMeshProUGUI[] skillFunc = new TextMeshProUGUI[3];
    int playerPowerUpLv = 1;
    public PlayerController player;
    bool isPanelOn = false;
    public Satellite satellite;
    bool isSelet = false;
    int getFuc;
    public Button[] cardButton = new Button[3];
    int CheckRan;
    public GameObject talkpanel;
    public TextMeshProUGUI MainTalk;
    bool isBossCome = false;
    bool isBossfirstCome = false;
    int index = 0;
    public CameraControoler mainCamera;
    public GameObject instBoss;
    BossController instBossScripts;

    bool isplayerMissileOn;
    bool isPlayerAllRoundShoot;
    bool isPlayerSateOn;
    bool isPlayerSateMissileOn;
    bool isPlayerSateAoe;

    int LV = 0;

    bool isAllSkill = false;

    public GameObject WinScene;


    void Awake()
    {
        LV = 0;
        player = GameObject.Find("Player").GetComponent<PlayerController>();
        // satellite = GameObject.Find("Satellite").GetComponent<Satellite>();
        talkpanel.SetActive(false);
        gameoverDisplay.SetActive(false);
        if (inst = null)
        {
            inst = this;
        }
        playTimeText = playTime.GetComponent<TextMeshProUGUI>();
        HP = 2;
        playerExp.transform.localScale = new Vector3(0, 1, 1);
        oneExp = 0.1f;
        skillPanelOn.SetActive(false);
        WinScene.SetActive(false);
        instBossScripts = instBoss.GetComponent<BossController>();

        bool isplayerMissileOn = player.isMissile;
        bool isPlayerAllRoundShoot = player.isAllRoundShoot;
        bool isPlayerSateOn = player.isSatelliteOn;
        bool isPlayerSateMissileOn = satellite.isSatellireMissile;
        bool isPlayerSateAoe = satellite.satelliteAoePlus;

    }
    private void Update()
    {
        if (isBattle)
        {
            realplaytime += Time.deltaTime;
        }

        if (realplaytime > 420 && !isBossfirstCome)
        {
            ComeBoss();
            isBossfirstCome = true;
        }

        //  Reroll();
        SkillPanelSelete();
        GameOver();
        PanelOnOff();
        TalkSys();
        Win();
    }

    void ComeBoss()
    {
        isBossCome = true;
        instBoss.SetActive(true);
        instBoss.transform.position = new Vector3(0, 0, 0);
        player.transform.position = new Vector3(0, player.transform.position.y, -15);
    }

    void Win()
    {
        if (instBoss.activeSelf == false && instBossScripts.isBossDead && realplaytime > 420)
        {
            WinScene.SetActive(true);
        }

    }

    private void LateUpdate()
    {
        int hour = (int)(realplaytime / 3600);
        int min = (int)(realplaytime - hour * 3600) / 60;
        int second = (int)(realplaytime % 60);

        playTimeText.text = string.Format("{0:00}", hour) + ":" + string.Format("{0:00}", min) + ":" + string.Format("{0:00}", second);
    }
    public void MinusPlayerHP()
    {
        if (HP > -1)
        {
            playerHp[HP].SetActive(false);
            HP--;
        }
    }

    void PlusPlayerHp()
    {
        if (HP > -1)
        {
            if (HP != 2)
            {
                HP++;
                playerHp[HP].SetActive(true);
            }
            else
            {
                return;
            }
        }
    }


    void GameOver()
    {
        if (HP < 0)
        {
            Time.timeScale = 0;
            gameoverDisplay.SetActive(true);
        }
    }
    public void EXPBarFill()
    {
        playerExp.transform.localScale = new Vector3(exp, 1, 1);

        exp += defultExp;
        if (exp > 1 && LV < 100)
        {
            LV++;
            isPanelOn = true;
            isSelet = false;
            exp = 0;

            if (LV == 1)
            {
                defultExp /= 4.0f;
            }
            else if (LV == 5)
            {
                defultExp /= 4.0f;
            }
            else if (LV == 10)
            {
                defultExp /= 4.0f;
            }

        }
    }

    void Reroll()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isPanelOn = true;
            isSelet = false;
        }
    }

    void RemoveFun(int Fuc)
    {
        for (int i = 0; i < skillCard.Count; i++)
        {
            if (Fuc == skillCard[i].SkillFuc)
            {
                skillCard.Remove(skillCard[i]);
                Debug.Log(skillCard.Count);
                Debug.Log(isAllSkill);
            }
        }
    }

    void SkillPanelSelete()
    {

        if (!isSelet)
        {

        Start:

            for (int i = 0; i < viewSkillCard.Length; i++)
            {

                viewSkillCard[i] = skillCard[Random.Range(0, skillCard.Count)];

                CheckRan = viewSkillCard[i].SkillFuc;

                if (CheckRan == 0 && playerPowerUpLv > 3)
                {
                    goto Start;
                }
                else if (CheckRan == 1 && player.isMissile)
                {
                    goto Start;
                }
                else if (CheckRan == 2 && !player.isMissile)
                {
                    goto Start;

                }
                else if (CheckRan == 2 && !player.isSatelliteOn)
                {
                    goto Start;

                }
                else if (CheckRan == 2 && player.MissileNumber > 10)
                {

                    goto Start;

                }
                else if (CheckRan == 3 && player.isAllRoundShoot)
                {
                    goto Start;

                }
                else if (CheckRan == 4 && !player.isAllRoundShoot)
                {
                    goto Start;

                }
                else if (CheckRan == 4 && player.playerShootNum > 20)
                {
                    goto Start;

                }
                else if (CheckRan == 5 && !player.isShockWave)
                {
                    goto Start;

                }
                else if (CheckRan == 6 && player.isSatelliteOn)
                {
                    goto Start;

                }
                else if (CheckRan == 7 && !player.isSatelliteOn)
                {
                    goto Start;

                }
                else if (CheckRan == 7 && satellite.satelliteAttackNumber > 10)
                {
                    goto Start;

                }
                else if (CheckRan == 8 && !player.isSatelliteOn)
                {
                    goto Start;

                }
                else if (CheckRan == 8 && satellite.isSatellireMissile)
                {
                    goto Start;

                }
                else if (CheckRan == 9 && !satellite.isSatellireMissile)
                {
                    goto Start;

                }
                else if (CheckRan == 9 && satellite.satelliteMissileNumber > 11)
                {
                    goto Start;

                }
                else if (CheckRan == 10 && !player.isSatelliteOn)
                {
                    goto Start;

                }
                else if (CheckRan == 10 && satellite.satelliteAttackDealy < 7)
                {
                    goto Start;
                }
                else if (CheckRan == 11 && !player.isSatelliteOn)
                {
                    goto Start;
                }
                else if (CheckRan == 11 && satellite.satelliteAoePlus)
                {
                    goto Start;
                }
                else if (CheckRan == 12 && HP == 2 && skillCard.Count > 4)
                {
                    goto Start;
                }
                else if (CheckRan == 12 && HP == 2 && skillCard.Count < 4)
                {
                    continue;
                }

            }


            if (skillCard.Count > 4)
            {
                if ((viewSkillCard[0].SkillFuc == viewSkillCard[1].SkillFuc))
                {
                    Debug.Log("중복된 숫자가 나왔어");
                    goto Start;

                }
                else if ((viewSkillCard[0].SkillFuc == viewSkillCard[2].SkillFuc))
                {
                    Debug.Log("중복된 숫자가 나왔어");
                    goto Start;
                }
                else if ((viewSkillCard[1].SkillFuc == viewSkillCard[2].SkillFuc))
                {
                    Debug.Log("중복된 숫자가 나왔어");
                    goto Start;
                }
            }
        }

        isSelet = true;

        for (int i = 0; i < SkillPanel.Length; i++)
        {
            {
                skillTitle[i].text = viewSkillCard[i].Skillname;
                skillMain[i].text = viewSkillCard[i].SkillMain;
                skillFunc[i].text = viewSkillCard[i].SkillFuc.ToString();
            }

        }

    }

    public void GetCardFun(int num)
    {
        getFuc = num;
    }

    public void PanelSelet()
    {
        int cardNumber = getFuc;

        if (cardNumber == 0)
        {
            if (playerPowerUpLv < 3)
            {
                if (playerPowerUpLv == 1)
                {
                    if (!player.isPowerUp)
                    {
                        player.isPowerUp = true;
                    }
                }

                player.PowerUpLv = playerPowerUpLv;

                playerPowerUpLv++;

                if (playerPowerUpLv == 3)
                {
                    RemoveFun(0);
                }

            }

            isPanelOn = false;
        }
        else if (cardNumber == 1)
        {
            if (!player.isMissile)
            {
                player.isMissile = true;
                player.isMissileDealy = true;
                isPanelOn = false;
                RemoveFun(1);
            }
        }
        else if (cardNumber == 2)
        {
            if (player.MissileNumber == 9)
            {
                player.MissileNumber += 2;
                isPanelOn = false;
                RemoveFun(2);
            }
            else if (player.MissileNumber < 9)
            {
                player.MissileNumber += 2;
                isPanelOn = false;

            }

        }
        else if (cardNumber == 3)
        {
            if (!player.isAllRoundShoot)
            {
                player.isAllRoundShoot = true;
                RemoveFun(3);
                isPanelOn = false;

            }
        }
        else if (cardNumber == 4)
        {
            if (player.playerShootNum == 20)
            {
                player.playerShootNum += 10;
                RemoveFun(4);
                isPanelOn = false;
            }
            else if (player.playerShootNum < 30)
            {
                player.playerShootNum += 10;
                isPanelOn = false;
            }
        }
        else if (cardNumber == 5)
        {
            if (!player.isShockWave)
            {
                player.isShockWave = true;
                isPanelOn = false;

            }
        }
        else if (cardNumber == 6)
        {
            if (!player.isSatelliteOn)
            {
                player.isSatelliteOn = true;
                RemoveFun(6);
                isPanelOn = false;

            }
        }
        else if (cardNumber == 7)
        {
            if (satellite.satelliteAttackNumber == 10)
            {
                satellite.satelliteAttackNumber += 5;
                RemoveFun(7);
                isPanelOn = false;
            }
            else if (satellite.satelliteAttackNumber < 10)
            {
                satellite.satelliteAttackNumber += 5;
                isPanelOn = false;
            }

        }
        else if (cardNumber == 8)
        {
            if (!satellite.isSatellireMissile)
            {
                satellite.isSatellireMissile = true;
                RemoveFun(8);
                isPanelOn = false;

            }
        }
        else if (cardNumber == 9)
        {
            if (satellite.satelliteMissileNumber == 11)
            {
                satellite.satelliteMissileNumber += 2;
                RemoveFun(9);
                isPanelOn = false;
            }
            else if (satellite.satelliteMissileNumber < 11)
            {
                satellite.satelliteMissileNumber += 2;
                isPanelOn = false;
            }

        }
        else if (cardNumber == 10)
        {
            if (satellite.satelliteAttackDealy == 7)
            {
                satellite.satelliteAttackDealy -= 3;
                RemoveFun(10);
                isPanelOn = false;
            }
            else if (satellite.satelliteAttackDealy > 7)
            {
                satellite.satelliteAttackDealy -= 3;
                isPanelOn = false;
            }

        }
        else if (cardNumber == 11)
        {
            if (!satellite.satelliteAoePlus)
            {
                satellite.satelliteAoePlus = true;
                RemoveFun(11);
                isPanelOn = false;
            }
        }
        else if (cardNumber == 12)
        {
            if (HP > 0)
            {
                PlusPlayerHp();
                isPanelOn = false;
            }
        }
    }

    void PanelOnOff()

    {
        if (!isPanelOn)
        {
            for (int i = 0; i < SkillPanel.Length; i++)
            {
                Time.timeScale = 1;
                skillPanelOn.SetActive(false);
            }

        }
        else if (isPanelOn && HP > 0)
        {
            for (int i = 0; i < SkillPanel.Length; i++)
            {
                SkillPanelSelete();
                Time.timeScale = 0;
                skillPanelOn.SetActive(true);
            }
        }
    }

    void TalkSys()
    {
        if (isBossCome)
        {
            Time.timeScale = 0;
            mainCamera.isBossTalk = true;
            mainCamera.transform.position = new Vector3(0, 5, -16.5f);
            mainCamera.transform.rotation = Quaternion.Euler(12, 0, 0);
            talkpanel.SetActive(true);
            string[] bossTalk = new string[] { "각진자여, 어찌 진리를 더렵히려 하는가.", "이 전시의 눈으로 천라의 지망 마냥 삼라를 훑을 수 있거늘", "보자하니 하늘 높은 줄 모르고 날뛰는 네 꼴을 더는 못봐주겠구나!" };
            if (index < bossTalk.Length)
            {
                MainTalk.text = bossTalk[index];
            }

            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.KeypadEnter) || (Input.GetMouseButtonDown(0)))
            {
                index++;
            }
            else if (index == bossTalk.Length)
            {
                talkpanel.SetActive(false);
                isBossCome = false;
                Time.timeScale = 1;
                mainCamera.transform.rotation = Quaternion.Euler(90, 0, 0);
                mainCamera.isBossTalk = false;
            }
        }
    }


}


