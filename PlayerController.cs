using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Haptics;

public class PlayerController : MonoBehaviour
{
    //이동을 위한 변수
    float hor;
    float var;
    //사격 위치를 정하기 위한 사격포인트 위치
    public Transform firePointPos;
    Collider playerCOl;
    //카메라 추적을 위한 카메라 변수
    public Camera characterCamera;
    //사격 확인을 위한 불 변수
    public bool isFire = false;
    //사운드 출력을 위한 오디오
    AudioSource playerAudio;
    //파워업 체크 함수
    public bool isPowerUp = false;
    //사격 효과음을 위한 오디오 클립 배열
    public AudioClip[] shootSound = new AudioClip[3];
    //피격 효과음
    public AudioClip[] hitSound = new AudioClip[2];
    //회피 사운드
    public AudioClip[] dodgeSound = new AudioClip[2];
    //총알 프리팹
    public GameObject bulletPrefab;
    //플레이어 리지드 바디
    Rigidbody playerRb;
    //플레이어 스킨, 머터리얼, 스킨 칼라, 피격시 스킨 칼라
    public GameObject playerSkin;
    Material playerSkinMat;
    Color playerSkinColor;
    Color playerHitColor = new Color(68, 142, 255, 50);
    public GameObject Trail;
    //스피드
    float speed = 0.5f;
    //회피 상태 체크를 위한 bool
    public bool isDodge = false;
    public bool isShockWave = false;
    public bool isAllRoundShoot = false;
    public int playerShootNum = 10;
    bool isLookMouse = true;
    public bool isMissile = false;
    public int MissileNumber = 1;
    public LayerMask expLM;
    public AudioClip[] EXPSound = new AudioClip[17];
    public int PowerUpLv = 1;
    public bool isSatelliteOn = false;
    bool isFirstSatelliteOn = true;
    public GameObject satellite;
    bool isPlayerFire = true;
    float gamePadhor;
    float gamePadvar;
    Vector3 PadVac;
    public GameObject padLook;
    Gamepad gamepad;
    public bool isMissileDealy = false;
    public GameManager instGameManager;

    WaitForSeconds waitCommaOneSec = new WaitForSeconds(0.1f);
    WaitForSeconds waitCommaTwoSec = new WaitForSeconds(0.2f);
    WaitForSeconds waitOneFiveSec = new WaitForSeconds(1.5f);
    WaitForSeconds waitThreeSec = new WaitForSeconds(3.0f);

    private void Awake()
    {
        gamepad = Gamepad.current;

        Instantiate(padLook, this.transform.position, Quaternion.identity);
        //게임이 작동하는 프레임 고정
        Application.targetFrameRate = 60;

        playerCOl = GetComponent<Collider>();
        playerAudio = GetComponent<AudioSource>();
        playerRb = GetComponent<Rigidbody>();
        playerSkinMat = playerSkin.GetComponent<Renderer>().material;
        playerSkinColor = playerSkinMat.color;
        isMissile = false;
    }

    private void FixedUpdate()
    {
        Move();
        Evasion();
        if (gamepad == null)
        {
            LookMouseCursor();
        }
        else if (gamepad != null)
        {
            PadLook();
        }
        StartCoroutine(Fire());
        PlayerDead();
        GetEXP();
        SatelliteOn();
        PlayerMissileAttack();
    }

    IEnumerator GamePadRumble()
    {
        if (gamepad != null)
        {
            gamepad.SetMotorSpeeds(1, 1);
            yield return waitCommaTwoSec;
            gamepad.SetMotorSpeeds(0, 0);
        }

    }

    //이동 함수
    void Move()
    {
        PlayerStageHold();
        hor = Input.GetAxis("Horizontal");
        var = Input.GetAxis("Vertical");

        //속도가 너무 빨라서 반으로 줄임
        Vector3 moveVec = new Vector3(hor, 0, var);

        transform.position += moveVec * speed;
    }

    void PlayerStageHold()
    {
        Vector3 holdPos = transform.position;

        holdPos.x = Mathf.Clamp(holdPos.x, -100, 100);
        holdPos.y = Mathf.Clamp(holdPos.z, -100, 100);

        transform.position = new Vector3(holdPos.x, 1.43f, holdPos.y);
    }

