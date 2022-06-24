using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    public AudioClip[] enemyDeadSoundClip = new AudioClip[5];
    public AudioClip[] missileBlastClip = new AudioClip[4];
    AudioSource[] audioSource = new AudioSource[7];
    private void Awake()
    {
        instance = this;
        audioSource[0] = instance.GetComponent<AudioSource>();
        audioSource[1] = instance.GetComponent<AudioSource>();
    }

    public void EnemyDeadSoundPlay()
    {
        int a = Random.Range(0, enemyDeadSoundClip.Length);

        audioSource[0].clip = enemyDeadSoundClip[a];
        audioSource[0].volume = 0.35f;
        audioSource[0].Play();
    }

}