using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

// Manager script that controls the initial creation of sites.
public class SiteManager : MonoBehaviour {

    // Singleton instance of this manager.
    public static SiteManager instance;

    // The path to the data folder, which contains all the site data.
    public static string pathToDataFolder
    {
        get
        {
            string dataJSONFilePath = GameManager.instance.caveSettings.pathToDataJSONFile;
            if(dataJSONFilePath.IndexOf("//") != -1){
                return "/" + Path.GetDirectoryName(dataJSONFilePath);
            }
            else{
                return Path.GetDirectoryName(dataJSONFilePath);
            }
        }
    }

    // All of the sites that were loaded from JSON.
    [HideInInspector]
    public List<Site> sites;

    // List of overrides to apply to elements 
    public SerializableElements customOverrides;

    // Prefab for a POI. Must be dragged in.
    public Object poiPrefab;

    // Materials for active and inactive POIs. Must be dragged in.
    public Material poiActiveMat;
    public Material poiInactiveMat;

    // How fast 3D models should rotate when loaded.
    public float modelRotationSpeed = 3.0f;

    // Default and transparent materials for 3D models that are loaded.
    public Material objectDefaultMat;
    public Material objectTransparentMat;

    // Active site element (i.e. a specific element of a data set, such as a single panorama from a set of panoramas)
    public static SiteElement activeSiteElement;

    // Active site element set (i.e. a specific data set, such as all panoramas from a specific site)
    public static SiteElementSet activeSiteElementSet;

    // If there's any data currently loading.
    public static bool loading = false;

    // All site elements, extracted from their individual data sets.
    public List<SiteElement> allSiteElements
    {
        get
        {
            // Create a new list to store all the elements in.
            List<SiteElement> allElements = new List<SiteElement>();

            // Iterate through every site.
            for (int i = 0; i < sites.Count; i++)
            {
                // Itereate through every data set in each site.
                for (int j = 0; j < sites[i].dataSets.Count; j++)
                {
                    // Iterate through every data element in each data set in each site.
                    for (int k = 0; k < sites[i].dataSets[j].siteElements.Count; k++)
                    {
                        // Add the data element to the list.
                        allElements.Add(sites[i].dataSets[j].siteElements[k]);
                    }
                }
            }

            // return the final list.
            return allElements;

        }
    }

    // Get all the site elements that have been loaded.
    public List<SiteElement> allLoadedSiteElements
    {
        get
        {

            // Create a new list and store all of the current site elements (see previous variable).
            List<SiteElement> allLoadedElements = new List<SiteElement>();
            List<SiteElement> allElements = allSiteElements;
            
            // Iterate through all site elements and check which ones are loaded.
            for (int i = 0; i < allElements.Count; i++)
            {
                // If this element has been loaded, add it to the list.
                if (allElements[i].loaded)
                {
                    // Add to list.
                    allLoadedElements.Add(allElements[i]);
                }
            }

            // Return the loaded elements
            return allLoadedElements;
        }
    }

    // Runs first, for Singleton use in this case.
    public void Awake()
    {
        // Set this to the Singleton if there isn't already one.
        if (instance == null)
        {
            instance = this;
        }

        // If there's already a SiteManager, destroy it.
        else
        {
            GameObject.Destroy(this.gameObject);
        }
    }

    // Runs on first frame.
    IEnumerator Start()
    {
        // If we're supposed to load all the data on start, do so.
        if (GameManager.instance.caveSettings.loadAllDataOnStart)
        {
            yield return StartCoroutine(LoadAllSites());
        }

        // Start the CheckForIdle coroutine, which will regularly check if the system should idle.
        StartCoroutine(CheckForIdle());
    }

    // Loads every single site and it's data.
    // Warning: This can take a LONG time! Best when scheduled when nobody will be using the application.
    public IEnumerator LoadAllSites()
    {
        // Set the status text so the user knows to wait.
        StatusText.SetText("Loading All Sites. Please Wait.");

        // Lock all input.
        GamepadInput.LockInput(true);

        // Check one more time to make sure we're supposed to be doing this.
        if (GameManager.instance.caveSettings.loadAllDataOnStart)
        {
            // Iterate through every single site.
            for (int i = 0; i < sites.Count; i++)
            {

                // Store the site at this array index.
                Site site = sites[i];

                // Iterate through all data sets that are part of this site.
                for (int j = 0; j < site.dataSets.Count; j++)
                {

                    // Store the data set.
                    SiteElementSet dataSet = site.dataSets[j];

                    // Iterate through all data elements that are part of this data set.
                    for (int k = 0; k < dataSet.siteElements.Count; k++)
                    {

                        // Store this data element.
                        SiteElement dataElement = dataSet.siteElements[k];

                        // Set the status text to a helpful message.
                        string statusString = string.Format("Loading element {0} of {1} from set {2} of {3} from site {4} of {5}",
                            k, dataSet.siteElements.Count, j, site.dataSets.Count, i, sites.Count);

                        StatusText.SetText(statusString);
                        Debug.Log(statusString);

                        // Wait for this data element to load before proceeding.
                        yield return dataElement.Load();

                    }
                }
            }
        }

        // Unlock input.
        GamepadInput.LockInput(false);
    }

    