    //사격 함수 관련
    IEnumerator Fire()
    {
        //표준 사격함수
        if (Input.GetMouseButton(0) || Input.GetAxisRaw("RigthTrigger") > 0)
        {
            if (!isPowerUp && isPlayerFire && !isDodge)
            {
                isPlayerFire = false;
                StartCoroutine(GamePadRumble());
                if (isMissileDealy)
                {
                    StartCoroutine(PlayerMissileAttack());
                }

                GameObject instBullte = ObjectPooler.SpawnFromPool("PlayerBullet", firePointPos.position, firePointPos.rotation);
                GameObject instFlash = ObjectPooler.SpawnFromPool("PlayerFlash", firePointPos.position, Quaternion.identity);
                instFlash.transform.parent = this.transform;
                ShootSFX("PlayerShoot");
                yield return waitCommaOneSec;
                isPlayerFire = true;
            }
            //파워업 사격 함수
            else if (isPowerUp)
            {
                StartCoroutine(GamePadRumble());
                StartCoroutine(PlayerPowerUP(PowerUpLv));
                if (isMissileDealy)
                {
                    StartCoroutine(PlayerMissileAttack());
                }

            }
        }
    }

    IEnumerator PlayerMissileAttack()
    {
        if (isMissile)
        {
            isMissileDealy = false;

            for (int i = 0; i < MissileNumber; i++)
            {
                GameObject instPlayerMissile = ObjectPooler.SpawnFromPool("PlayerMissile", this.transform.position);
                instPlayerMissile.GetComponent<Rigidbody>().velocity = Vector3.up * 15f;
            }

            yield return waitThreeSec;
        }
        isMissileDealy = true;
    }


    IEnumerator PlayerPowerUP(int powerLv)
    {
        if (isPlayerFire && !isDodge)
        {
            isPlayerFire = false;
            isFire = false;

            int middleNumber = 0;
            int minusNumber = 0;

            GameObject instFlash1 = ObjectPooler.SpawnFromPool("PlayerFlash", firePointPos.position, Quaternion.identity);
            instFlash1.transform.parent = this.transform;
            ShootSFX("PlayerShoot");

            if (powerLv == 1 || powerLv < 1)
            {
                powerLv = 3;
            }
            else if (powerLv == 2)
            {
                powerLv = 5;
            }
            else if (powerLv == 3 || powerLv > 3)
            {
                powerLv = 7;
            }

            middleNumber = powerLv / 2;

            for (int i = 0; i < powerLv; i++)
            {
                GameObject[] powerUpbullet = new GameObject[powerLv];

                powerUpbullet[i] = ObjectPooler.SpawnFromPool("PlayerBullet", transform.position, transform.rotation);

                minusNumber = i - middleNumber;

                powerUpbullet[i].transform.Rotate(0, minusNumber * 5, 0);
            }

            yield return waitCommaTwoSec;
            isPlayerFire = true;
        }
    }
    //전방위 사격
    IEnumerator PlayerAllRoundFire()
    {
        if (isAllRoundShoot)
        {
            isLookMouse = false;

            for (int i = 0; i < playerShootNum; i++)
            {
                Vector3 dirVac = new Vector3(Mathf.Cos(Mathf.PI * 2 * i / playerShootNum), 0, Mathf.Sin(Mathf.PI * 2 * i / playerShootNum));

                transform.forward = dirVac;

                GameObject AllRoundBullet = ObjectPooler.SpawnFromPool("PlayerBullet", firePointPos.position, transform.rotation);

            }
            yield return new WaitForEndOfFrame();
            isLookMouse = true;
        }
    }
    //회피 실행 나중에 조작 함수에
    void Evasion()
    {
        if ((Input.GetMouseButtonDown(1) && !isDodge) || (Input.GetAxisRaw("LeftTrigger") > 0 && !isDodge))
        {
            StartCoroutine(PlayerDodge());
            ShockWave();
            StartCoroutine(PlayerAllRoundFire());
        }
    }
    //능력 : 회피 시 충격파 
    void ShockWave()
    {
        if (isShockWave)
        {

            GameObject instSW = ObjectPooler.SpawnFromPool("ShockWave", transform.position, Quaternion.identity);
            instSW.transform.localScale = new Vector3(7, 7, 7);

        }
    }

