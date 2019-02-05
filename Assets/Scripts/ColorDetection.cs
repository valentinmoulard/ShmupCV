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


public class ColorDetection : MonoBehaviour
{
    private VideoCapture _webcam;
    
    private Texture2D tex;
    private Mat imageMat = new Mat();

    public Image imageGO;

    [Header("HSV Blue")]
    public int h_high_blue = 255;
    public int h_low_blue = 0;
    public int s_high_blue = 255;
    public int s_low_blue = 0;
    public int v_high_blue = 255;
    public int v_low_blue = 0;

    [Header("HSV Yellow")]
    public int h_high_yellow = 255;
    public int h_low_yellow = 0;
    public int s_high_yellow = 255;
    public int s_low_yellow = 0;
    public int v_high_yellow = 255;
    public int v_low_yellow = 0;

    /*Jaune
     * H 40 -20
     * S 255-59
     * V 255-160
     */


    [Header("Centroids position")]
    public static Tuple<int, int> yellowTuple;
    public static Tuple<int, int> blueTuple;

    public int size_structure = 4;

    [Header("Contours Size")]
    [Range(0, 50000)]
    public float contourSizeMin;
    [Range(0, 50000)]
    public float contourSizeMax;

    void Start()
    {
        _webcam = new VideoCapture();
    }
    
    void Update()
    {
        if (_webcam.IsOpened)
        {
            //matrice de la frame
            imageMat = _webcam.QueryFrame();
            CvInvoke.Flip(imageMat, imageMat, FlipType.Horizontal);

            Mat imageYellowDetect = imageMat.Clone();
            Mat imageBlueDetect = imageMat.Clone();

            //matrice hsv
            Mat HsvMat = imageMat.Clone();
            CvInvoke.CvtColor(HsvMat, HsvMat, ColorConversion.Bgr2Hsv);
            //conversion au format image
            Image<Hsv, byte> HsvImage = HsvMat.ToImage<Hsv, byte>();
            
            Mat structuringElement = CvInvoke.GetStructuringElement(ElementShape.Ellipse, new Size(size_structure, size_structure), new Point(-1, -1));


            //Bleu ================================================================================
            Mat binaryMatBlue = HsvImage.InRange(new Hsv(h_low_blue, s_low_blue, v_low_blue), new Hsv(h_high_blue, s_high_blue, v_high_blue)).Mat;

            blueTuple = Traitement(binaryMatBlue, structuringElement, imageBlueDetect);

            //Jaune ================================================================================
            Mat binaryMatYellow = HsvImage.InRange(new Hsv(h_low_yellow, s_low_yellow, v_low_yellow), new Hsv(h_high_yellow, s_high_yellow, v_high_yellow)).Mat;

            yellowTuple = Traitement(binaryMatYellow, structuringElement, imageYellowDetect);


            //affichage
            CvInvoke.Imshow("Image Detection Jaune", imageYellowDetect);
            CvInvoke.Imshow("Image Detection Bleu", imageBlueDetect);
            DisplayImage();
            CvInvoke.WaitKey(5);
        }

    }


    private Tuple<int, int> Traitement(Mat m, Mat structure, Mat output)
    {
        //filtre median
        Mat binaryMatFiltered = new Mat();
        binaryMatFiltered = MedianFilter(m);

        //erosion dilatation
        Mat fermetureMat = Fermeture(binaryMatFiltered, structure);

        //contours
        VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
        VectorOfVectorOfPoint desiredContours = new VectorOfVectorOfPoint();
        Mat hierarchy = new Mat();
        CvInvoke.FindContours(fermetureMat, contours, hierarchy, RetrType.List, ChainApproxMethod.ChainApproxNone);

        
        desiredContours.Clear();
        for (int i = 0; i < contours.Size; i++)
        {
            if (CvInvoke.ContourArea(contours[i]) > contourSizeMin && CvInvoke.ContourArea(contours[i]) < contourSizeMax)
            {
                desiredContours.Push(contours[i]);
            }
        }

        //recherche de contour
        VectorOfPoint biggest_contour = new VectorOfPoint();
        int biggest_contour_index;
        double biggest_contour_area = 0;

        //plus gros contour
        for (int i = 0; i < desiredContours.Size; i++)
        {
            if (CvInvoke.ContourArea(desiredContours[i]) > biggest_contour_area)
            {
                biggest_contour = desiredContours[i];
                biggest_contour_index = i;
                biggest_contour_area = CvInvoke.ContourArea(desiredContours[i]);
            }
        }

        //centroid
        var moments = CvInvoke.Moments(biggest_contour);
        int cx = (int)(moments.M10 / moments.M00);
        int cy = (int)(moments.M01 / moments.M00);
        Point centroid = new Point(cx, cy);
        CvInvoke.DrawContours(output, desiredContours, -1, new MCvScalar(150), 3);
        CvInvoke.Circle(output, centroid, 5, new MCvScalar(150), 3);

        return Tuple.Create(cx, cy);
    }





    private Mat GaussianFilter(Mat m)
    {
        Mat u = new Mat();
        CvInvoke.GaussianBlur(m, u, new Size(3, 3), 2.0f);
        return u;
    }

    private Mat MedianFilter(Mat m)
    {
        Mat u = new Mat();
        CvInvoke.MedianBlur(m, u, 3);
        return u;
    }

    private Mat MeanFilter(Mat m)
    {
        Mat u = new Mat();
        CvInvoke.Blur(m, u, new Size(3, 3), new Point(-1, -1));
        return u;
    }



    private Mat Dilate(Mat m, Mat structure)
    {
        CvInvoke.Dilate(m, m, structure, new Point(-1, -1), 1, BorderType.Constant, new MCvScalar(0));
        return m;
    }

    private Mat Erode(Mat m, Mat structure)
    {
        CvInvoke.Erode(m, m, structure, new Point(-1, -1), 1, BorderType.Constant, new MCvScalar(0));
        return m;
    }

    private Mat Ouverture(Mat m, Mat structure)
    {
        Mat result = new Mat();
        result = Erode(m, structure);
        result = Dilate(result, structure);
        return result;
    }

    private Mat Fermeture(Mat m, Mat structure)
    {
        Mat result = new Mat();
        result = Dilate(m, structure);
        result = Erode(result, structure);
        return result;
    }



    private void DisplayImage()
    {
        if (!imageMat.IsEmpty)
        {
            CvInvoke.Flip(imageMat, imageMat, FlipType.Horizontal);
            Destroy(tex);
            tex = convertMatToTexture2D(imageMat.Clone(), (int)imageGO.rectTransform.rect.width, (int)imageGO.rectTransform.rect.height);
            imageGO.sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
        }
    }

    Texture2D convertMatToTexture2D(Mat matImage, int width, int height)
    {
        if (matImage.IsEmpty)
        {
            return new Texture2D(width, height);
        }
        //on redimentionne
        CvInvoke.Resize(matImage, matImage, new Size(width, height));
        if (matImage.IsEmpty)
        {
            return new Texture2D(width, height);
        }
        //flip
        CvInvoke.Flip(matImage, matImage, FlipType.Vertical);
        CvInvoke.Flip(matImage, matImage, FlipType.Horizontal);

        if (matImage.IsEmpty)
        {
            return new Texture2D(width, height);
        }

        Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        texture.LoadRawTextureData(matImage.ToImage<Rgba, byte>().Bytes);
        texture.Apply();

        return texture;
    }

    
}
