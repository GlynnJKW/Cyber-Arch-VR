using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VideoSet : SiteElementSet
{

    public const string elementString = "Videos";

    protected override SiteElement AddElementComponent(GameObject elementObject, SerializableSiteElement element)
    {
        Video newElement = elementObject.AddComponent<Video>();
        return newElement;
    }

    protected override string GetSetType()
    {
        return elementString;
    }
}