    // Unloads every single site that's loaded.
    public IEnumerator UnloadAllSites()
    {

        // If all data was loaded on start.
        if (GameManager.instance.caveSettings.loadAllDataOnStart)
        {
            // Iterate through all sites.
            foreach (Site site in sites)
            {
                foreach (SiteElementSet dataSet in site.dataSets)
                {
                    foreach (SiteElement dataElement in dataSet.siteElements)
                    {

                        // Unload the element.
                        yield return dataElement.Unload();

                    }
                }
            }
        }
    }

    // Loads site data from JSON and creates POIs out of it.
    public void LoadSites(string dataPath)
    {
        
        // If the data file isn't found, create a sample site so the system still shows SOMETHING.
        if (!File.Exists(dataPath))
        {

            // Loads sample data of UCSD.
            SerializableSites sampleSites = new SerializableSites();
            sampleSites.sites = new SerializableSite[1];
            SerializableSite newSite = new SerializableSite();

            // UC San Diego Lat/Lon as sample
            newSite.latitude = 32.8801f;
            newSite.longitude = 117.2340f;
            newSite.name = "UC San Diego";
            newSite.description = "This site was generated as a sample of what the JSON file should look like, roughly";

            sampleSites.sites[0] = newSite;

            string jsonText = JsonUtility.ToJson(sampleSites);

            // Save the JSON file as a sample.
            File.WriteAllText(dataPath, jsonText);

            // Return since there isn't any more we can do.
            return;

        }

        // Read the JSON from file.
        string jsonString = File.ReadAllText(dataPath);

        // Load in the JSON object as a Serializable Sites object.
        SerializableSites siteData = JsonUtility.FromJson<SerializableSites>(jsonString);

        // Load in custom JSON overrides and save
        if(File.Exists(GameManager.instance.caveSettings.pathToCustomOverrideFile)){
            jsonString = File.ReadAllText(GameManager.instance.caveSettings.pathToCustomOverrideFile);
            customOverrides = JsonUtility.FromJson<SerializableElements>(jsonString);
            Debug.Log("loaded custom overrides");
            Debug.Log(customOverrides.elements);
        }
        else{
            Debug.LogWarning(GameManager.instance.caveSettings.pathToCustomOverrideFile + " does not exist");
        }

        // If the site data of the JSON isn't null and there are sites that are part of it.
        if (siteData.sites != null && siteData.sites.Length > 0)
        {

            // Iterate through each site from JSON.
            foreach (SerializableSite site in siteData.sites)
            {
                // Create an object for this site and set it's parent.
                GameObject newSiteObject = new GameObject(site.name);
                newSiteObject.transform.parent = this.transform;

                // Add the "Site" component to the new object.
                Site newSite = newSiteObject.AddComponent<Site>();

                // Initialize the site by passing it it's JSON data.
                newSite.InitializeSite(site);

                // Add this site to the list.
                sites.Add(newSite);

            }
        }

        // If there were no sites in the JSON, print an error and return.
        else
        {
            Debug.LogErrorFormat("Error: No sites loaded. Please check the following file: {0}", GameManager.dataJsonFile);
            return;
        }

        Debug.LogFormat("Loaded {0} sites", sites.Count);

        // Create POIs from all the loaded sites.
        StartCoroutine(CreatePOIs());
    }

    // Creates physical POIs for each site that was loaded.
    public IEnumerator CreatePOIs()
    {

        // If there's no earth, wait until there is one available (to solve execution order issues).
        while (CatalystEarth.earthTransform == null)
        {
            yield return null;
        }

        // Iterate through all the loaded sites.
        foreach (Site site in sites)
        {

            // Create the new POI object and get the POI component from it.
            POI newPOI = (GameObject.Instantiate(poiPrefab) as GameObject).GetComponent<POI>();

            // Set the associated site of this POI. It should do the rest.
            newPOI.SetAssociatedSite(site);

            // Set the POIs active and inactive materials.
            newPOI.activeMat = poiActiveMat;
            newPOI.inactiveMat = poiInactiveMat;

        }
    }

