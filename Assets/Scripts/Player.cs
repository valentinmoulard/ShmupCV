using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed = 5f;

    public GameObject bullet;
    public float shootFrequency;

    IEnumerator Start()
    {
        while (true)
        {
            //Bullet make at the same location and angle as the player
            Instantiate(bullet, transform.position, transform.rotation);

            yield return new WaitForSeconds(shootFrequency);
        }

    }

    // Update is called once per frame
    void Update()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        Vector2 direction = new Vector2(x, y).normalized;

        this.GetComponent<Rigidbody2D>().velocity = direction * speed;
    }
}
