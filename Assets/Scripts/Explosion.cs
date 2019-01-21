using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    void OnAnimationFinish()
    {
        Destroy(gameObject);
    }
}
