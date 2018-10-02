using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This object represents an entire site.
public class Site : MonoBehaviour
{

    // The name of the site, which is contained inside the JSON data of the site that was loaded.
    public string siteName
    {
        get
        {
            return siteData.name;
        }
    }

    // The description of this site, which is found in the JSON info associated with this site.
    public string siteDescription
    {
        get
        {
            return siteData.description;
        }
    }

    // Serializable site info, which is the loaded JSON data.
    public SerializableSite siteData;

    // List of all data sets associated with this site. Ex: 3D Models, 360 Photos, Etc.
    public List<SiteElementSet> dataSets;

    // POI associated with this site, that's on the Earth and visible to the user.
    public POI associatedPOI;

    // Initializes the site with loaded JSON data.
    public void InitializeSite(SerializableSite siteJSON)
    {
        siteData = siteJSON;

        // Initialiez all data sets associated with this site.
        InitializeSiteElements();
    }

    // Initialize all site elements (data sets) that were provided in the JSON data.
    public void InitializeSiteElements()
    {

        // Create a new list of data sets.
        dataSets = new List<SiteElementSet>();

        // If there are panos, load the panos.
        if (siteData.panos != null && siteData.panos.Length > 0)
        {
            // Create a new set object, which contains data elements.
            GameObject panoSetObj = CreateElementSetObject(PanoSet.elementString);

            // Add the "PanoSet" to this new object, to show it's a set of panoramas.
            PanoSet panoSet = panoSetObj.AddComponent<PanoSet>();

            // Initialize the pano set.
            panoSet.Initialize(siteData.panos, this);

            // Add the pano set to the list of data sets on this site.
            dataSets.Add(panoSet);
        }

        // If there are models, load the models.
        if (siteData.models != null && siteData.models.Length > 0)
        {
            // Create a new object for this data set.
            GameObject modelSetObj = CreateElementSetObject(ModelSet.elementString);

            // Add the model set script to that object.
            ModelSet modelSet = modelSetObj.AddComponent<ModelSet>();

            // Initialize the model set, which will initialize all it's models.
            modelSet.Initialize(siteData.models, this);

            // Add the models to the list of data sets here.
            dataSets.Add(modelSet);

        }
       
       /*
       if(siteData.points != null && siteData.points.Length > 0){
            // Create a new object for this data set.
            GameObject pointsSetObj = CreateElementSetObject(PointsOOCSet.elementString);

            // Add the points set script to that object.
            PointsOOCSet pointsSet = pointsSetObj.AddComponent<PointsOOCSet>();

            // Initialize the points set, which will initialize all it's points.
            pointsSet.Initialize(siteData.points, this);

            // Add the points to the list of data sets here.
            dataSets.Add(pointsSet);
       }
        */
        // TODO: More supported data types.

    }
    
    // Creates an object of with the specified name and parents it to the sites object.
    private GameObject CreateElementSetObject(string name)
    {

        // Create a new object with the given name.
        GameObject newElementSet = new GameObject(name);

        // Set the object's parent.
        newElementSet.transform.SetParent(this.transform);

        // Set the position and rotation of the object.
        newElementSet.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);

        // Return the object.
        return newElementSet;

    }
}

// Create a new serializable class, which will be used to save/load the Site JSON data.
[System.Serializable]
public class SerializableSite
{

    // Name of the site.
    public string name;

    // Description of the site.
    public string description;

    // Lat/lon of the site, for POI placement.
    public float latitude;
    public float longitude;

    // Associated scene, if there is one.
    public string scene;

    // All the associated data types that are specified in the JSON file.
    public SerializableCAVECam[] panos;
    public SerializableVideo[] videos;
    public SerializableModel[] models;
    public Serializable3DSite[] sites3D;
    public SerializableImage[] images;
}
