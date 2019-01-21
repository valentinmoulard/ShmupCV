using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    Spaceship spaceship;

    IEnumerator Start()
    {
        spaceship = this.GetComponent<Spaceship>();

        spaceship.Move(transform.up * -1);

        while (true)
        {
            for(int i = 0; i < transform.childCount; i++)
            {
                Transform shotPosition = transform.GetChild(i);
                spaceship.Shot(shotPosition);
            }

            yield return new WaitForSeconds(spaceship.shotDelay);
        }
        
    }

}
