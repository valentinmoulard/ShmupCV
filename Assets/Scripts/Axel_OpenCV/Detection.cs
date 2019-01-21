using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Emgu.CV;
using Emgu.CV.Util;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System;
//using System.Drawing;
using System.IO;
using Emgu.CV.Cuda;
using System.Drawing;

/// <summary>
/// Note :
/// Pour la détection d'un sourire 
/// Detecter le visage, garder la zone en question dans un rectangle
/// chercher dans le bas du rectangle/visage, soit directement le sourire / soit des variations de zones
/// </summary>
/// 
public class Detection : MonoBehaviour
{

    public VideoCapture webcam;
    public EventHandler eventHandler;
    public Mat image;
    private Mat imageGray;
    public CascadeClassifier frontFaceCascadeClassifier;
    public string pathFaceCascadeClassifier;
    public Texture2D tex;
    public UnityEngine.UI.Image myImage;

    /// <summary>
    /// A table of Rectangle that is gonna contain the region of interest of the detected faces
    /// </summary>
    private Rectangle[] frontFaces;

    private int MIN_FACE_SIZE = 50;
    private int MAX_FACE_SIZE = 200;

    //Threshold values
    [Header("Threshold values")]
    public int maxValue;
    public int blockSize;
    public int diviser;

    [Header("Contours Size")]
    [Range(0, 2000)]
    public float contourSizeMin;
    [Range(0, 2000)]
    public float contourSizeMax;

    public VectorOfVectorOfPoint allContours;
    public VectorOfVectorOfPoint desiredContours;
    public VectorOfPoint biggestContour;


    // Use this for initialization
    void Start() {
        webcam = new VideoCapture(0);
        image = new Mat();

        allContours = new VectorOfVectorOfPoint();
        desiredContours = new VectorOfVectorOfPoint();
        biggestContour = new VectorOfPoint();

        pathFaceCascadeClassifier = "C:/Users/atetart/Documents/opencv-master/opencv-master/data/lbpcascades/lbpcascade_frontalface_improved.xml";
        frontFaceCascadeClassifier = new CascadeClassifier(pathFaceCascadeClassifier);
        // webcam.Start();
        webcam.ImageGrabbed += new EventHandler(HandleWebcamQueryFrame);

    }

    // Update is called once per frame
    void Update() {
        webcam.Grab();

        //Debug.Log(frontFaces.Length.ToString());

    }

    public void HandleWebcamQueryFrame(object sender, EventArgs e) {
        if (webcam.IsOpened) webcam.Retrieve(image);
        if (image.IsEmpty) return;

        imageGray = image.Clone();
        CvInvoke.CvtColor(image, imageGray, ColorConversion.Bgr2Gray);
        if (imageGray.IsEmpty) return;

        frontFaces = frontFaceCascadeClassifier.DetectMultiScale(image: imageGray, scaleFactor: 1.1, minNeighbors: 5, minSize: new Size(MIN_FACE_SIZE, MIN_FACE_SIZE), maxSize: new Size(MAX_FACE_SIZE, MAX_FACE_SIZE));
        Debug.Log(frontFaces.Length.ToString());

        for (int i = 0; i < frontFaces.Length; i++) {

            CvInvoke.Rectangle(image, frontFaces[i], new MCvScalar(0, 180, 0), 0);
            Debug.Log("i: " + i.ToString());

        }

        //Nouvelle matrice qui focus sur le premier visage
        if (frontFaces.Length > 0) image = new Mat(image, frontFaces[0]);
        DisplayFrame(image);

        //Seuillage adaptatif
        Mat hierarchy = new Mat();
        CvInvoke.AdaptiveThreshold(imageGray, imageGray, maxValue, AdaptiveThresholdType.MeanC, ThresholdType.Binary, blockSize, diviser);
        CvInvoke.FindContours(imageGray, allContours, hierarchy, RetrType.List, ChainApproxMethod.ChainApproxNone);
        
        desiredContours.Clear();
        for (int i = 0; i < allContours.Size; i++) {
            if (CvInvoke.ContourArea(allContours[i]) > contourSizeMin && CvInvoke.ContourArea(allContours[i]) < contourSizeMax) {
                desiredContours.Push(allContours[i]);
            }   
        }

        CvInvoke.DrawContours(image, desiredContours, -1, new MCvScalar(200, 100, 200), 2);

        //RotatedRect rotatedRect;
        //rotatedRect = CvInvoke.MinAreaRect(biggestContour);

        //rotatedRect.GetVertices();

        CvInvoke.Imshow("Webcam view Normal", image);
        CvInvoke.Imshow("Webcam view Gray", imageGray);

    }

    public void DisplayFrame(Mat image) {
        if (!image.IsEmpty) {
            Destroy(tex);
            tex = ConvertMatToTexture2D(image.Clone(), (int)myImage.rectTransform.rect.width, (int)myImage.rectTransform.rect.height);

            myImage.sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
        }
    }

    Texture2D ConvertMatToTexture2D(Mat matImage, int width, int height) {
        //Resize the mat
        if (matImage.IsEmpty) return new Texture2D(width, height);
        CvInvoke.Resize(matImage, matImage, new Size(width, height));

        //The LoadRawTextureData below flip the image vertically, we handle it beforehand
        if (matImage.IsEmpty) return new Texture2D(width, height);
        CvInvoke.Flip(matImage, matImage, FlipType.Vertical);
        CvInvoke.Flip(matImage, matImage, FlipType.Horizontal);

        //Load the Mat in a Texture2D
        if (matImage.IsEmpty) return new Texture2D(width, height);
        Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        texture.LoadRawTextureData(matImage.ToImage<Rgba, Byte>().Bytes);

        texture.Apply();
        return texture;

    }
    private void OnDestroy() {
        webcam.Dispose();
        webcam.Stop();
        CvInvoke.DestroyAllWindows();
    }
}
