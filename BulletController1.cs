using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController1 : MonoBehaviour
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


    //강체를 할당 
    //인스턴스들 초기화
    //총 세 개의 플링 오브젝트 
    //디스에이블 함수를 만들어서 불렛 컨트롤러에 붙이기

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    void OnEnable()
    {
        speed = 15f;
        rb.constraints = RigidbodyConstraints.None;
        StartCoroutine(DisableBullet(this.gameObject, 5.0f));
    }

    void FixedUpdate()
    {
        if (speed != 0)
        {
            rb.velocity = transform.forward * speed;
        }
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {

            rb.constraints = RigidbodyConstraints.FreezeAll;
            speed = 0;

            ContactPoint contact = collision.contacts[0];
            Quaternion rot = Quaternion.FromToRotation(Vector3.up, contact.normal);
            Vector3 pos = contact.point + contact.normal * hitOffset;

            hitInstance = ObjectPooler.SpawnFromPool("Enemy2Hit", pos, rot);
            hitInstance.transform.localScale = new Vector3(5, 5, 5);

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

            StartCoroutine(DisableBullet(gameObject, 0.1f));
        }
    }

    IEnumerator DisableBullet(GameObject bullet, float disTime)
    {
        yield return new WaitForSeconds(disTime);
        bullet.SetActive(false);
    }

    void OnDisable()
    {
        StopAllCoroutines();
        ObjectPooler.ReturnToPool(gameObject);
    }

}
