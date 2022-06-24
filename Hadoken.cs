using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hadoken : MonoBehaviour
{
    public GameObject player;
    Vector3 playerPos;
    public enum hadokenType { A, B };
    public hadokenType hadoken;
    Vector3 playerFallPos;
    AudioSource HadokenAudioSource;
    public AudioClip[] HadokenAudioClip = new AudioClip[2];
    bool isFloor = true;
    WaitForSeconds waitThreeSec = new WaitForSeconds(3.0f);

    private void Awake()
    {
        if (HadokenAudioSource == null)
        {
            HadokenAudioSource = GetComponent<AudioSource>();
        }

        player = GameObject.FindWithTag("Player");
        playerPos = player.transform.position;

        if (hadoken == hadokenType.B)
        {
            playerFallPos = new Vector3(playerPos.x, 2, playerPos.z);
        }
    }
    void Start()
    {
        StartCoroutine(DisableTriAngle());
    }

    void Update()
    {
        if (hadoken == hadokenType.A)
        {
            this.transform.position = Vector3.MoveTowards(this.transform.position, playerPos, 30 * Time.deltaTime);
            this.transform.Rotate(0, 0, 500 * Time.deltaTime);
        }
        else
        {
            this.transform.position = Vector3.MoveTowards(transform.position, playerFallPos, 55 * Time.deltaTime);

            if (this.transform.position.y < 3 && isFloor)
            {
                isFloor = false;
                HadokenAudioSource.PlayOneShot(HadokenAudioClip[1], 0.3f);
                GameObject instmissilePat = ObjectPooler.SpawnFromPool("missileEXP", this.transform.position);
                instmissilePat.transform.localScale = new Vector3(4, 4, 4);
            }

        }
    }

    IEnumerator DisableTriAngle()
    {
        isFloor = true;
        yield return waitThreeSec;
        this.gameObject.SetActive(false);
    }

}
