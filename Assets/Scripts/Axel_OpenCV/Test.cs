using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Emgu.CV;
using Emgu.CV.Util;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System;
//using System.Drawing;
using System.IO;
using Emgu.CV.Cuda;
using System.Drawing;


public class Test : MonoBehaviour
{

    public VideoCapture webcam;
    enum BlurType { Blur, Median, Gaussian };
    [Header("HSV Filter")]
    [Range(0, 180)]
    public float hMin;
    [Range(0, 180)]
    public float hMax;
    [Range(0, 255)]
    public float sMin;
    [Range(0, 255)]
    public float sMax;
    [Range(0, 255)]
    public float vMin;
    [Range(0, 255)]
    public float vMax;

    //A quelle distance on souhaite s'éloigner de l'élément central
    public int operationSize;
    public int nbOfIter;

    //Contour
    public VectorOfVectorOfPoint contours;
    public VectorOfPoint biggestContour;
    public int biggestContourIndex;
    public double biggestContourArea = 0;


    // Use this for initialization
    void Start()
    {
        Debug.Log("test");

        //Init the webcam
        //webcam = new VideoCapture("C:/Users/atetart/Videos/DK.mp4");
        webcam = new VideoCapture(0);
        contours = new VectorOfVectorOfPoint();
        biggestContour = new VectorOfPoint();

    }

    // Update is called once per frame
    void Update()
    {

        //A Mat image - basic container
        Mat image;

        //Query the frame the webcam
        image = webcam.QueryFrame();

        //Flip the image
        Mat flippedImage = image.Clone();
        CvInvoke.Flip(image, flippedImage, FlipType.Horizontal);


        Mat imgGray = image.Clone();
        Mat imgHSV = image.Clone();

        CvInvoke.CvtColor(image, imgGray, ColorConversion.Bgr2Gray);
        CvInvoke.CvtColor(image, imgHSV, ColorConversion.Bgr2Hsv);

        Mat imgBlur = image.Clone();


        switch (BlurType.Median) {
            case BlurType.Blur:
                CvInvoke.Blur(image, imgBlur, new Size(2, 2), new Point(-1, 1));
                break;

            case BlurType.Median:
                CvInvoke.MedianBlur(image, imgBlur, 3);
                break;
            case BlurType.Gaussian:
                CvInvoke.GaussianBlur(image, imgBlur, new Size(2, 2), 2);
                break;
        }

        //On va faire ressortir une seule couleur 
        Image<Hsv, byte> imageHSV = imgHSV.ToImage<Hsv, byte>();
        //(ici vert)
        //Hsv teinteBas = new Hsv(60, 100, 0);
        //Hsv teinteHaut = new Hsv(80, 255, 255);   
        //(ici jaune fluo)
        //Hsv teinteBas = new Hsv(30, 90, 90);
        //Hsv teinteHaut = new Hsv(45, 255, 255);

        Hsv teinteBas = new Hsv(hMin, sMin, vMin);
        Hsv teinteHaut = new Hsv(hMax, sMax, vMax);
        Image<Gray, byte> imgFilter = imageHSV.InRange(teinteBas, teinteHaut);

        //Contour
        Mat hierarchy = new Mat();
        contours.Clear();
        biggestContourArea = 0;
        CvInvoke.FindContours(imgFilter, contours, hierarchy, RetrType.List, ChainApproxMethod.ChainApproxNone);
        //CvInvoke.DrawContours(image, contours, -1, new MCvScalar(200, 100, 200), 10);

        for (int i = 0; i < contours.Size; i++) {
            if (CvInvoke.ContourArea(contours[i]) > biggestContourArea) {
                biggestContour = contours[i];
                biggestContourIndex = i;
                biggestContourArea = CvInvoke.ContourArea(contours[i]);
            }
        }
        CvInvoke.DrawContours(image, contours, biggestContourIndex, new MCvScalar(200, 100, 200), 3);

        Debug.Log(biggestContourArea.ToString());
        //Centroid
        var moments = CvInvoke.Moments(biggestContour);
        int cx = (int)(moments.M10 / moments.M00);
        int cy = (int)(moments.M01 / moments.M00);
        Point centroid = new Point(cx, cy);


        //Ouverture morphologique
        Mat structuringElement = CvInvoke.GetStructuringElement(ElementShape.Cross, new Size(2 * operationSize + 1, 2 * operationSize + 1), new Point(operationSize, operationSize));
        Mat imgMorpho = image.Clone();
        CvInvoke.Erode(imgFilter, imgMorpho, structuringElement, new Point(-1, -1), nbOfIter, BorderType.Constant, new MCvScalar(0));
        CvInvoke.Dilate(imgMorpho, imgMorpho, structuringElement, new Point(-1, -1), nbOfIter, BorderType.Constant, new MCvScalar(0));


        //Invoke the c++ interface function "imshow"
        //Display image in a separated window named "Webcam view"
        CvInvoke.Imshow("Webcam view classic", image);
        //CvInvoke.Imshow("Webcam view flipped", flippedImage);
        //CvInvoke.Imshow("Webcam view Gray", imgGray);
        //CvInvoke.Imshow("Webcam view HSV", imgHSV);
        //CvInvoke.Imshow("Webcam view Blur", imgBlur);
        CvInvoke.Imshow("Webcam view HSVFilter", imgFilter);
        CvInvoke.Imshow("Webcam view Morpho", imgMorpho);


        //for(int i=0; i<100; i++) {

        //    CvInvoke.Imshow("Webcam view HSV" + i.ToString(), flippedImage);

        //}

        CvInvoke.WaitKey(24);


    }

    private void OnDestroy()
    {
        webcam.Dispose();
        CvInvoke.DestroyAllWindows();
    }
}
