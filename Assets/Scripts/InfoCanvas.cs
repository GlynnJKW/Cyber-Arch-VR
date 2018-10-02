using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoCanvas : MonoBehaviour {

	public static InfoCanvas instance;
	public static string infoText {
		get{
			if(instance){
				return instance.gameObject.GetComponent<Text>().text;
			}
			else{
				return "";
			}
		}
		set{
			if(instance){
				value = value.Trim();
				instance.gameObject.GetComponent<Text>().text = value;
				Image backgroundImage = instance.gameObject.transform.parent.gameObject.GetComponent<Image>();
				backgroundImage.enabled = value != "";
			}
		}
	}

	void Awake(){
		instance = this;
	}
}
