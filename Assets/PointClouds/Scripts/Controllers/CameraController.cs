using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Controllers {
    /*
     * CameraController for flying-controls
     */
    public class CameraController : MonoBehaviour {

        //Current yaw
        private float yaw = 0.0f;
        //Current pitch
        private float pitch = 0.0f;
       
        //public float transitionDuration = 3.5f;
        //public float rotationDuration = 0.5f;

        //flag to fly over
        private bool stopFlying = false;
        
        public float normalSpeed = 100;

        //public List<Transform> pathNodes = new List<Transform>();

        //Transform currNode, targetNode;
        //int targetIndex = 1;

        //public Coroutine co;

        //IEnumerator Transition() {

        //    while (true) {

        //        float t = 0.0f;
        //        Vector3 startingPos = currNode.position;
        //        Vector3 targetPos = targetNode.position;
          
        //        while (t < 1.0f) {
        //            t += Time.deltaTime * (Time.timeScale / transitionDuration);
               
        //            transform.position = Vector3.Lerp(startingPos, targetPos, t);
        //            yield return 0;
        //        }

                
        //        currNode = pathNodes[targetIndex];
        //        targetIndex++;
      
        //        if (targetIndex == 4) {
        //            targetIndex = 0;
        //        }
        //        targetNode = pathNodes[targetIndex];

        //        Quaternion currRotation = transform.rotation;
        //        Vector3 relativePos = targetNode.position - transform.position;
        //        Quaternion targetRotation = Quaternion.LookRotation(relativePos);
               
        //        float rt = 0.0f;
        //        while (rt < 1.0f) {
        //            rt += Time.deltaTime * (Time.timeScale / transitionDuration);
        //            transform.rotation = Quaternion.Slerp(currRotation, targetRotation, rt);
        //            yield return 0;
        //        }
        //        transform.rotation = targetRotation;
          

        //        rt = 0.0f;
        //        t = 0.0f;

        //    }
        //}


        void Start() {
            
       

        }

        void FixedUpdate() {
            if (stopFlying == true) {
                float moveHorizontal = Input.GetAxis("Horizontal");
                float moveVertical = Input.GetAxis("Vertical");
                float moveUp = Input.GetKey(KeyCode.E) ? 1 : Input.GetKey(KeyCode.Q) ? -1 : 0;

                float speed = normalSpeed;
                if (Input.GetKey(KeyCode.C)) {
                    speed /= 10; ;
                }
                else if (Input.GetKey(KeyCode.LeftShift)) {
                    speed *= 5;
                }
                transform.Translate(new Vector3(moveHorizontal * speed * Time.deltaTime, moveUp * speed * Time.deltaTime, moveVertical * speed * Time.deltaTime));

                yaw += 2 * Input.GetAxis("Mouse X");
                pitch -= 2 * Input.GetAxis("Mouse Y");
                transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);
            }
        }

        public void stopFly() {
      
            stopFlying = true;

        }

    }

}