    //피격 함수 
    public IEnumerator PlayerOnHit(int inviTime)
    {
        for (var i = 0; i < inviTime; i++)
        {
            playerCOl.enabled = false;
            playerSkinMat.SetColor("_Color", playerHitColor);
            yield return waitCommaTwoSec;
            playerSkinMat.SetColor("_Color", playerSkinColor);
            yield return waitCommaTwoSec;
        }
        playerCOl.enabled = true;
    }
    //회피 함수
    IEnumerator PlayerDodge()
    {
        isDodge = true;
        ShootSFX("PlayerDodge");
        Trail.SetActive(true);
        StartCoroutine(PlayerOnHit(2));
        speed *= 1.5f;
        yield return waitOneFiveSec;
        speed = 0.5f;
        isDodge = false;
        Trail.SetActive(false);
    }

    //사격 효과음 재생 함수
    public void ShootSFX(string soundName)
    {
        if (soundName == "PlayerShoot")
        {
            int a = Random.Range(0, 3);
            playerAudio.volume = 0.2f;
            playerAudio.clip = shootSound[a];
            playerAudio.Play();
        }
        else if (soundName == "PlayerOnHit")
        {
            int a = Random.Range(0, 2);
            playerAudio.clip = hitSound[a];
            playerAudio.Play();
        }
        else if (soundName == "PlayerDodge")
        {
            playerAudio.clip = dodgeSound[0];
            playerAudio.Play();
            if (isShockWave)
            {
                playerAudio.PlayOneShot(dodgeSound[1]);
            }
        }
        else if (soundName == "EXPSound")
        {
            playerAudio.PlayOneShot(EXPSound[Random.Range(0, 17)], 2.0f);
        }
        else if (soundName == "MS")
        {
            playerAudio.PlayOneShot(EXPSound[17], 2.0f);
        }
    }
    //플레이어가 마우스 방향을 바라보게 하는 함수

    void LookMouseCursor()
    {
        var dir = Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position);
        var angle = Mathf.Atan2(dir.x, dir.y) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.up);
    }

    void PadLook()
    {
        Vector2 padX = gamepad.rightStick.ReadValue();

        if (padX.x != 0 || padX.y != 0)
        {
            Vector3 padVac = new Vector3(padX.x, 0, padX.y);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(padVac), Time.deltaTime * 15);
        }
    }

    void PlayerDead()
    {
        if (instGameManager.HP < 0)
        {
            ObjectPooler.SpawnFromPool("missileEXP", this.transform.position);
            Destroy(this.gameObject);
        }
    }
    void GetEXP()
    {
        Collider[] expCol = Physics.OverlapSphere(this.transform.position, 50f, expLM);
        if (expCol.Length > 0)
        {
            for (int i = 0; i < expCol.Length; i++)
            {
                expCol[i].transform.position = Vector3.MoveTowards(expCol[i].transform.position, this.transform.position, 45 * Time.deltaTime);
            }
        }
    }

    void SatelliteOn()
    {
        if (isSatelliteOn && isFirstSatelliteOn)
        {
            ShootSFX("MS");
            satellite.SetActive(true);
            isFirstSatelliteOn = false;
        }
    }

    //충돌함수
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("EXP"))
        {
            ShootSFX("EXPSound");
            other.gameObject.SetActive(false);
            instGameManager.EXPBarFill();
        }
        if (other.gameObject.CompareTag("EnemyBullet"))
        {
            ShootSFX("PlayerOnHit");
            instGameManager.MinusPlayerHP();
            StartCoroutine(PlayerOnHit(5));
        }
        else if (other.gameObject.CompareTag("Enemy") || other.gameObject.CompareTag("Boss"))
        {
            ShootSFX("PlayerOnHit");
            instGameManager.MinusPlayerHP();
            StartCoroutine(PlayerOnHit(5));
            Vector3 incomingVec = other.transform.position - this.transform.position;
            Rigidbody otherRb = other.GetComponent<Rigidbody>();
            otherRb.AddForce(incomingVec * 50f, ForceMode.Impulse);
        }

    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("PowerUp"))
        {
            isPowerUp = true;
        }
        if (other.gameObject.CompareTag("EnemyBullet"))
        {
            ShootSFX("PlayerOnHit");
            instGameManager.MinusPlayerHP();
            StartCoroutine(PlayerOnHit(5));
        }
        else if (other.gameObject.CompareTag("Enemy") || other.gameObject.CompareTag("Boss"))
        {
            ShootSFX("PlayerOnHit");
            instGameManager.MinusPlayerHP();
            StartCoroutine(PlayerOnHit(5));
            Vector3 incomingVec = other.transform.position - this.transform.position;
            Rigidbody otherRb = other.gameObject.GetComponent<Rigidbody>();
            otherRb.AddForce(incomingVec * 50f, ForceMode.Impulse);
        }
    }

}
