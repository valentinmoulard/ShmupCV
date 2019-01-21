using System;
using System.Drawing;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Emgu.CV;
using Emgu.CV.Util;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;

public class ShipMovement : MonoBehaviour
{
    ColorDetection colorDetect = new ColorDetection();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Point centre = ColorDetection.centroid;

        //resolution de la webcam : 620 x 460
        //taille de la camera : 40 x 20
        //ce sont les calcul pour cadrer les vaisseau dans la camera
        float posX = (centre.X / 620.0f) * 40.0f;
        float posY = (((centre.Y - 460) / 460.0f) * 20.0f) * -1.0f;

        if (posX < 0 || posX > 620 || posY < 0 || posY > 460)
        {
            Debug.Log(posX + " - " + posY);
        }
        
        gameObject.transform.position = new Vector3(posX, posY);
        
    }
}
