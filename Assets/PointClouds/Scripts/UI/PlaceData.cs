using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlaceData {

    public string name;
    public Vector3 pos;
    public Quaternion rot;

    public PlaceData(string n, Vector3 p,Vector3 r) {
        name = n;
        pos = p;
        rot = Quaternion.Euler(r);
    }
}
