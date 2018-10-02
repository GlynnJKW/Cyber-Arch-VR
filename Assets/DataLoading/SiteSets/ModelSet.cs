using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelSet : SiteElementSet {

    public const string elementString = "3D Models";

    protected override SiteElement AddElementComponent(GameObject elementObject, SerializableSiteElement element)
    {
        SerializableModel modelData = element as SerializableModel;
        //Debug.Log(modelData.customData);
        if(modelData.customData.IndexOf("ply") != -1){
            Debug.Log("Creating new PointsOOC");
            PointsOOC newElement = elementObject.AddComponent<PointsOOC>();
            return newElement;
        }
        else{
            Model newElement = elementObject.AddComponent<Model>();
            return newElement;
        }
    }

    protected override string GetSetType()
    {
        return elementString;
    }
}
