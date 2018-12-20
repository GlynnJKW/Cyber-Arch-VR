using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelSet : SiteElementSet {

    public const string elementString = "3D Models";

    protected override SiteElement AddElementComponent(GameObject elementObject, SerializableSiteElement element)
    {
        SerializableModel modelData = element as SerializableModel;
        if(modelData.custom.modelType  == "ply" || modelData.custom.modelType == "las" || modelData.custom.modelType == "xyz"){
            Debug.Log("Creating new PointsOOC");
            PointsOOC newElement = elementObject.AddComponent<PointsOOC>();
            return newElement;
        }
        else{
            Debug.Log("Creating new Model");
            Model newElement = elementObject.AddComponent<Model>();
            return newElement;
        }
    }

    protected override string GetSetType()
    {
        return elementString;
    }
}
