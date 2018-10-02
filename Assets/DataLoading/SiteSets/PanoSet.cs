using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanoSet : SiteElementSet
{
    public const string elementString = "360 Photos";

    protected override SiteElement AddElementComponent(GameObject elementObject, SerializableSiteElement element)
    {
        Panorama newPano = elementObject.AddComponent<Panorama>();
        return newPano;
    }

    protected override string GetSetType()
    {
        return elementString;
    }
}
