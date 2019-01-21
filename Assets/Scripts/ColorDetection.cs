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

    public int h_high = 255;
    public int h_low = 0;
    public int s_high = 255;
    public int s_low = 0;
    public int v_high = 255;
    public int v_low = 0;

    public static Point centroid;

    public int size_structure = 4;

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

            //matrice hsv
            Mat HsvMat = imageMat.Clone();
            CvInvoke.CvtColor(HsvMat, HsvMat, ColorConversion.Bgr2Hsv);
            //conversion au format image
            Image<Hsv, byte> HsvImage = HsvMat.ToImage<Hsv, byte>();


            //matrice binaire (noir et blanc)
            Mat binaryMat = HsvImage.InRange(new Hsv(h_low, s_low, v_low), new Hsv(h_high, s_high, v_high)).Mat;

            Mat binaryMatFiltered = new Mat();
            binaryMatFiltered = MedianFilter(binaryMat);


            //Partie Erosion Dilatation
            Mat structuringElement = CvInvoke.GetStructuringElement(ElementShape.Ellipse, new Size(size_structure, size_structure), new Point(-1, -1));
            Mat fermetureMat = Fermeture(binaryMatFiltered, structuringElement);


            //recherche de contour
            VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
            VectorOfPoint biggest_contour = new VectorOfPoint();
            int biggest_contour_index;
            double biggest_contour_area = 0;
            Mat hierarchy = new Mat();
            Mat img_find_contour = new Mat();
            CvInvoke.FindContours(fermetureMat, contours, hierarchy, RetrType.List, ChainApproxMethod.ChainApproxNone);

            //plus gros contour
            for (int i = 0; i < contours.Size; i++)
            {
                if (CvInvoke.ContourArea(contours[i]) > biggest_contour_area)
                {
                    biggest_contour = contours[i];
                    biggest_contour_index = i;
                    biggest_contour_area = CvInvoke.ContourArea(contours[i]);
                }
            }

            //centroid
            var moments = CvInvoke.Moments(biggest_contour);
            int cx = (int)(moments.M10 / moments.M00);
            int cy = (int)(moments.M01 / moments.M00);
            centroid = new Point(cx, cy);

            CvInvoke.DrawContours(imageMat, contours, -1, new MCvScalar(150), 3);
            CvInvoke.Circle(imageMat, centroid, 5, new MCvScalar(150), 3);

            //affichage
            CvInvoke.Imshow("Image binaire", imageMat);
            DisplayImage();
            CvInvoke.WaitKey(10);
        }

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