    // Check if the system should idle right now.
    public bool ShouldIdle()
    {
        // Compares the time since system last received input, and also makes sure the system isn't currently loading.
        if (GamepadInput.timeSinceLastInput >= (GameManager.instance.minutesBeforeIdle * 60.0f) && !SiteManager.loading)
        {
            // True if no input for a period of time and not loading.
            return true;
        }

        // Otherwise, shouldn't idle.
        return false;
    }

    // Check and see if the system should idle.
    public IEnumerator CheckForIdle()
    {

        bool idling = false;

        // Loop and check this every single frame.
        while (true)
        {
            // If the system should idle, start the idle process.
            if (ShouldIdle())
            {
                Debug.Log("Starting Idle!");

                // Go to the Home scene, in case we're already in a scene.
                yield return StartCoroutine(GameManager.instance.GoHome());

                // Set the rotation back to normal.
                Quaternion originalPlayerRotation = Player.instance.transform.rotation;

                // Hide all UI.
                KioskUI.instance.Show(false);

                // We're going to idle now.
                idling = true;

                // Start the idling coroutine and wait for it to finish.
                // Note that when this returns, it means we're done idling!
                yield return StartCoroutine(IdleScroll());

                // No longer idling.
                idling = false;

                // Ensure we go home once more
                yield return StartCoroutine(GameManager.instance.GoHome());

                // Set the rotation back to normal.
                Player.instance.transform.rotation = originalPlayerRotation;

                // Show all UI.
                KioskUI.instance.Show(true);
            }

            // Wait a frame before checking again.
            yield return null;

        }
    }

    // Creates a nice screensaver/scene scroller while idling.
    public IEnumerator IdleScroll()
    {

        // Get all currently loaded site elements.
        List<SiteElement> loadedElements = allLoadedSiteElements;

        // If no sites have been loaded yet, we need to load at least one.
        if (loadedElements.Count <= 0)
        {

            Debug.LogWarning("No data already loaded. Attempting to load very first data set");

            // Loading the first site element.
            yield return allSiteElements[0].Load();
            loadedElements = allLoadedSiteElements;

            // If we STILL couldn't load an element, quit the application... this means the screensaver failed, and if it failed, we risk burn-in.
            if (loadedElements.Count <= 0)
            {
                Debug.LogError("Could not go to screensaver mode because no data was found! Exiting application in 1 minute to prevent burn-in.");
                yield return new WaitForSeconds(60.0f);
                Application.Quit();
            }
        }

        // All loaded site elements.
        loadedElements = allLoadedSiteElements;

        // Variable to keep track of our element index.
        int currentElementIndex = 0;

        // While we're supposed to idle.
        while (ShouldIdle())
        {

            // Get a new element.
            SiteElement currentElement = loadedElements[currentElementIndex];

            // Activate the specified element.
            yield return currentElement.Activate();

            // Initialize an elapsed time.
            float elapsedTime = 0.0f;

            // Save the initial player rotation.
            Quaternion originalPlayerRotation = Player.instance.transform.rotation;

            // While the idle time is less than idle duration we want, and the system should still be idling.
            // If there is only one loaded element, don't try to switch between elements (prevents jumping around)
            while ((elapsedTime < GameManager.instance.secondsPerIdleScene || loadedElements.Count == 1) && ShouldIdle())
            {

                // // If this is a panorama, just rotate the camera around and look at the whole 360 photo.
                // if (currentElement is Panorama)
                // {
                //     // Speed of rotation.
                //     float camRotationSpeed = 0.05f;

                //     // Actually rotate the player.
                //     Player.instance.transform.Rotate(Vector3.up, camRotationSpeed);
                   
                // }

                // // If this element is a model.
                // else if (currentElement is Model)
                // {
                //     // TODO: Add functionality for idling with models.
                // }
                currentElement.idleAnimation.Tick();

                // Wait a frame, then increase the elapsed time and loop again.
                yield return null;
                elapsedTime += Time.deltaTime;

            }

            // Reset the player's rotation.
            Player.instance.transform.rotation = originalPlayerRotation;

            // Deactivate the last activated element.
            yield return currentElement.Deactivate();

            // Increment the current element we're loading.
            currentElementIndex++;

            // Wrap back to start if needed.
            if (currentElementIndex >= loadedElements.Count)
            {
                currentElementIndex = 0;
            }

            // Wait a frame, then loop again.
            yield return null;

        }
    }
	
    // Serializable class to load JSON data into.
    [System.Serializable]
    private class SerializableSites
    {
        // All sites
        public SerializableSite[] sites;
   
    }

}

[System.Serializable]
public class SerializableElements
{
    public SerializableSiteElement[] elements;
}


