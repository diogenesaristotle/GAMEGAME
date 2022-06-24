using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControoler : MonoBehaviour
{
    public GameObject player;
    public bool isBossTalk;
    float offsetX;
    float offsetZ;

    void Start()
    {
        isBossTalk = false;
    }
    void Update()
    {
        if (!isBossTalk)
        {
            Vector3 moveVec = new Vector3(player.transform.position.x + offsetX, 45, player.transform.position.z + offsetZ);

            transform.position = moveVec;
        }
    }
}
