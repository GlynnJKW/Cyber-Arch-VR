using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
using System.IO;
using System.Drawing;
using System.Threading;
using System;

public class ImageToColorArray
{

    private Bitmap bitmap;

    public int width;
    public int height;

    private int numThreadsPerImage = 2;
    private UnityEngine.Color[] finalColorArray;

    private List<ImagePieceToColorArray> converters;

    public bool finished = false;

    public ImageToColorArray(Bitmap originalImage, int numThreadsPerImage)
    {

        bitmap = originalImage;
        this.numThreadsPerImage = numThreadsPerImage;

        width = bitmap.Width;
        height = bitmap.Height;

    }

    ~ImageToColorArray(){
        finalColorArray = null;
        converters.Clear();
        converters = null;
        bitmap.Dispose();
        bitmap = null;
        Debug.Log("img2clr being destroyed");
        System.GC.Collect();
        Debug.Log("img2clr garbage collection done");
    }

    public void Convert()
    {
        Profiler.BeginThreadProfiling("ImageToColorArray", this.GetHashCode().ToString());
        try
        {
            Debug.Log("Starting conversion of entire file on thread " + Thread.CurrentThread.ManagedThreadId);
            converters = new List<ImagePieceToColorArray>();

            int picHeight = bitmap.Height;

            for (int i = 0; i < numThreadsPerImage; i++)
            {

                int start = i * (picHeight / numThreadsPerImage);
                int end = (i + 1) * (picHeight / numThreadsPerImage);

                if (i == numThreadsPerImage - 1)
                {
                    end = picHeight;
                }

                Debug.LogFormat("Creating thread #{0} with start at {1} and end at {2}", i, start, end);

                Bitmap newBitmap = new Bitmap(bitmap);

                ImagePieceToColorArray newConverter = new ImagePieceToColorArray(newBitmap, start, end);
                converters.Add(newConverter);

                ThreadPool.QueueUserWorkItem(new WaitCallback(state => newConverter.Convert()));
            }
        }
        catch (Exception e)
        {
            Debug.LogErrorFormat("Exception on thread {0}: {1}", Thread.CurrentThread.ManagedThreadId, e);
        }
        Profiler.EndThreadProfiling();
    }

    public bool IsFinished()
    {
        if (converters == null || converters.Count != numThreadsPerImage)
        {
            return false;
        }

        for (int i = 0; i < converters.Count; i++)
        {

            ImagePieceToColorArray converter = converters[i];

            if (!converter.finished)
            {
                return false;
            }
        }

        if (!finished)
        {
            CombinePieces();
        }

        return finished;

    }

    private void CombinePieces()
    {

        List<UnityEngine.Color> colorList = new List<UnityEngine.Color>();

        for (int i = 0; i < converters.Count; i++)
        {
            UnityEngine.Color[] colorArray = converters[i].GetFinalArray();

            colorList.AddRange(colorArray);

        }

        finalColorArray = colorList.ToArray();
        finished = true;
        colorList.Clear();
    }

    public UnityEngine.Color[] GetFinalArray()
    {

        if (IsFinished())
        {
            return finalColorArray;
        }
        else
        {
            Debug.LogError("Trying to get final color array before conversion is finished!");
            return null;
        }

    }
}
