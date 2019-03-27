using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    public string fp;
    public Vector3 tr;

    public Material mat;

    private ShapefilePOI spoi;
    // Start is called before the first frame update
    void Start()
    {
        spoi = new ShapefilePOI(fp, tr);
        foreach(var obj in spoi.objects){
            obj.GetComponent<PointOfInterest>().ribbonMat = mat;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnDestroy(){
        spoi.Delete();
    }
}
