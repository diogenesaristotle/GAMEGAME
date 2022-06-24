using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.AI;
using System;


public class EnemyController : MonoBehaviour
{
    [SerializeField]
    //색 변화를 위한 렌더러 변수
    private Renderer sphereMes;
    MeshFilter enemyMeshFilter;
    public Mesh[] enemyMesh;
    //플레이어 추적을 위한 네비매쉬
    private NavMeshAgent enemyNav;
    //피격시 색변화를 위한 머테리얼 변수
    private Material enemyMat;
    //기본 색
    Color enemymatColor;
    //피격시 색
    Color hitColor;
    //피격시 이펙트를 표현하기 위한 오브젝트
    public GameObject hits;
    //추적대상 플레이어
    GameObject player;
    //플레이어 위치
    Transform playertr;
    //사운드 표현을 위한 오디오 소스
    AudioSource enemyAudio;
    //사망 사운드 오디오 클립
    public AudioClip[] deadSoundClip = new AudioClip[5];
    //적 공격 사운드 오디오 클립
    public AudioClip[] ShootSoundClip = new AudioClip[3];
    //피격 사운드 오디오 클립
    public AudioClip[] hitSoundClip = new AudioClip[7];
    //체력
    public float enemyHp = 3;
    //활성화 상태를 체크하기 위한 불 변수;
    bool isActive;
    //파괴를 체크하기 위한 불 변수
    bool isDead = false;
    //맹거 스펀지 타입 적의 분열 체크를 위한 boo변수
    bool isDebris1 = false;
    bool isDebris2 = false;
    //종류를 정하기 위한 에넘
    public enum Type { A, B, C, D, F };
    //구체적인 에넘 타입
    public Type enemyType;
    //적 공격이 나오는 총구
    public GameObject enemyFirePos;
    LineRenderer enemyLine;
    //적 리지드 바디
    Rigidbody enemyRb;
    GameObject laserCore;
    //레이저 발사시 나타나는 머즐 스프라이트
    GameObject laserMuzzule;
    //레이저 공격 판정 체크를 위한 콜라이더
    BoxCollider laserCol;
    //레이저 사운드 클립
    public AudioClip[] lasarSoundClip = new AudioClip[3];
    //돌진 사운드 클립
    public AudioClip[] chargeSoundClip = new AudioClip[1];
    float delay = 0;
    float randomDealy;
    float lasarDealy;
    bool isCharge = false;
    bool isAttack = false;
    bool isChargeAttck = true;
    int shildRotateSpeed = 5;
    WaitForSeconds waitCommaZeroOneSec = new WaitForSeconds(0.01f);
    WaitForSeconds waitCommaZeroTwoSec = new WaitForSeconds(0.02f);
    WaitForSeconds waitCommaOneSec = new WaitForSeconds(0.1f);
    WaitForSeconds waitOneSec = new WaitForSeconds(1.0f);
    WaitForSeconds waitTwoSec = new WaitForSeconds(2.0f);
    WaitForSeconds waitThreeSec = new WaitForSeconds(3.0f);

