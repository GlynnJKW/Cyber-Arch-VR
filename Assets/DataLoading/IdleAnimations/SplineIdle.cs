using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SplineIdle : Idle{
    private float step = 4.0f;
    private float timeElapsed;

    private Spline spline;

    public SplineIdle(JSONTransform[] jsonSpline){
        timeElapsed = 0;
        spline = new Spline();
        Debug.Log(jsonSpline);
        for(int i = 0; i < jsonSpline.Length; ++i){
            JSONTransform element = jsonSpline[i];
            spline.AddPoint(
                element.position, 
                Quaternion.Euler(element.eulerAngles),
                step * i,
                new Vector2(0,1)
            );
        }
        spline.StartInterpolation(null, true, eWrapMode.LOOP);
    }
    
    public override void Tick(){
        // Actually rotate the player.
        timeElapsed += Time.deltaTime;
        SplineNode curr = spline.GetTransformAtTime(ref timeElapsed);
        Player.instance.transform.position = curr.Point;
        Player.instance.transform.rotation = curr.Rot;
    }

    public override void Reset(){
        // Reset time passed to 0
        //timeElapsed = 0;
    }
}

[System.Serializable]
public class JSONTransform{
    public Vector3 position;
    public Vector3 eulerAngles;
}