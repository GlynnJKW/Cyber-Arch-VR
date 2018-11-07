using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KioskUI : MonoBehaviour {

    public static KioskUI instance;
    public UnityEngine.Rendering.CompareFunction comparison = UnityEngine.Rendering.CompareFunction.Always;

    public void Awake()
    {

        instance = this;

    }

    public void Show(bool show)
    {
        if (show)
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }


    }

    // Use this for initialization
    void Start () {
        FixUIOverlay(transform);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    //Fix the UI display in world space by switching off Z-Test in shader
    void FixUIOverlay(Transform trans) {
        if (trans.childCount == 0) return;
        foreach (Transform child in trans) {
            if (child.gameObject.GetComponent<Image>() != null) {
                Image image = child.gameObject.GetComponent<Image>();


                //get the current material
                Material currMaterial = image.materialForRendering;

                //create a new material
                Material updatedMaterial = new Material(currMaterial);

                //turn off zTest
                updatedMaterial.SetInt("unity_GUIZTestMode", (int)comparison);
                image.material = updatedMaterial;
            }
            if (child.gameObject.GetComponent<Text>() != null) {
                Text text = child.gameObject.GetComponent<Text>();


                //get the current material
                Material currMaterial = text.materialForRendering;

                //create a new material
                Material updatedMaterial = new Material(currMaterial);

                //turn off zTest
                updatedMaterial.SetInt("unity_GUIZTestMode", (int)comparison);
                text.material = updatedMaterial;
            }
            FixUIOverlay(child.transform);
        }
        return;

    }
}
