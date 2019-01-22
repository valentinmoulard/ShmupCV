using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{

    public float speed = 0.1f;
    
    // Update is called once per frame
    void Update()
    {
        // Value of Y change from 0 to 1 by time. return to 0 if it becomes 1 and repeat.
        float y = Mathf.Repeat(Time.time * speed, 1);

        Vector2 offset = new Vector2(0, y);

        this.GetComponent<Renderer>().sharedMaterial.SetTextureOffset("_MainTex", offset);
    }
}
