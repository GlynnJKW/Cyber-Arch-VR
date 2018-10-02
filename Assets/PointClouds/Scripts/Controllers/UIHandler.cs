using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Controllers
{
    public class UIHandler : MonoBehaviour {

        public GameObject mainCam;

        public GameObject mainMenuPanel;
        public GameObject sideMeun;


        public void onFreeExploreClicked() {

            mainMenuPanel.SetActive(false);
            mainCam.GetComponent<SplineInterpolator>().Interrupt();
            mainCam.GetComponent<CameraController>().stopFly();

        }

        public void onSelectPlaceClicked() {

            mainMenuPanel = gameObject.transform.Find("MainMenuPanel").gameObject;
            mainMenuPanel.SetActive(false);
            sideMeun.SetActive(true);
        }

        // Use this for initialization
        void Start() {
            
            sideMeun.SetActive(false);
            
        }

        // Update is called once per frame
        void Update() {

        }
    }
}
