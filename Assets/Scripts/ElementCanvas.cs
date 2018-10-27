using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementCanvas : MonoBehaviour {

	public static Canvas Instance;
	
	[SerializeField]
	private float distance = 5;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if(!Instance && gameObject.GetComponent<Canvas>()){
			Instance = gameObject.GetComponent<Canvas>();
		}
		if(!Instance && CAVECameraRig.frustumCamera){
			Vector3 bottomleft = CAVECameraRig.frustumCamera.ScreenToWorldPoint(new Vector3(0, 0, distance));
			Vector3 topright = CAVECameraRig.frustumCamera.ScreenToWorldPoint(new Vector3(CAVECameraRig.frustumCamera.pixelWidth, CAVECameraRig.frustumCamera.pixelHeight, distance));

			float w = (topright.x - bottomleft.x) / 2;
			float h = (topright.y - bottomleft.y) / 2;

			Canvas c = gameObject.AddComponent<Canvas>();
			c.renderMode = RenderMode.WorldSpace;

			RectTransform r = gameObject.GetComponent<RectTransform>();
			r.position = (bottomleft + topright) / 2;
			r.sizeDelta = new Vector2(1920, 1080);
			r.localScale = new Vector3(w/1920.0f, h/1080.0f, 1);
			Instance = c;
		}

	}
}
