using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{
    Rigidbody missile_rb;
    Transform target;
    [SerializeField]
    AudioClip[] missileFireSound = new AudioClip[5];
    [SerializeField]
    AudioClip[] missileBlastSound = new AudioClip[4];
    [SerializeField]
    LayerMask missile_lm = 7;
    ParticleSystem missile_ps;
    TrailRenderer missileTrail;
    float nowSpeed = 10f;
    [SerializeField]
    float maxSpeed = 30f;
    AudioSource missile_audio;

    WaitForSeconds waitCommaOneSec = new WaitForSeconds(0.1f);
    WaitForSeconds waitCommaTwoSec = new WaitForSeconds(0.2f);
    WaitForSeconds waitFiveSec = new WaitForSeconds(5.0f);


    void Awake()
    {
        missile_rb = GetComponent<Rigidbody>();
        missile_audio = GetComponent<AudioSource>();
        missileTrail = GetComponent<TrailRenderer>();
    }
    private void OnEnable()
    {
        StartCoroutine("LaunchDealy");
        missileTrail.enabled = true;
    }

    void Update()
    {
        if (target != null)
        {
            if (!missile_audio.isPlaying)
            {
                MissileSoundPlay();
            }

            if (nowSpeed <= maxSpeed)
            {
                nowSpeed += maxSpeed * Time.deltaTime;
            }

            transform.position += transform.up * nowSpeed * Time.deltaTime;

            Vector3 targetDir = (target.position - transform.position).normalized;
            transform.up = Vector3.Lerp(transform.up, targetDir, 0.25f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.transform.CompareTag("Player"))
        {
            StartCoroutine(MissileBlast());
            GameObject instmissilePat = ObjectPooler.SpawnFromPool("missileEXP", this.transform.position);
        }
        else if (other.transform.CompareTag("Plane"))
        {
            StartCoroutine(MissileBlast());
            GameObject instmissilePat = ObjectPooler.SpawnFromPool("missileEXP", this.transform.position);
        }
        else if (other.transform.CompareTag("Bullet"))
        {
            StartCoroutine(MissileBlast());
            this.gameObject.SetActive(false);
        }
    }
    private void OnCollisionEnter(Collision other)
    {
        if (other.transform.CompareTag("Player"))
        {
            GameObject instmissilePat = ObjectPooler.SpawnFromPool("missileEXP", this.transform.position);
            StartCoroutine(MissileBlast());
        }
        else if (other.transform.CompareTag("Plane"))
        {
            GameObject instmissilePat = ObjectPooler.SpawnFromPool("missileEXP", this.transform.position);
            StartCoroutine(MissileBlast());

        }
        else if (other.transform.CompareTag("Bullet"))
        {
            StartCoroutine(MissileBlast());
        }
    }


    void SearchEnemy()
    {
        Collider[] targetCol = Physics.OverlapSphere(transform.position, 100f, missile_lm);

        if (targetCol.Length > 0)
        {
            target = targetCol[Random.Range(0, targetCol.Length)].transform;
        }
    }

    void MissileSoundPlay()
    {
        missile_audio.PlayOneShot(missileFireSound[Random.Range(0, 5)], 0.2f);
    }

    IEnumerator LaunchDealy()
    {
        yield return new WaitUntil(() => missile_rb.velocity.y > 0f);
        yield return waitCommaOneSec;

        SearchEnemy();

        yield return waitFiveSec;

        this.gameObject.SetActive(false);

    }

    IEnumerator MissileBlast()
    {
        missile_audio.PlayOneShot(missileBlastSound[Random.Range(0, 4)], 0.7f);
        yield return waitCommaTwoSec;
        this.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        missileTrail.enabled = false;
        ObjectPooler.ReturnToPool(gameObject);
    }
}
