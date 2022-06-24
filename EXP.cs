using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EXP : MonoBehaviour
{
    private void OnDisable()
    {
        ObjectPooler.ReturnToPool(gameObject);
    }
}
