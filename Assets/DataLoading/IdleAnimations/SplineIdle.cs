using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SplineIdle : Idle{
    private float rotationSpeed;

    private List<Transform> points;

    public SplineIdle(JSONSpline spline){
        GameObject splineRoot = new GameObject("spline root");
        foreach(JSONSplineElement element in spline.elements){
            GameObject s1 = new GameObject();
            s1.transform.parent = splineRoot.transform;
            s1.transform.position = element.position;
            s1.transform.rotation = Quaternion.Euler(element.eulerAngles);
        }
    }
    
    public override void Tick(){
        // Actually rotate the player.
        Player.instance.transform.Rotate(Vector3.up, rotationSpeed);
    }
}

public class JSONSpline{
    public JSONSplineElement[] elements;
}

public class JSONSplineElement{
    public Vector3 position;
    public Vector3 eulerAngles;
}