    void Awake()
    {

        //플레이어와, 플레이어 위치
        player = GameObject.FindWithTag("Player");
        playertr = player.transform;
        //적 오디오
        enemyAudio = GetComponent<AudioSource>();
        //색 변환을 위한 메쉬 필터 및 렌더러, 머테리얼, 색, 피격시 색
        enemyMeshFilter = GetComponent<MeshFilter>();
        sphereMes = GetComponent<MeshRenderer>();
        enemyMat = GetComponent<Renderer>().material;
        enemymatColor = enemyMat.color;
        hitColor = new Color(255, 100, 100, 50);
        //적 리지드 바디
        enemyRb = GetComponent<Rigidbody>();
        //네비매쉬
        enemyNav = GetComponent<NavMeshAgent>();

        if (enemyType == Type.C)
        {
            //돌진타입적 돌진시 궤적 표현을 위한 라인렌더러
            enemyLine = GetComponent<LineRenderer>();
            //라인 속성 초기화
            enemyLine.startWidth = 0.5f;
            enemyLine.endWidth = 5;
        }



    }
    private void OnEnable()
    {
        //초기화 함수 

        //사망 체크 변수 초기화
        isDead = false;
        //생존 체크 변수 초기화
        isActive = true;
        //레이저 딜레이 초기화
        lasarDealy = 0;
        //색 초기화
        enemyMat.SetColor("_Color", enemymatColor);
        //네브 매쉬 위치 
        enemyNav.destination = playertr.transform.position;

        if (enemyType == Type.A)
        {
            //표준 체력 
            enemyHp = 3;
            enemyNav.stoppingDistance = 20;
        }
        else if (enemyType == Type.B && !isDebris1)
        {
            enemyNav.stoppingDistance = 25;
            //첫번째 껍데기로 초기화, 사이즈 3, 체력 5
            enemyMeshFilter.sharedMesh = enemyMesh[0];
            //첫번째면 알아보기 쉽게 크기 뻥튀기
            transform.localScale = new Vector3(3.0f, 3.0f, 3.0f);
            enemyHp = 5;
        }
        else if (enemyType == Type.C)
        {
            //표준 체력 
            enemyHp = 7;
            //레이저 타입 적은 껍데기를 따로 씌워놔서 표준 껍데기는 비활성화
            sphereMes.enabled = false;
            enemyNav.stoppingDistance = 45;
        }
        else if (enemyType == Type.D)
        {
            //표준 체력 
            enemyHp = 5;
            enemyNav.stoppingDistance = 9;
        }
        else if (enemyType == Type.F)
        {
            //표준 체력 
            enemyHp = 3;
            enemyNav.stoppingDistance = 35;
        }
    }

