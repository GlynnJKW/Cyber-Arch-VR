﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Picture : SiteElement
{
    protected override IEnumerator ActivateCoroutine()
    {
        throw new NotImplementedException();
    }

    protected override IEnumerator DeactivateCoroutine()
    {
        throw new NotImplementedException();
    }

    protected override IEnumerator LoadCoroutine()
    {
        throw new NotImplementedException();
    }

    protected override IEnumerator UnloadCoroutine()
    {
        throw new NotImplementedException();
    }
}

[System.Serializable]
public class SerializableImage : SerializableSiteElement
{
    public string filePath;
}
