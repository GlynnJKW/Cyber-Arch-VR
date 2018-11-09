using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementUI : MonoBehaviour {

    private bool created;

	protected Canvas elementCanvas;

	public ElementUI(){
		created = false;
		elementCanvas = ElementCanvas.Instance;
	}

	public void Create(){
		created = true;
		onCreate();		
	}


    public void Cleanup() {
        onCleanup();
    }

    // Update is called once per frame
    void Update () {
		if(created){
			onUpdate();
		}
	}

	protected virtual void onCreate(){}
	protected virtual void onCleanup(){}
	protected virtual void onUpdate(){}

}
