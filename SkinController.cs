using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinController : MonoBehaviour
{

    public GameObject player;
    PlayerController instPlayer;
    public float speed;
    bool isShoot = true;
    private void Awake()
    {
        instPlayer = player.GetComponent<PlayerController>();
        transform.Translate(player.transform.position);
    }
    // Update is called once per frame
    void Update()
    {
        transform.position = player.transform.position;
        SkinRotate();
    }

    public void SkinRotate()
    {
        if (isShoot && speed < 3)
        {
            transform.Rotate(Vector3.up * speed);
        }
        if (instPlayer.isFire || instPlayer.isDodge)
        {
            speed = 30;
            // 30 에서 50 사이 정도 100으로 가면 왜인지 뒤로 돌았다가 돈다.
            player.GetComponent<PlayerController>().isFire = false;
        }
        if (speed > 2)
        {
            speed = speed - 1;
            transform.Rotate(Vector3.up, speed);
        }
    }
}


