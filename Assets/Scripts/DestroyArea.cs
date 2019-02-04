using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyArea : MonoBehaviour
{
    private void OnTriggerExit2D(Collider2D collision)
    {
       if(!collision.CompareTag("Player") || !collision.CompareTag("Boss")){
            Destroy(collision.gameObject);
        }
    }

}
