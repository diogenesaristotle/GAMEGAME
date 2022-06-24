using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;
public class Satellite : MonoBehaviour
{
    GameObject[] SatelliteChild = new GameObject[5];
    bool isSatelliteFlying = true;
    float upAndDown = 0f;
    float speed = 1f;
    float length = 0.25f;
    float yPos = 0;
    public bool isSatellireMissile = false;
    public LayerMask satelliteLM;
    Vector3 closeEnemyPos;
    AudioSource satelliteAudio;
    public AudioClip[] satelliteClip = new AudioClip[5];
    public GameObject Player;
    public int satelliteAttackNumber = 5;
    public int satelliteMissileNumber = 3;
    public float satelliteAttackDealy = 10;
    public bool satelliteAoePlus = false;
    float satelliteAoe = 0.5f;
    float DealyTime;

    WaitForSeconds waitCommaThreeFive = new WaitForSeconds(0.35f);
    private void Awake()
    {
        satelliteAudio = GetComponent<AudioSource>();
        Player = GameObject.Find("Player");

        for (int i = 0; i < 5; i++)
        {
            SatelliteChild[i] = this.transform.GetChild(i).gameObject;
        }
    }

    void Update()
    {
        SatelliteFlying();
        FollowPlayer();
        SatellitePatten();
    }

    void SatellitePatten()
    {
        DealyTime += Time.deltaTime;

        if (DealyTime > satelliteAttackDealy)
        {
            StartCoroutine(SatelliteNormalAttack());
            SatelliteMissileAttack();
            DealyTime = 0;
        }
    }

    void FollowPlayer()
    {
        Vector3 playerDir = new Vector3(Player.transform.position.x, this.transform.position.y, Player.transform.position.z);

        this.transform.position = Vector3.MoveTowards(this.transform.position, playerDir, 7 * Time.deltaTime);
    }

    void SatelliteFlying()

    {
        if (isSatelliteFlying)
        {
            //내용물 회전
            //SatelliteChild[0].transform.Rotate(3.5f, 5.5f, 7.5f);

            //뚜껑&밑 바닥 진자운동을 위한 연산 
            upAndDown += Time.deltaTime * speed;

            yPos = Mathf.Sin(upAndDown) * length;
            //뚜껑 진자운동 + 회전
            SatelliteChild[1].transform.position = new Vector3(this.transform.position.x, this.transform.position.y + 0.25f + yPos, this.transform.position.z);
            SatelliteChild[1].transform.Rotate(0, 0, 1);
            //밑바닥 진자운동 + 역회전
            SatelliteChild[2].transform.position = new Vector3(this.transform.position.x, this.transform.position.y - 0.25f - yPos, this.transform.position.z);
            SatelliteChild[2].transform.Rotate(0, 0, -1);
            //윗날개 사이즈 변경 및 역회전
            while (SatelliteChild[3].transform.localScale.x < 3)
            {
                SatelliteChild[3].transform.localScale += new Vector3(upAndDown, upAndDown, 0);
            }
            SatelliteChild[3].transform.Rotate(0, 0, -1);
            //밑날개 사이즈 변경 및 회전
            while (SatelliteChild[4].transform.localScale.x < 2)
            {
                SatelliteChild[4].transform.localScale += new Vector3(upAndDown, upAndDown, 0);
            }
            SatelliteChild[4].transform.Rotate(0, 0, 1);
        }
    }

    IEnumerator SatelliteNormalAttack()
    {
        if (satelliteAoePlus)
        {
            satelliteAoe = 0.75f;
        }


        Collider[] targetCol = Physics.OverlapBox(this.transform.position, new Vector3(50, 16, 50) * satelliteAoe, Quaternion.identity, satelliteLM);

        for (int i = 0; i < satelliteAttackNumber; i++)
        {
            if (targetCol.Length > 0)
            {
                SatelliteChild[0].transform.LookAt(targetCol[0].transform.position);

                Vector3 targetPos = (this.transform.position - targetCol[0].transform.position).normalized;

                yield return waitCommaThreeFive;

                SatelliteSoundPlay(0);
                GameObject instSateBullet =
                    ObjectPooler.SpawnFromPool("PlayerBullet", SatelliteChild[0].transform.position, SatelliteChild[0].transform.rotation);
            }
        }
    }
    void SatelliteMissileAttack()
    {
        if (isSatellireMissile)
        {
            for (int i = 0; i < satelliteMissileNumber; i++)
            {
                SatelliteSoundPlay(1);
                GameObject instPlayerMissile = ObjectPooler.SpawnFromPool("PlayerMissile", this.transform.position);
                instPlayerMissile.GetComponent<Rigidbody>().velocity = Vector3.up * 15f;
            }
        }
    }

    void SatelliteSoundPlay(int a)
    {
        if (a == 0)
        {
            satelliteAudio.PlayOneShot(satelliteClip[Random.Range(0, 2)], 0.25f);
        }
        else if (a == 1)
        {
            satelliteAudio.PlayOneShot(satelliteClip[3], 0.25f);
        }
    }
}