    void Update()
    {
        enemyNav.destination = playertr.transform.position;

        if (isActive)
        {
            randomDealy = Random.Range(2, 9);
            EnemyAttackPattern();
        }
    }
    //표준 물리 체크 
    private void OnCollisionEnter(Collision other)
    {
        //플레이어나, 플레이어 총알에 맞으면 
        if (other.gameObject.tag == ("Player") || other.gameObject.tag == ("Bullet"))

        {
            //체력 감소, 체력이 0이 되면 파괴
            enemyHp--;

            // EnemyReflct(other);
            //순서대로 피격 확인 사운드, 피격시 색 변화, 피격 사운드
            EnemySoundPlay("COD");
            StartCoroutine("OnHit");
            EnemySoundPlay("HitSoundPlay");

            if (enemyHp <= 0)
            {
                //파괴함수
                Destrution();
            }

        }
    }
    //관통 총알을 위한 트리거 체크
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == ("Player") || other.gameObject.tag == ("Bullet"))

        {
            enemyHp--;

            // EnemyReflct(other);
            EnemySoundPlay("COD");
            StartCoroutine("OnHit");
            EnemySoundPlay("HitSoundPlay");
            if (enemyHp <= 0)
            {
                //파괴함수
                Destrution();
            }

        }
    }
    //적 튕겨 나가는 함수 실장 --미사용--
    void EnemyReflct(Collision other)
    {
        Vector3 incomingVec = other.transform.position - this.transform.position;
        enemyRb.AddForce(-incomingVec * 50f, ForceMode.Impulse);
    }
    //적의 전체적인 공격 패턴 관리 함수
    //딜레이 초기화가 안 붙어있는 공격들은 자체 지연이 있는 코루틴으로 작성했음
    void EnemyAttackPattern()
    {
        //상시 플레이어 주목
        //맹거 스펀지 적의 분열 함수

        DebrisAttack();

        //공격 딜레이 체크를 위한 시간
        delay += Time.deltaTime;

        if (delay > randomDealy)
        {
            if (enemyType == Type.A)
            {
                LookTarget();
                RangeAttack();
                delay = 0;
            }
            else if (enemyType == Type.B)
            {
                LookTarget();
                AllRoundShoot();
                delay = 0;
            }
            else if (enemyType == Type.C)
            {
                EnemyCRotate();
                LasarAttack();
            }
            else if (enemyType == Type.D)
            {
                LookTarget();
                ChargeAttack();
            }
            else if (enemyType == Type.F)
            {
                MissileAttack();
                delay = 0;
            }

        }
    }

    //표준 적의 공격 함수
    void RangeAttack()
    {
        //플레이어 와의 거리가 25 미만이면
        if (enemyNav.remainingDistance < 25)
        {
            //사격 사운드 재생, 총알 발사, 총알은 발사시 내장된 스크립트로 자동으로 나아감, 효과 파티클도 오브젝트 풀에서 소환돼 총알의 자식으로 붙음
            EnemySoundPlay("ShootSoundPlay");
            ObjectPooler.SpawnFromPool("EnemyBullet2", enemyFirePos.transform.position, transform.rotation);
            GameObject flashInstance = ObjectPooler.SpawnFromPool("Enemy2Flash", enemyFirePos.transform.position, Quaternion.identity);
            flashInstance.transform.parent = this.gameObject.transform;
            flashInstance.transform.forward = enemyFirePos.transform.forward;
        }
    }
    //전방위 사격  
    private void AllRoundShoot()
    {
        if (enemyNav.remainingDistance < 30)
        {
            //이 공격 패턴은 한 번도 분열하지 않았을 때만 실행됨
            if (!isDebris1)
            {
                //발사되는 총알의 총 수
                int roundNum = 10;

                for (var i = 0; i < roundNum; i++)
                {
                    EnemySoundPlay("ShootSoundPlay");
                    GameObject instBullet = ObjectPooler.SpawnFromPool("EnemyBullet2", enemyFirePos.transform.position, transform.rotation);

                    //파이와 사인을 이용한 전방위 사격
                    Vector3 dirVac = new Vector3(Mathf.Cos(Mathf.PI * 2 * i / roundNum), 0, Mathf.Sin(Mathf.PI * 2 * i / roundNum));

                    GameObject flashInstance = ObjectPooler.SpawnFromPool("Enemy2Flash", enemyFirePos.transform.position, Quaternion.identity);
                    flashInstance.transform.parent = this.gameObject.transform;
                    instBullet.transform.forward = dirVac;
                }
            }
        }
    }
    //맹거 스펀지 타입적의 분열 함수
    void DebrisAttack()
    {
        if (enemyType == Type.B)
        {
            /*사망시 분열 
            사망시를 체크, 몇 번 죽었는지를 데브리 변수로도 체크, 
            만족할 경우 풀에서 불러온 적을 인스턴스화해서 껍데기, 스케일, 체력을 바꿈
            첫번째 파편일 경우 불 변수들을 조정해서 한 번 더 죽었을 때 두번째 파편 함수로 가게끔 만듬*/
            //나중에 생각해보니 콜백함수로 간단하게 구현할 수 있었던 게 아닐까 싶음
            if (isDead && !isDebris1)
            {
                isDead = false;
                //처음 분열시 두 개로 분열
                for (int i = 0; i < 2; i++)
                {
                    //사망 위치에 다시 본인을 소환해서 껍데기, 사이즈, 체력, 네비메쉬 거리를 바꿈
                    EnemyController instDebris1 = ObjectPooler.SpawnFromPool<EnemyController>("Enemy2", this.transform.position, Quaternion.identity);
                    instDebris1.enemyMeshFilter.sharedMesh = enemyMesh[1];
                    instDebris1.transform.localScale -= new Vector3(1.0f, 1.0f, 1.0f);
                    instDebris1.enemyHp = 3;
                    instDebris1.enemyNav.stoppingDistance = 0;
                    instDebris1.isDebris1 = true;
                    instDebris1.isDead = false;
                }

            }
            //두 번째 분열시 
            else if (isDead && isDebris1 && !isDebris2)
            {
                isDead = false;
                enemyMeshFilter.sharedMesh = enemyMesh[2];
                for (int i = 0; i < 3; i++)
                {
                    EnemyController instDebris2 = ObjectPooler.SpawnFromPool<EnemyController>("Enemy2", this.transform.position, Quaternion.identity);
                    instDebris2.enemyMeshFilter.sharedMesh = enemyMesh[2];
                    instDebris2.transform.localScale -= new Vector3(2.0f, 2.0f, 2.0f);
                    instDebris2.enemyHp = 1;
                    instDebris2.enemyNav.stoppingDistance = 0;
                    instDebris2.isDebris1 = true;
                    instDebris2.isDebris2 = true;
                    instDebris2.isDead = false;
                }
            }
        }
    }
    //플레이어를 바라보는 함수
    void LookTarget()
    {
        //플레이어 주목 함수, 레이저 공격형 C는 플레이어를 즉시 추적하면 공격을 피할 수 없게 되기 때문에 다른 주목함수로 회전
        //F타입은 껍데기 회전이 안정되지 않아 넣었음 유도
        if (this.enemyType != Type.C)
        {
            transform.LookAt(playertr);
        }
        else if (enemyType != Type.F)
        {
            transform.LookAt(playertr);
        }
    }
    //적 사운드
    void EnemySoundPlay(string name)
    {
        if (name == "DeadSoundPlay")
        {
            //파괴 사운드
            SoundManager.instance.EnemyDeadSoundPlay();
        }
        else if (name == "HitSoundPlay")
        {
            //피격 사운드
            enemyAudio.volume = 0.5f;
            enemyAudio.PlayOneShot(hitSoundClip[Random.Range(0, 6)], 0.6f);

            // int a = Random.Range(0, 6);
            // enemyAudio.clip = hitSoundClip[a];
            // enemyAudio.Play();
        }
        else if (name == "ShootSoundPlay")
        {
            //사격 사운드
            int a = Random.Range(0, ShootSoundClip.Length);

            enemyAudio.clip = ShootSoundClip[a];
            enemyAudio.volume = 0.3f;
            enemyAudio.Play();
        }
        else if (name == "LaserSearch")
        {
            //레이저 탐색 사운드
            enemyAudio.PlayOneShot(lasarSoundClip[1], 0.1f);
        }
        else if (name == "LaserCharge")
        {
            //레이저 모으기 사운드
            enemyAudio.PlayOneShot(lasarSoundClip[0], 0.1f);
        }
        else if (name == "LaserShoot")
        {
            //레이저 사격 사운드
            enemyAudio.PlayOneShot(lasarSoundClip[2]);
        }
        else if (name == "Charge")
        {
            //돌진 모으기 사운드
            enemyAudio.PlayOneShot(chargeSoundClip[0], 0.1f);
        }
        else if (name == "COD")
        {
            //피격 확인음
            enemyAudio.PlayOneShot(hitSoundClip[6], 0.4f);
        }
    }
    //피격시 색 변화
    IEnumerator OnHit()
    {
        //C타입은 껍데기를 따로 입혀놔서 개별로 작성
        if (enemyType == Type.C)
        {
            sphereMes.enabled = true;

            enemyMat.SetColor("_Color", hitColor);

            yield return waitCommaOneSec;

            enemyMat.SetColor("_Color", enemymatColor);

            sphereMes.enabled = false;

        }
        //초기화 해둔 피격 색을 0.1초간 입혔다가 원래대로 돌림
        else
        {
            enemyMat.SetColor("_Color", hitColor);

            yield return waitCommaOneSec;

            enemyMat.SetColor("_Color", enemymatColor);
        }
    }
    //레이저 타입의 회전 속도 지연을 위한 함수 (대가리가 느리게 돌아감)
    void EnemyCRotate()
    {
        //레이저 타입 적의 껍데기 회전을 위한 함수
        if (enemyType == Type.C)
        {
            GameObject[] enemyrotate = new GameObject[3];
            for (int i = 0; i < enemyrotate.Length; i++)
            {
                enemyrotate[i] = this.transform.GetChild(i).gameObject;
            }
            enemyrotate[1].transform.Rotate(Vector3.right * 0.4f);
            laserCore = enemyrotate[0];
            laserMuzzule = enemyrotate[2];
            laserCol = laserMuzzule.GetComponent<BoxCollider>();
        }
    }
    //레이저 타입 레이저 공격
    void LasarAttack()
    {
        if (enemyNav.remainingDistance < 80)
        {
            if (enemyType == Type.C)
            {

                RaycastHit lasarHit;

                Vector3 dir = player.transform.position - this.transform.position;

                //보간을 이용하여 느리게 회전
                this.transform.rotation = Quaternion.Lerp(this.transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 1.0f);

                transform.forward = laserCore.transform.forward;

                //발사되는 레이저 위치 지정
                enemyLine.SetPosition(0, laserCore.transform.position);
                enemyLine.SetPosition(1, laserCore.transform.position + transform.forward * 40);

                Physics.Raycast(transform.position, laserCore.transform.forward, out lasarHit);

                if (isCharge)
                {
                    //조준시 스프라이트 회전 
                    MuzzuleRotate(0.5f);
                }
                else if (!isCharge && isAttack)
                {
                    //발사시 더 빠르게 회전
                    MuzzuleRotate(3.0f);
                }

                //레이저 발사 코루틴 
                IEnumerator LasarShoot()
                {
                    isAttack = true;

                    laserCol.enabled = false;

                    yield return waitThreeSec;

                    laserCol.enabled = true;

                    isCharge = false;

                    //반짝반짝 거리는 레이저 색 표현을 위한 코루틴
                    StartCoroutine(LaserColorChange());

                    enemyLine.startWidth = 0.5f;
                    enemyLine.endWidth = 0.5f;

                    EnemySoundPlay("LaserShoot");

                    yield return waitTwoSec;

                    laserCol.enabled = false;
                    enemyLine.enabled = false;

                    yield return waitTwoSec;

                    lasarDealy = 0;
                    enemyLine.enabled = true;
                    enemyLine.startWidth = 0.5f;
                    enemyLine.endWidth = 5.0f;
                    isAttack = false;
                }

                //레이져 색 변화
                IEnumerator LaserColorChange()
                {
                    for (int i = 0; i < 50; i++)
                    {
                        yield return waitCommaZeroOneSec;

                        enemyLine.startColor = Color.red;
                        enemyLine.endColor = Color.red;

                        yield return waitCommaZeroTwoSec;

                        enemyLine.startColor = Color.white;
                        enemyLine.endColor = Color.yellow;
                    }
                }
                //조준, 모으기 함수 
                if (lasarHit.collider != null && lasarHit.collider.CompareTag("Player") && !isAttack)
                {
                    lasarDealy += Time.deltaTime;

                    if (lasarDealy > 2)
                    {
                        //조준선이 계속 줄어들다가 
                        if (enemyLine.endWidth > 0.1f)
                        {
                            isCharge = true;
                            enemyLine.startWidth -= 0.005f;
                            enemyLine.endWidth -= 0.05f;
                            EnemySoundPlay("LaserCharge");

                        }
                        //0.1보다 작아지면 3초후 레이저 발사
                        else if (enemyLine.endWidth < 0.1f)
                        {
                            StartCoroutine(LasarShoot());
                        }
                    }
                }

                //머즐 스프라이트 회전 
                void MuzzuleRotate(float number)
                {
                    laserMuzzule.SetActive(true);

                    for (int i = 0; i < 3; i++)
                    {
                        laserMuzzule.transform.Rotate(Vector3.forward * number);
                        if (i == 3)
                        {
                            laserMuzzule.SetActive(false);
                        }
                    }
                }

            }
        }

    }
    //돌진타입 돌진 공격
    void ChargeAttack()
    {
        if (enemyType == Type.D)
        {
            TrailRenderer ChargeTrail = GetComponent<TrailRenderer>();

            //돌진시 쉴드 회전을 위한 함수 
            chargeShildRotate();

            void chargeShildRotate()
            {
                //자식 쉴드 오브젝트들을 넣어서 뱅글뱅글 돌림
                GameObject[] chargeChild = new GameObject[4];

                for (int i = 0; i < chargeChild.Length; i++)
                {
                    chargeChild[i] = this.transform.GetChild(i).gameObject;
                }

                if (isChargeAttck)
                {
                    shildRotateSpeed = 0;
                }
                else
                {
                    shildRotateSpeed = 5;
                }

                chargeChild[1].transform.Rotate(Vector3.forward * shildRotateSpeed);
                chargeChild[2].transform.Rotate(Vector3.forward * shildRotateSpeed);
                chargeChild[3].transform.Rotate(Vector3.forward * shildRotateSpeed);
                chargeChild[0].transform.Rotate(Vector3.forward * shildRotateSpeed);

            }

            IEnumerator Charge()
            {
                //차지 사운드 재생, 파티클 재생, 1초간 차지후, 파티클을 지우고 앞으로 1000의 힘을 줘서 발사 
                //차치 어택 불 함수로 공략법을 만들려고 했는데 제대로 작동을 안 하는 듯?
                isChargeAttck = false;

                EnemySoundPlay("Charge");

                GameObject chargePat = ObjectPooler.SpawnFromPool("ChargePat", this.transform.position, Quaternion.identity);
                chargePat.transform.SetParent(this.transform);

                yield return waitOneSec;

                chargePat.SetActive(false);

                ChargeTrail.enabled = true;
                enemyNav.stoppingDistance = 0;
                enemyNav.speed = 30;
                enemyRb.AddForce(transform.forward * 1000, ForceMode.Impulse);

                yield return waitThreeSec;

                ChargeTrail.enabled = false;
                enemyNav.stoppingDistance = 9;
                enemyNav.speed = 3.5f;

                yield return waitThreeSec;

                isChargeAttck = true;
            }

            if (enemyNav.remainingDistance < 10 && isChargeAttck)
            {
                StartCoroutine(Charge());
            }


        }
    }
    //미사일 타입, 미사일 공격
    void MissileAttack()
    {
        if (enemyNav.remainingDistance < 40)
        {
            //미사일 공격류 함수는 소환하기만 하면 미사일에 내장된 유도 추적 함수로 알아서 날아가서 알아서 맞음
            GameObject instMissile = ObjectPooler.SpawnFromPool("Missile", this.transform.position);
            ObjectPooler.SpawnFromPool("missileShoot", this.transform.position);
            instMissile.GetComponent<Rigidbody>().velocity = Vector3.up * 15f;
        }
    }
    //파괴함수
    void Destrution()
    {
        isDead = true;

        EnemySoundPlay("DeadSoundPlay");
        //경험치와 파괴 이펙트를 재생
        ObjectPooler.SpawnFromPool("Exp", this.transform.position, this.transform.rotation);
        ObjectPooler.SpawnFromPool("Hits", this.transform.position, this.transform.rotation);

        this.gameObject.SetActive(false);
    }
    void OnDisable()
    {
        //비활성회시 초기화
        //분열 체크 초기화 
        //돌아가는 코루틴 정지
        //오브젝트 풀 대기열에 넣기
        DebrisAttack();
        if (isDebris1 || isDebris2)
        {
            isDebris1 = false;
            isDebris2 = false;
            enemyMeshFilter.sharedMesh = enemyMesh[0];
        }
        isActive = false;
        CancelInvoke();
        StopAllCoroutines();
        ObjectPooler.ReturnToPool(gameObject);
    }

}
