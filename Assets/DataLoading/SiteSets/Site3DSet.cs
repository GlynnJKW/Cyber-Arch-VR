using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Site3DSet : SiteElementSet
{

    public const string elementString = "3D Sites";

    protected override SiteElement AddElementComponent(GameObject elementObject, SerializableSiteElement element)
    {
        Site3D newElement = elementObject.AddComponent<Site3D>();
        return newElement;
    }

    protected override string GetSetType()
    {
        return elementString;
    }
}
