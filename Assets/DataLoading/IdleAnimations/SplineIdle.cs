using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SplineIdle : Idle{
    private float step;
    private float timeElapsed;

    private Spline spline;

    public SplineIdle(JSONSplineElement[] jsonSpline){
        timeElapsed = 0;
        spline = new Spline();
        Debug.Log(jsonSpline);
        for(int i = 0; i < jsonSpline.Length; ++i){
            JSONSplineElement element = jsonSpline[i];
            spline.AddPoint(
                new Vector3(element.position.x, element.position.y, element.position.z), 
                Quaternion.Euler(element.eulerAngles.x, element.eulerAngles.y, element.eulerAngles.z),
                step * i,
                new Vector2(0,1)
            );
        }
        spline.StartInterpolation(null, true, eWrapMode.LOOP);
    }
    
    public override void Tick(){
        // Actually rotate the player.
        timeElapsed += Time.deltaTime;
        SplineNode curr = spline.GetTransformAtTime(timeElapsed);
        Player.instance.transform.position = curr.Point;
        Player.instance.transform.rotation = curr.Rot;
    }

    public override void Reset(){
        // Reset time passed to 0
        timeElapsed = 0;
    }
}

public class JSONSplineElement{
    public JSONVector3 position;
    public JSONVector3 eulerAngles;
}

public class JSONVector3{
    public float x;
    public float y;
    public float z;
}