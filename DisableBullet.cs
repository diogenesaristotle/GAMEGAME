using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableBullet : MonoBehaviour
{
    WaitForSeconds wait = new WaitForSeconds(1.0f);

    void OnEnable()
    {
        StartCoroutine(BulletOff());
    }
    void OnDisable()
    {
        StopAllCoroutines();
        ObjectPooler.ReturnToPool(gameObject);
    }
    IEnumerator BulletOff()
    {
        yield return wait;
        this.gameObject.SetActive(false);
    }
}
