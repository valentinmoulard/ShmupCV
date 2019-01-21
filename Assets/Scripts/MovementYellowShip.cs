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

public class MovementYellowShip : MonoBehaviour
{
    void Update()
    {
        float x = ColorDetection.yellowTuple.Item1;
        float y = ColorDetection.yellowTuple.Item2;
        
        //Debug.Log("yellow : " + x + " - " + y);
        //resolution de la webcam : 620 x 460
        //taille de la camera : 40 x 20
        //ce sont les calcul pour cadrer les vaisseau dans la camera
        float posX = (x / 620.0f) * 40.0f;
        float posY = (((y - 460) / 460.0f) * 20.0f) * -1.0f;

        gameObject.transform.position = new Vector3(posX, posY);

    }
}
