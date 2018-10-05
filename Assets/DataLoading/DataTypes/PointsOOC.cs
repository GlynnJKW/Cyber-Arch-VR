using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Controllers;
using ObjectCreation;

public class PointsOOC : SiteElement {

    private DynamicLoaderController controller;
    private PointCloudSetRealTimeController set;
    private GeoQuadMeshConfiguration mesh;

    protected override IEnumerator ActivateCoroutine()
    {
        Debug.Log("Activating points");
        //create/activate pointcloudsetrealtimecontroller
        SerializableModel pointsData = siteData as SerializableModel;

        Debug.Log("Loading points " + pointsData.name);

        Debug.Log("Initializing mesh");

        mesh = this.gameObject.AddComponent<GeoQuadMeshConfiguration>();
        mesh.pointRadius = 5;
        mesh.renderCircles = true;
        mesh.screenSize = true;
        mesh.interpolation = InterpolationMode.OFF;
        mesh.reloadingPossible = true;

        Debug.Log("Initializing set");

        set = this.gameObject.AddComponent<PointCloudSetRealTimeController>();
        set.moveCenterToTransformPosition = true;
        set.pointBudget = 4294967295;
        set.minNodeSize = 10;
        set.nodesLoadedPerFrame = 15;
        set.nodesGOsPerFrame = 30;
        set.meshConfiguration = mesh;
        set.cacheSizeInPoints = 1000000;
        set.userCamera = CAVECameraRig.frustumCamera;

        Debug.Log("Initializing controller");

        //create dynamicloadercontroller
        controller = this.gameObject.AddComponent<DynamicLoaderController>();
        controller.cloudPath = GetCacheDirectory(pointsData.filePath);
        controller.setController = set;
        set.userCamera = CAVECameraRig.frustumCamera;

        yield return null;
    }

    protected override IEnumerator DeactivateCoroutine()
    {
        Debug.Log("Deactivating points");
        //delete/deactivate pointcloudsetrealtimecontroller
        
        set.Shutdown();
        Destroy(this.gameObject.GetComponent<GeoQuadMeshConfiguration>());
        Destroy(this.gameObject.GetComponent<PointCloudSetRealTimeController>());
        Destroy(this.gameObject.GetComponent<DynamicLoaderController>());
        yield return null;
    }

    protected override IEnumerator LoadCoroutine()
    {
        SerializableModel pointsData = siteData as SerializableModel;
        CustomData test = JsonUtility.FromJson<CustomData>(pointsData.customData);
        Debug.Log(test.splines);

        idleAnimation = new SplineIdle(test.splines);
        //idleAnimation = new SpinningIdle(0.2f);

        string cacheDirectory = GetCacheDirectory(pointsData.filePath);
        if(!Directory.Exists(cacheDirectory)){
            yield return StartCoroutine(ProcessPLY(GetAbsolutePath(pointsData.filePath), cacheDirectory));
        }
        loaded = true;
        yield return null;
    }

    protected override IEnumerator UnloadCoroutine()
    {
        yield return null;
    }

    public static string GetCacheDirectory(string filePath)
    {

        string fileName = Path.GetFileNameWithoutExtension(filePath);

        string destinationFolder = GameManager.cacheDirectory + "/" + fileName;

        return destinationFolder;

    }

    public IEnumerator ProcessPLY(string input, string output){
        string pinput = Path.GetFullPath(input);
        string poutput = Path.GetFullPath(output);
        Debug.Log("converting " + pinput + " into " + poutput);
        if(File.Exists(input)){
            string parameters = " \"" + pinput + "\" -o \"" + poutput + "\"";

            StatusText.SetText("Converting " + input + "into octree format and caching");
            
            var stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
            var proc = System.Diagnostics.Process.Start(Path.GetFullPath(GameManager.pathToPotreeExecutable), parameters);
            proc.WaitForExit();
            proc.CloseMainWindow();
            StatusText.Hide();
            stopwatch.Stop();
            Debug.Log("Done in " + stopwatch.Elapsed.Seconds + " seconds");
        }
        yield return null; 
    }
}

[System.Serializable]
public class CustomData{
    public string modelType;
    public JSONSplineElement[] splines;
}
