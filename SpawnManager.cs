using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Random = UnityEngine.Random;


public class SpawnManager : MonoBehaviour
{
    //스폰이 될 적
    public GameObject Enemy;
    //스폰 중심이 되는 플레이어
    public GameObject Player;
    //플레이어 위치
    Vector3 playerPos;
    ObjectPooler objectPooler;
    GameObject[] spawnPoint = new GameObject[9];
    public LayerMask playerLM;
    float timer = 0;
    bool stopSpawn = false; Vector3 randomPos;
    float phaseOneTimer;
    float phaseTwoTimer;
    float phaseThreeTimer;
    float phaseOneDealy = 2.0f;

    private void Awake()
    {
        for (int i = 0; i < spawnPoint.Length; i++)
        {
            spawnPoint[i] = transform.GetChild(i).gameObject;
        }
    }

    private void Start()
    {

    }

    void Update()
    {
        timer += Time.deltaTime;
        phaseOneTimer += Time.deltaTime;
        phaseTwoTimer += Time.deltaTime;
        phaseThreeTimer += Time.deltaTime;

        playerPos = Player.transform.position;
        //  RandomSpawn();

        if (timer > 3 && !stopSpawn && phaseOneTimer > 1.5f)
        {
            phaseOneTimer = 0;
            StartCoroutine(SpawnEnemy("Enemy", 1.0f, SafeSpawnPoint()));
            StartCoroutine(SpawnEnemy("Enemy2", 1.5f, SafeSpawnPoint()));
            StartCoroutine(SpawnEnemy("Enemy", 1.0f, SafeSpawnPoint()));
            StartCoroutine(SpawnEnemy("Enemy2", 1.5f, SafeSpawnPoint()));

        }
        else if (timer > 180 && !stopSpawn && phaseTwoTimer > 5)
        {
            phaseTwoTimer = 0;
            StartCoroutine(SpawnEnemy("Enemy3", 1.0f, SafeSpawnPoint()));
            StartCoroutine(SpawnEnemy("Enemy3", 1.0f, SafeSpawnPoint()));

        }
        else if (timer > 300 && !stopSpawn && phaseThreeTimer > 10)
        {
            phaseThreeTimer = 0;
            StartCoroutine(SpawnEnemy("Enemy4", 1f, SafeSpawnPoint()));
            StartCoroutine(SpawnEnemy("Enemy5", 1f, SafeSpawnPoint()));
        }
        else if (timer > 420 && !stopSpawn)
        {
            stopSpawn = true;
        }
    }


    Vector3 SafeSpawnPoint()
    {
        float[] dis = new float[spawnPoint.Length];

        for (int i = 0; i < spawnPoint.Length; i++)
        {
            dis[i] = Vector3.Distance(Player.transform.position, spawnPoint[i].transform.position);
        }

        float maxdis = dis.Max();
        int maxdisindex = Array.IndexOf(dis, maxdis);

        return spawnPoint[maxdisindex].transform.position;
    }

    IEnumerator SpawnEnemy(string enemyType, float spawnDealy, Vector3 spawnPos)
    {
        yield return new WaitForSeconds(spawnDealy);
        ObjectPooler.SpawnFromPool<EnemyController>(enemyType, spawnPos, Quaternion.identity);
    }

}
