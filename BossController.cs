using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossController : MonoBehaviour
{

    //플레이어 공격을 위한 함수
    public GameObject player;
    //회전, 공격 연출등을 위한 자식 오브젝트
    GameObject[] Body = new GameObject[5];
    //보스 공격 범위 표시를 위한 라인 렌더러 
    LineRenderer BossLineRenderer;
    //돌진 라인 렌더러 
    LineRenderer ChargeLineRenderer;
    //보이지 않는 레이저 타겟 오브젝트
    public GameObject LaserTarget;
    //시에르핀스키 장풍 사이즈 소,중,대
    public GameObject[] triAngleHadoKen = new GameObject[3];
    //시에르핀스키 낙하 공격 사이즈 소, 중, 대
    public GameObject[] triAngleFall = new GameObject[3];
    //낙하 공격 체크용
    int hadokeFallNumber = 0;
    //돌진 거리 
    float chargeDistence = 0;
    //돌진 거리 완충 체크
    bool isChargefull = false;
    //라인 렌더러 발사 위치
    Vector3 linePos;
    //플레이어 위치 
    Vector3 playerPos;
    //보스 오디오 소스
    AudioSource bossAudioSource;
    //보스 돌진 오디오 클립
    public AudioClip[] bossChrgeClip = new AudioClip[5];
    //차지 횟수 체크 일반적으로 3회 까지 
    int chargenumber = 0;
    //보스 미사일 발사 수;
    int bossMissileNumber = 30;
    //보스 어택 딜레이
    float bossAttackDealy;
    //딜레이 체크를 위한 시간
    float bossTime;
    bool isBossAttack = false;
    public bool isBossDead = false;

    void Awake()
    {
        player = GameObject.Find("Player");
        int bossRandomDealy = Random.Range(7, 12);
        BossLineRenderer = GetComponent<LineRenderer>();
        bossAudioSource = GetComponent<AudioSource>();
        for (int i = 0; i < 5; i++)
        {
            Body[i] = this.transform.GetChild(i).gameObject;
        }

        //마지막 단에서 돌진 범위 체크용 라인렌더러 관리
        ChargeLineRenderer = Body[4].GetComponent<LineRenderer>();

        //보이지 않는 레이저 타겟 소환 , 소환 후 비활성화
        LaserTarget = Instantiate(LaserTarget, new Vector3(0, 1, 1), Quaternion.identity);
        LaserTarget.SetActive(false);
        bossAttackDealy = Random.Range(7, 12);
    }

    void Update()
    {
        if (isBossAttack)
        {
            StartCoroutine(BossCharge());
        }
        else if (!isBossAttack)
        {
            RotateAllBody(5.0f);
        }

        BossAttackPattern();
        BossStageHold();


    }

    //전체적인 보스 패턴, 조건문을 이용해서 다중 분기
    void BossAttackPattern()
    {
        bossTime += Time.deltaTime;
        if (bossTime > bossAttackDealy && !isBossAttack)
        {
            isBossAttack = true;

            int randomPattern = Random.Range(0, 6);

            if (randomPattern == 1)
            {
                StartCoroutine(BossLaserAllRoundAttack());
                bossTime = 0;
            }
            else if (randomPattern == 2)
            {
                StartCoroutine(BossTriAngleFall());
                bossTime = 0;
            }
            else if (randomPattern == 3)
            {
                StartCoroutine(BossMissileAttack());
                bossTime = 0;
            }
            else if (randomPattern == 4)
            {
                StartCoroutine(BossTriAngleHadoKen());
                bossTime = 0;
            }
        }
    }

    void BossStageHold()
    {
        Vector3 holdPos = transform.position;

        holdPos.x = Mathf.Clamp(holdPos.x, -100, 100);
        holdPos.y = Mathf.Clamp(holdPos.z, -100, 100);

        transform.position = new Vector3(holdPos.x, 0, holdPos.y);
    }

    //보스의 몸체가 뱅글뱅글 돌아간다. 대기, 회화 등의 상황에서 실행, 문제는 갑자기 중지할 때 원래 상태로 어떻게 돌리느냐다.
    //한 번 실행하면, 원위지로 돌아갈 때 까지 돌리는 건? 
    public void RotateAllBody(float AllSpeed)
    {

        Body[0].transform.Rotate(0, AllSpeed, 0);
        Body[1].transform.Rotate(0, AllSpeed + 0.1f, 0);
        Body[2].transform.Rotate(0, AllSpeed + 0.2f, 0);
        Body[3].transform.Rotate(0, AllSpeed + 0.3f, 0);
        Body[4].transform.Rotate(0, AllSpeed + 0.4f, 0);
    }
    //보스 몸체 다섯 부분 중 하나를 지정해서 돌릴 수 있다. x랑 z앵글은 돌리면 이상하게 돌아가서 아예 배제
    void RotateSomeBody(int bodynumber, float rotateSpeed)
    {
        Body[bodynumber].transform.Rotate(0, rotateSpeed, 0);
    }

    IEnumerator BossLaserAllRoundAttack()
    {
        //비활성화 했던 레이저 타겟을 소환, 회전 
        LaserTarget.SetActive(true);

        LaserTarget.transform.RotateAround(Body[4].transform.position, Vector3.down * 1.5f, 5);

        yield return new WaitForSeconds(0.2f);

        //전시안이 레이저 타겟을 바라봄 
        Body[0].transform.LookAt(LaserTarget.transform);

        //바라보는 게 어색하지 않기 위한 전시안 대가리 위치
        Vector3 HeadVec = new Vector3(Body[0].transform.position.x, 9, Body[0].transform.position.z);

        BossLineRenderer.SetPosition(0, HeadVec);
        BossLineRenderer.SetPosition(1, LaserTarget.transform.position);

        yield return new WaitForSeconds(5.0f);

        LaserTarget.SetActive(false);
        isBossAttack = false;
    }
    IEnumerator BossCharge()
    {
        //보스 돌격 

        yield return new WaitForSeconds(0.1f);

        //돌진 중이 아닐때 플레이어를 0.1초 늦게 바라봄 
        if (!isChargefull)
        {
            //나중에 빼서 함수로 구현
            Body[0].transform.LookAt(player.transform);
            Body[1].transform.LookAt(player.transform);
            Body[2].transform.LookAt(player.transform);
            Body[3].transform.LookAt(player.transform);
            Body[4].transform.LookAt(player.transform);

        }

        if (chargeDistence < 50)
        {
            //돌진 공격 범위가 천천히 증가함 
            chargeDistence += 15 * Time.deltaTime;
            StartCoroutine(BossLaserAllRoundAttack());

            ChargeLineRenderer.enabled = true;

            ChargeLineRenderer.SetPosition(0, Body[4].transform.position);
            ChargeLineRenderer.SetPosition(1, Body[4].transform.position + Body[4].transform.forward * chargeDistence);

            linePos = Body[4].transform.position;
            playerPos = Body[4].transform.position + Body[4].transform.forward * chargeDistence;

            if (!bossAudioSource.isPlaying && chargeDistence > 5)
            {
                //조준 시 사운드 재생
                bossAudioSource.PlayOneShot(bossChrgeClip[Random.Range(0, 2)], 0.5f);
            }

        }
        else if (chargeDistence > 49)
        {
            //돌진 거리가 50이 됐을 때 라인 렌더러 색 변화 
            ChargeLineRenderer.startColor = Color.red;
            ChargeLineRenderer.endColor = Color.red;
            isChargefull = true;

            ChargeLineRenderer.enabled = false;

            Vector3 bodyVec = new Vector3(Body[4].transform.position.x, 0, Body[4].transform.position.z);


            //Body[4].transform.position += this.transform.forward * 15 * Time.deltaTime;

            //밑동 부터 하나씩 발사
            //FOR 루프로 돌려도 되었던 거 아닌지? 

            Body[4].transform.position = Vector3.MoveTowards(bodyVec, playerPos, 30 * Time.deltaTime);

            yield return new WaitForSeconds(0.2f);

            while (chargenumber < 4)
            {
                bossAudioSource.PlayOneShot(bossChrgeClip[3], 0.3f);
                chargenumber++;
            }

            Body[3].transform.position = Vector3.MoveTowards(bodyVec, playerPos, 30 * Time.deltaTime);

            yield return new WaitForSeconds(0.2f);
            Body[2].transform.position = Vector3.MoveTowards(bodyVec, playerPos, 30 * Time.deltaTime);

            yield return new WaitForSeconds(0.2f);
            Body[1].transform.position = Vector3.MoveTowards(bodyVec, playerPos, 30 * Time.deltaTime);

            yield return new WaitForSeconds(0.2f);
            Body[0].transform.position = Vector3.MoveTowards(bodyVec, playerPos, 30 * Time.deltaTime);

            yield return new WaitForSeconds(1.0f);

            chargeDistence = 0;
            chargenumber = 0;
            isChargefull = false;

            isBossAttack = false;
        }

    }
    IEnumerator BossMissileAttack()
    {
        //정수리에서 미사일 발사 
        Vector3 HeadVec = new Vector3(Body[0].transform.position.x, 10, Body[0].transform.position.z);

        yield return new WaitForSeconds(1.0f);

        bossAudioSource.PlayOneShot(bossChrgeClip[5], 0.5f);

        for (var i = 0; i < bossMissileNumber; i++)
        {
            yield return new WaitForSeconds(0.1f);
            GameObject instBossMissile = ObjectPooler.SpawnFromPool("Missile", HeadVec);
            ObjectPooler.SpawnFromPool("missileShoot", HeadVec);
            instBossMissile.GetComponent<Rigidbody>().velocity = Vector3.up * 35f;
        }


    }

    IEnumerator BossTriAngleHadoKen()
    {
        if (isBossAttack)
        {
            //삼각형 장풍 소 중 대 순서로 나감 
            for (int i = 0; i < 3; i++)
            {
                Vector3 hadokenPos = new Vector3(Body[0].transform.position.x, 5, Body[0].transform.position.z);
                yield return new WaitForSeconds(1.5f);
                bossAudioSource.PlayOneShot(bossChrgeClip[1], 0.5f);
                GameObject instHadoke = Instantiate(triAngleHadoKen[i], hadokenPos, Quaternion.Euler(-90, 0, 0));
            }
        }
    }

    IEnumerator BossTriAngleFall()
    {
        //삼각형 낙하 패턴 소 중 대로 낙하함 

        Vector3 trianglePlayerPos = new Vector3(playerPos.x, 30, playerPos.z);
        //폭격 사운드 재생 
        bossAudioSource.PlayOneShot(bossChrgeClip[4], 0.5f);
        //사운드 재생, 및 플레이어가 대비하기 위한 시간 3초 
        yield return new WaitForSeconds(3.0f);

        for (int i = 0; i < 50; i++)
        {
            //50발 나감
            Vector3 FallPos = new Vector3(trianglePlayerPos.x, trianglePlayerPos.y * 1, trianglePlayerPos.z);
            GameObject instFall = Instantiate(triAngleFall[hadokeFallNumber], FallPos, Quaternion.Euler(-90, 0, 0));
            yield return new WaitForSeconds(0.2f);

            if (i == 35)
            {
                //35발 이후 부터는 대짜
                hadokeFallNumber++;
            }
            else if (i == 20)
            {
                //20발 이후 부터는 중짜 
                hadokeFallNumber++;
            }

        }

        hadokeFallNumber = 0;

    }

    private void OnDisable()
    {
        isBossDead = true;
    }


}



