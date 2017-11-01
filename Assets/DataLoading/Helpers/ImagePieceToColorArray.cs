﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Drawing;
using System.Threading;
using System;

public class ImagePieceToColorArray {

    Bitmap bitmap;

    int startRow;
    int endRow;
    int picWidth;

    UnityEngine.Color[] finalColorArray;

    public bool finished = false;

    public ImagePieceToColorArray(Bitmap originalImage, int startRow, int endRow)
    {
        bitmap = originalImage;
        this.startRow = startRow;
        this.endRow = endRow;
    }

    public void Convert()
    {

        try
        {

            Debug.LogFormat("Starting conversion from row {0} to row {1} on thread {2}", startRow, endRow, Thread.CurrentThread.ManagedThreadId);

            picWidth = bitmap.Width;

            int numRows = endRow - startRow;

            Vector2 arrayPosition = Vector2.zero;

            finalColorArray = new UnityEngine.Color[numRows * picWidth];

            for (int y = startRow; y < endRow; y++)
            {
                for (int x = 0; x < picWidth; x++)
                {
                    System.Drawing.Color color = bitmap.GetPixel(x, y);

                    UnityEngine.Color newColor = new UnityEngine.Color(color.R / 255.0f, color.G / 255.0f, color.B / 255.0f);

                    int colorIndex = (picWidth * (int)arrayPosition.y) + (int)arrayPosition.x;

                    finalColorArray[colorIndex] = newColor;

                    arrayPosition.x++;

                }

                arrayPosition.x = 0;
                arrayPosition.y++;
            }

            Debug.Log("Done converting on thread " + Thread.CurrentThread.ManagedThreadId);

            finished = true;
        }
        catch (Exception e)
        {
            Debug.LogErrorFormat("Exception on thread {0}: {1}", Thread.CurrentThread.ManagedThreadId, e);
            finished = true;
        }

    }

    public UnityEngine.Color[] GetFinalArray()
    {
        return finalColorArray;
    }

}
