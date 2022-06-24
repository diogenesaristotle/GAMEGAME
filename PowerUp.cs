using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    //획득시 이펙트
    public GameObject geteffetobj;
    //획득시 사운드 제어를 위한 오디오 소스
    AudioSource getsoundSos;
    //획득시 사운드
    public AudioClip getsound;

    void Start()
    {
        getsoundSos = GetComponent<AudioSource>();
        getsoundSos.clip = getsound;
    }

    // Update is called once per frame
    void Update()
    {
        //기본적으로 회전
        transform.Rotate(Vector3.up * 75 * Time.deltaTime);
    }


    //획득시 함수
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            getsoundSos.Play();
            Instantiate(geteffetobj, transform.position, Quaternion.identity);
            Destroy(this.gameObject, 0.25f);
        }
    }
}
