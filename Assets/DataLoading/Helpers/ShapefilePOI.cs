using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Catfood.Shapefile;

public class ShapefilePOI
{
    private Shapefile shapefile;

    public List<GameObject> objects;

    public ShapefilePOI(string filepath) : this(filepath, Vector3.zero){
    }
    public ShapefilePOI(string filepath, Vector3 transform){
        objects = new List<GameObject>();
        shapefile = new Shapefile(filepath);
        GameObject prefab = Resources.Load<GameObject>("Prefabs/pole");
        Material mat = Resources.Load<Material>("Materials/ribbon");

        foreach (Shape shape in shapefile){
            if(shape.Type == ShapeType.Polygon){
                ShapePolygon poly = shape as ShapePolygon;

                string name = shape.GetMetadata("Area");

                foreach(PointD[] part in poly.Parts){
                    GameObject obj = new GameObject(name);
                    PointOfInterest poi = obj.AddComponent<PointOfInterest>();
                    poi.boundaries = new List<Vector3>();
                    poi.poleprefab = prefab;
                    poi.ribbonMat = mat;

                    foreach(PointD p in part){
                        poi.boundaries.Add(new Vector3((float)(p.X + transform.x), transform.y, (float)(p.Y + transform.z)));
                    }

                    objects.Add(obj);
                }
            }
        }
    }

    

    public void Disable(){
        foreach(GameObject obj in objects){
            obj.SetActive(false);
        }
    }

    public void Enable(){
        foreach(GameObject obj in objects){
            obj.SetActive(true);
        }
    }

    public void Delete(){
        foreach(GameObject obj in objects){
            Object.Destroy(obj);
        }
    }
}
