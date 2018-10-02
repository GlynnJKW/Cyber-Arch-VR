using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointsOOCSet : SiteElementSet {

    public const string elementString = "Point Clouds";

    protected override SiteElement AddElementComponent(GameObject elementObject, SerializableSiteElement element)
    {
        PointsOOC newElement = elementObject.AddComponent<PointsOOC>();
        return newElement;
    }

    protected override string GetSetType()
    {
        return elementString;
    }
}
