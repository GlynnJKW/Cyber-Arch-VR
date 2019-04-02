using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Controllers;
using ObjectCreation;
using CloudData;
using Loading;

public class PointsOOC : SiteElement {

    private DynamicLoaderController controller;
    private PointCloudSetRealTimeController set;
    private GeoTriMeshConfiguration mesh;

    protected override IEnumerator ActivateCoroutine()
    {

        //Change the max tilt angle of the camera
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        Player player = playerObj.GetComponent<Player>();
        player.ChangeTiltAngle(85.0f);

        Debug.Log("Activating points");
        //create/activate pointcloudsetrealtimecontroller
        SerializableModel pointsData = siteData as SerializableModel;

        Debug.Log("Loading points " + pointsData.name);

        Debug.Log("Initializing mesh");

        mesh = this.gameObject.AddComponent<GeoTriMeshConfiguration>();
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
        set.cacheSizeInPoints = 3000000;
        set.userCamera = CAVECameraRig.frustumCamera;

        Debug.Log("Initializing controller");

        //create dynamicloadercontroller
        controller = this.gameObject.AddComponent<DynamicLoaderController>();
        controller.cloudPath = GetCacheDirectory(pointsData.file);
        controller.setController = set;
        set.userCamera = CAVECameraRig.frustumCamera;

        yield return null;

        if(this.spoi != null){
            try {
                string cloudPath = GetCacheDirectory(pointsData.file);
                if (!cloudPath.EndsWith("\\")) {
                    cloudPath = cloudPath + "\\";
                }

                PointCloudMetaData md = CloudLoader.LoadMetaData(cloudPath, false);
                this.spoi.Delete();
                this.spoi = new ShapefilePOI(this.siteData.custom.shapefilePath, -md.boundingBox.Center().ToFloatVector());

            } catch (Exception ex) {
                Debug.LogError(ex);
            }
        }
    }

    protected override IEnumerator DeactivateCoroutine()
    {
        Debug.Log("Deactivating points");
        //delete/deactivate pointcloudsetrealtimecontroller
        
        set.Shutdown();
        Destroy(this.gameObject.GetComponent<GeoTriMeshConfiguration>());
        Destroy(this.gameObject.GetComponent<PointCloudSetRealTimeController>());
        Destroy(this.gameObject.GetComponent<DynamicLoaderController>());
        this.spoi.Delete();
        yield return null;
    }

    protected override IEnumerator LoadCoroutine()
    {
        SerializableModel pointsData = siteData as SerializableModel;
        // CustomData custom = JsonUtility.FromJson<CustomData>(pointsData.customData);


        //Choose UI type
        elementUI = new PointOOCElementUI();


        string cacheDirectory = GetCacheDirectory(pointsData.file);
        if(!Directory.Exists(cacheDirectory)){
            yield return StartCoroutine(ProcessPLY(GetAbsolutePath(pointsData.file), cacheDirectory));
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
            string parameters = " \"" + pinput + "\" -o \"" + poutput + "\" -t " + this.siteData.custom.modelType;

            StatusText.SetText("Converting " + input + "into octree format and caching");
            
            var stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
            var proc = System.Diagnostics.Process.Start(Path.GetFullPath(GameManager.pathToPotreeExecutable), parameters);
            while(!proc.HasExited){
                yield return null;
            }
            // proc.WaitForExit();
            // proc.CloseMainWindow();
            StatusText.Hide();
            stopwatch.Stop();
            Debug.Log("Done in " + stopwatch.Elapsed.Seconds + " seconds");
        }
        yield return null; 
    }
}
