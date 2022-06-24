using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileController : MonoBehaviour
{
    [SerializeField]
    GameObject missilePrefab;
    public Transform missileSpawn;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            for (int i = 0; i < 6; i++)
            {
                GameObject inst_missile = Instantiate(missilePrefab, missileSpawn.position, Quaternion.identity);
                inst_missile.GetComponent<Rigidbody>().velocity = Vector3.up * 15f;
                
            }
        }
    }
}
