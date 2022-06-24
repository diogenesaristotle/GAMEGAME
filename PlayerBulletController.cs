using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEditor;
using System;


public class PlayerBulletController : MonoBehaviour
{
    public float speed = 15f;
    public float hitOffset = 0f;
    public bool UseFirePointRotation;
    public Vector3 rotationOffset = new Vector3(0, 0, 0);
    public GameObject hit;
    public GameObject flash;
    private Rigidbody rb;
    public GameObject[] Detached;
    GameObject flashInstance;
    GameObject hitInstance;
    ParticleSystem flashPs;
    ParticleSystem flashPsParts;
    ParticleSystem hitPs;
    ParticleSystem hitPsParts;
    SphereCollider bulletCol;
    bool isRicochet = false;
    public LayerMask BulletLM;
    Vector3 StartPos;
    int overlap = 1;
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        bulletCol = GetComponent<SphereCollider>();
    }
    void OnEnable()
    {
        speed = 50f;
        bulletCol.enabled = true;
        rb.constraints = RigidbodyConstraints.None;
        //StartPos = transform.forward;

        StartCoroutine(DisableBullet(this.gameObject, 5));
    }

    void FixedUpdate()
    {
        if (speed != 0 && !isRicochet)
        {
            //rb.velocity = transform.forward * speed;
            transform.position += transform.forward * (speed * Time.deltaTime);
        }
    }

    void BulletRicochet()
    {
        if (isRicochet)
        {
            Collider[] targetCol = Physics.OverlapSphere(this.transform.position, 100f, BulletLM);

            float[] targetDis = new float[targetCol.Length];

            for (int i = 0; i < targetCol.Length; i++)
            {
                targetDis[i] = Vector3.Distance(this.transform.position, targetCol[i].transform.position);
            }

            Array.Sort(targetDis);

            StartPos = (targetCol[overlap].transform.position - this.transform.position).normalized;

            isRicochet = false;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Boss"))
        {
            rb.constraints = RigidbodyConstraints.FreezeAll;
            speed = 0;
            ContactPoint contact = collision.contacts[0];
            Quaternion rot = Quaternion.FromToRotation(Vector3.up, contact.normal);
            Vector3 pos = contact.point + contact.normal * hitOffset;

            hitInstance = ObjectPooler.SpawnFromPool("PlayerHit", pos, rot);

            // BulletRicochet();

            if (UseFirePointRotation)
            {
                hitInstance.transform.rotation = gameObject.transform.rotation * Quaternion.Euler(0, 180f, 0);
            }
            else if (rotationOffset != Vector3.zero)
            {
                hitInstance.transform.rotation = Quaternion.Euler(rotationOffset);
            }
            else
            {
                hitInstance.transform.LookAt(contact.point + contact.normal);
            }

            hitPs = hitInstance.GetComponent<ParticleSystem>();
            hitPsParts = hitInstance.transform.GetChild(0).GetComponent<ParticleSystem>();

            StartCoroutine(DisableBullet(hitInstance, 2));

            foreach (var detachedPrefab in Detached)
            {
                if (detachedPrefab != null)
                {
                    detachedPrefab.transform.parent = null;
                }
            }
            bulletCol.enabled = false;
            StartCoroutine(DisableBullet(gameObject, 0.01f));
        }
    }

    IEnumerator DisableBullet(GameObject bullet, float disTime)
    {
        yield return new WaitForSeconds(disTime);
        bullet.SetActive(false);
    }

    void OnDisable()
    {
        isRicochet = false;
        StopAllCoroutines();
        ObjectPooler.ReturnToPool(gameObject);
    }

}
