using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveGestion : MonoBehaviour
{

    public GameObject[] waves;
    private int currentWave;
    public GameObject youPassedUI;

    IEnumerator Start()
    {
        if (waves.Length == 0)
        {
            yield break;
        }

        while (true)
        {
            //Make wave
            GameObject wave = (GameObject)Instantiate(waves[currentWave], transform.position, Quaternion.identity);

            //Wave make Emitter to Child elements.
            wave.transform.parent = this.transform;

            // Wait for all Enemy child elements is deleted .
            while (wave.transform.childCount != 0)
            {
                yield return new WaitForEndOfFrame();
            }

            Destroy(wave);

            // When Wave stored have executed all, currentWave makes 0.（Initialize -> loop）.
            if (waves.Length <= ++currentWave)
            {
                youPassedUI.SetActive(true);
                yield break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
