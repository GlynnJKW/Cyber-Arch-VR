using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SideMenuManager : MonoBehaviour {

    public PlaceData[] data;
    public Button[] buttons;
    private SplineInterpolator splineInterpolator;
    private SplineController splineController;
    private bool firstTransition = true;
    private GameObject topNode;
  
    private Vector3 currPos;
    private Vector3 nextPos;
    private Quaternion currRot;
    private Quaternion nextRot;
	// Use this for initialization
	void Start () {
        SetupData();
        SetupButtons();
        splineInterpolator = GameObject.Find("Main Camera").GetComponent<SplineInterpolator>();
        splineController = GameObject.Find("Main Camera").GetComponent<SplineController>();
        topNode = new GameObject("Side Menu");
        topNode.transform.position = new Vector3(-100.0f, -79.0f, 106.0f);
        topNode.transform.rotation = Quaternion.Euler(77.5f, 5.6f, 0);
    }


    void SetupData() {
        data = new PlaceData[5];
        data[0] = new PlaceData("place1", new Vector3(-240, -456, -450), new Vector3(14.1f,55.7f,0f));
        data[1] = new PlaceData("place2", new Vector3(-429, -463, -300), new Vector3(14.1f, 55.7f, 0f));
        data[2] = new PlaceData("place3", new Vector3(-278, -463, 175), new Vector3(14.1f, 55.7f, 0f));
        data[3] = new PlaceData("place4", new Vector3(-150, -400, 10), new Vector3(14.1f, 55.7f, 0f));
        data[4] = new PlaceData("place5", new Vector3(-119, -437, 415), new Vector3(14.1f, 55.7f, 0f));

    }


    void SetupButtons() {
        for(int i = 0; i < buttons.Length; i++) {
            int closureIndex = i;
            buttons[i].GetComponentInChildren<Text>().text = data[i].name;
            //add listener to 
            buttons[closureIndex].onClick.AddListener(() => ButtonOnClick(closureIndex));
        }
    }


    public void ButtonOnClick(int index) {
     
      
        if (firstTransition) {
            currPos = GameObject.Find("Main Camera").transform.position;
            currRot = GameObject.Find("Main Camera").transform.rotation;
            GameObject currSplineRoot = new GameObject();
            GameObject firstNode = new GameObject();
            firstNode.name = "1";
            firstNode.transform.position = currPos;
            firstNode.transform.rotation = currRot;
            firstNode.transform.parent = currSplineRoot.transform;

            GameObject targetNode = new GameObject();
            targetNode.name = "3";
            targetNode.transform.position = data[index].pos;
            targetNode.transform.rotation = data[index].rot;
            
            targetNode.transform.parent = currSplineRoot.transform;

            topNode.name = "2";
            topNode.transform.parent = currSplineRoot.transform;
      
            splineController.SplineRoot = currSplineRoot;
            //splineInterpolator.Reset();
            splineController.Duration = 3;
            splineController.WrapMode = eWrapMode.ONCE;
            splineController.FollowSpline();
            
        }
        
            
    }
    

	// Update is called once per frame
	void Update () {
		
	}
}
