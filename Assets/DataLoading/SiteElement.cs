using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public abstract class SiteElement : MonoBehaviour
{
    //public GameObject caveLogo;

    // Variables to keep track of the loading status.
    public bool loaded = false;
    public bool failed = false;
    public bool active = false;

    // Which site this element is part of.
    public Site parentSite;

    //What type of idle animation this element will use
    public Idle idleAnimation;

    public ElementUI elementUI;

    // The serializable data (from JSON) associated with this site element.
    protected SerializableSiteElement siteData;

    // The Coroutines that need to be implemented in children.
    protected abstract IEnumerator LoadCoroutine();
    protected abstract IEnumerator UnloadCoroutine();
    protected abstract IEnumerator ActivateCoroutine();
    protected abstract IEnumerator DeactivateCoroutine();

    // Initializes this site element by setting the parent site and site data.
    public void Initialize(SerializableSiteElement siteData, Site parentSite)
    {
        this.parentSite = parentSite;
        this.siteData = siteData;
    }

    // Activate function that returns an Acitvate coroutine.
    public Coroutine Activate()
    {

        // Hide the Catalyst logo.
        Logo.Hide();

        // Hide the Earth
        CatalystEarth.Hide();

        // Load the Data Scene
        SceneManager.LoadScene("DataScene");

        // Show the panel for CAVE Cams
        ControlPanel.SetCaveCamPanel();

        // Start the activation coroutine (load first if needed)
        Coroutine activeCoroutine = StartCoroutine(LoadThenActivate());

        // Show the element-specific UI
        if(elementUI != null){
            elementUI.Create();
        }

        // Mark the active element as this element.
        SiteManager.activeSiteElement = this;

        // Set active as true
        active = true;

        // Return the coroutine.
        return activeCoroutine;
    }

    // Deactivate this site element to clean it up.
    public Coroutine Deactivate()
    {

        // Start a coroutine to deactivate the object.
        Coroutine deactivateCoroutine = StartCoroutine(DeactivateCoroutine());

        // If this is the active site elmeent, mark the active site element as null.
        if (SiteManager.activeSiteElement == this)
        {
            SiteManager.activeSiteElement = null;
        }

        // Set active to false
        active = false;

        //Reset idle animation
        idleAnimation.Reset();

        //Hide the element-specific UI
        if(elementUI != null){
            elementUI.Cleanup();
        }

        DeactivateCustomData();

        // Return the deactivation coroutine.
        return deactivateCoroutine;
    }

    // Function that starts the WaitForLoad coroutine and returns it.
    public Coroutine Load()
    {
        // Start WaitForLoad() and return it, so it can be waited for.
        return StartCoroutine(WaitForLoad());

    }

    public IEnumerator LoadCustomData(){
        if(this.siteData.customData != null){
            CustomData custom = JsonUtility.FromJson<CustomData>(this.siteData.customData);
            if(custom.splines != null){
                idleAnimation = new SplineIdle(custom.splines);
            }
            if(custom.audio != null && custom.audio.filepath != null){
                Debug.Log("loading audio from " + custom.audio.filepath);
                WWW audiowww = new WWW(custom.audio.filepath);
                while(!audiowww.isDone){
                    yield return null;
                }
                AudioClip clip = audiowww.GetAudioClipCompressed(false);
                clip.name = "asdf";
                AudioSource source = this.gameObject.AddComponent<AudioSource>();
                source.clip = clip;
                source.loop = custom.audio.loop;
                source.playOnAwake = false;
                audiowww.Dispose();
            }
        }
    }

    // Unloads this object, by starting the Unload coroutine and returning it.
    public Coroutine Unload()
    {
        // Start WaitForUnload(), and return it so it can be waited for.
        return StartCoroutine(WaitForUnload());
    }

    // Wait for the object to load. Adds a layer of abstraction.
    private IEnumerator WaitForLoad()
    {

        // Don't try to load two different elements at once. Could get messy.
        while (SiteManager.loading)
        {
            yield return null;
        }

        // Lock user input so they can't interact while an object is loading.
        GamepadInput.LockInput(true);
        SiteManager.loading = true;

        // Show the loading status.
        StatusText.Show();

        // If there's no specified scene name in the JSON, then we just load the data normally.
        if (string.IsNullOrEmpty(siteData.sceneName))
        {

            yield return StartCoroutine(LoadCoroutine());

            yield return StartCoroutine(LoadCustomData());
        }

        // Otherwise, we just load the scene specified in JSON.
        else
        {
            // Variable to keep track if we actually found the scene or not.
            bool foundScene = false;

            // Iterate through all existing scenes.
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {

                // Get the scene at specified index.
                Scene scene = SceneManager.GetSceneAt(i);

                // Check if the names match.
                if (scene.name.Equals(siteData.sceneName))
                {

                    // If they match, scene was found. Load the scene.
                    foundScene = true;
                    AsyncOperation sceneLoad = SceneManager.LoadSceneAsync(siteData.sceneName);

                    // Load the scene specified in JSON and wait for it to finish loading.
                    while (!sceneLoad.isDone)
                    {
                        yield return null;
                    }

                    // Break, since we found the scene.
                    break;
                }
            }

            // If we didn't find the scene, log an error and try to load normally... which probably won't work.
            if (!foundScene)
            {
                Debug.LogErrorFormat("Could not find scene name '{0}', which was provided in JSON data file. Ensure this scene exists and is added to build settings.");
                yield return StartCoroutine(LoadCoroutine());
            }

        }

        // Everything is now loaded.
        loaded = true;

        // Hide the status text.
        StatusText.Hide();

        // Unlock input and mark scene as not loading anymore.
        GamepadInput.LockInput(false);
        SiteManager.loading = false;
    }

    // Coroutine to wait for the unload coroutine.
    private IEnumerator WaitForUnload()
    {
        // Waits for unload, then marks it as unloaded afterwards.
        yield return StartCoroutine(UnloadCoroutine());
        loaded = false;
    }

    // Wait for this element to load, then activate it.
    private IEnumerator LoadThenActivate()
    {

        // If this object is not already loaded, then load it and wait for it to finish.
        if (!loaded)
        {
            Debug.LogFormat(gameObject, "Site element {0} not yet loaded. Will activate when done loading.", gameObject.name);
            yield return Load();
        }

        yield return StartCoroutine(ActivateCustomData());

        // Activate the element now that it's guaranteed to have loaded.
        yield return StartCoroutine(ActivateCoroutine());

    }

    private IEnumerator ActivateCustomData(){
        //Check for custom options to do
        CustomData custom = JsonUtility.FromJson<CustomData>(this.siteData.customData);

        var audiosource = this.gameObject.GetComponent<AudioSource>();
        if(audiosource != null){
            Debug.Log("playing audio");
            audiosource.Play();
        }
        yield return null;
    }

    private void DeactivateCustomData(){
        var audiosource = this.gameObject.GetComponent<AudioSource>();
        if(audiosource != null){
            Debug.Log("stopping audio");
            audiosource.Stop();
        }
    }
    
    // Print an incorrect type error.
    protected void PrintIncorrectTypeError(string siteName, string dataType)
    {
        Debug.LogErrorFormat("Could not load site element {0} at site {1}: Incorrect data passed to Activate method", dataType, siteName);
    }

    // Get an absolute path of data on the computer.
    public string GetAbsolutePath(string relativeDataPath)
    {

        // Create the full path.
        string filePath = SiteManager.pathToDataFolder + "/" + relativeDataPath;

        // Ensure the file actually exists
        if (!File.Exists(SiteManager.pathToDataFolder + "/" + "config_3.json"))
        {
            Debug.LogWarning("Could not find config file: " + SiteManager.pathToDataFolder + "/" + "config_3.json");
        }

        // Ensure the full filepath exists
        if (!File.Exists(filePath))
        {
            Debug.LogWarning("Could not find data file at: " + filePath);
        }

        // Return the absolute path.
        return filePath;

    }

    public string GetDescription(){
        return this.siteData.description;
    }
}

[System.Serializable]
public abstract class SerializableSiteElement
{

    public int id;
    public string name;
    public string description;

    // IF the scene string is present, Unity will just load the scene with the specified name, ignoring all other JSON elements.
    public string sceneName;

    public string customData;

}

[System.Serializable]
public class CustomData{
    public string modelType;
    public JSONTransform[] splines;
    public Vector3 translation;
    public JSONAudio audio;
}

[System.Serializable]
public class JSONAudio{
    public string filepath;
    public bool loop;
}