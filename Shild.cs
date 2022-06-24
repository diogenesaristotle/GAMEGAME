using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Shild : MonoBehaviour
{
    int hp = 7;
    private Material enemyMat;
    Color enemymatColor;
    Color hitColor;
    AudioSource shildAudio;
    public AudioClip[] shildClip = new AudioClip[2];
    public GameObject Boss;

    WaitForSeconds waitCommaOneSec = new WaitForSeconds(0.1f);

    bool isHit = false;

    private void Awake()
    {
        shildAudio = GetComponent<AudioSource>();
        enemyMat = GetComponent<Renderer>().material;
        enemymatColor = enemyMat.color;
        hitColor = new Color(255, 100, 100, 50);

        if (this.transform.parent.gameObject.CompareTag("Boss"))
        {
            hp = 850;
        }

    }

    private void OnEnable()
    {
        this.gameObject.SetActive(true);
    }

    void Update()
    {

    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == ("Player") || other.gameObject.tag == ("Bullet"))
        {
            isHit = true;

            hp--;
            shildAudio.PlayOneShot(shildClip[0]);
            StartCoroutine(OnHit());

            if (hp < 1)
            {
                SoundManager.instance.EnemyDeadSoundPlay();
                brokeshield();
                Boss.SetActive(false);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == ("Player") || other.gameObject.tag == ("Bullet"))
        {
            isHit = true;

            hp--;
            shildAudio.clip = shildClip[0];
            shildAudio.Play();
            StartCoroutine(OnHit());

            if (hp < 1)
            {
                SoundManager.instance.EnemyDeadSoundPlay();
                brokeshield();

                Boss.SetActive(false);
            }
        }
    }

    void brokeshield()
    {
        this.gameObject.SetActive(false);
    }

    IEnumerator OnHit()
    {
        enemyMat.SetColor("_Color", hitColor);

        yield return waitCommaOneSec;

        enemyMat.SetColor("_Color", enemymatColor);
        isHit = false;
    }
}
