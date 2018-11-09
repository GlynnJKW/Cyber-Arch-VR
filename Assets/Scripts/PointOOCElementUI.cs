using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PointOOCElementUI : ElementUI {


 
    protected override void onCreate() {
        base.onCreate();

        //Add basic properties for canvas
        GameObject go = elementCanvas.gameObject;
        RectTransform rectTransform;

        go.AddComponent<CanvasScaler>();
        go.AddComponent<GraphicRaycaster>();
        // Text
        GameObject myText = new GameObject();
        myText.transform.parent = go.transform;
        myText.name = "Test 123";

        Text text = myText.AddComponent<Text>();
        
        text.font = (Font)Resources.Load("MyFont");
        text.text = "wobble";
        text.fontSize = 100;

        // Text position
        rectTransform = text.GetComponent<RectTransform>();
        rectTransform.localPosition = new Vector3(0, 0, 0);
        rectTransform.sizeDelta = new Vector2(400, 200);

    }

    protected override void onCleanup() {
        base.onCleanup();
    }

    protected override void onUpdate() {
        base.onUpdate();
    }

    private void CreateButtons() {


    }

    public void FixUIOverlay() {


    }

}
