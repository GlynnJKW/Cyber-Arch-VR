using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class ElementCanvas : MonoBehaviour {

	public static Canvas Instance;

	// Use this for initialization
	void Start () {
		if(!Instance){
			Instance = this.gameObject.GetComponent<Canvas>();
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
