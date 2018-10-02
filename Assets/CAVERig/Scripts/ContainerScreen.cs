// This script should be attached to a Camera object 
// in Unity. Once a Plane object is specified as the 
// "projectionScreen", the script computes a suitable
// view and projection matrix for the camera.
// The code is based on Robert Kooima's publication  
// "Generalized Perspective Projection," 2009, 
// http://csc.lsu.edu/~kooima/pdfs/gen-perspective.pdf 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerScreen : MonoBehaviour
{
    public List<GameObject> projectionScreens;
    public bool estimateViewFrustum = true;
    public bool setNearClipPlane = false;
    public float nearClipDistanceOffset = -0.01f;

    private Camera cameraComponent;

    void LateUpdate()
    {
        cameraComponent = GetComponent<Camera>();
        if (null != cameraComponent)
        {
            Vector3 pa = Vector3.zero;
            // lower left corner in world coordinates
            Vector3 pb = Vector3.zero;
            // lower right corner
            Vector3 pc = Vector3.zero;
            // upper left corner

            bool first = true;

            foreach(GameObject screen in projectionScreens){
                Vector3 tempPa = cameraComponent.transform.InverseTransformPoint(
                    screen.transform.TransformPoint(new Vector3(-5.0f, 0.0f, -5.0f)));
                Vector3 tempPb = cameraComponent.transform.InverseTransformPoint(
                    screen.transform.TransformPoint(new Vector3(5.0f, 0.0f, -5.0f)));
                Vector3 tempPc = cameraComponent.transform.InverseTransformPoint(
                    screen.transform.TransformPoint(new Vector3(-5.0f, 0.0f, 5.0f)));
            
                if(first){
                    pa = tempPa;
                    pb = tempPb;
                    pc = tempPc;
                    first = false;
                }
                else{
                    if(tempPa.x < pa.x) pa.x = tempPa.x;
                    if(tempPa.y < pa.y) pa.y = tempPa.y;
                    if(tempPa.z > pa.z) pa.z = tempPa.z;

                    if(tempPb.x > pb.x) pb.x = tempPb.x;
                    if(tempPb.y < pb.y) pb.y = tempPb.y;
                    if(tempPb.z > pb.z) pb.z = tempPb.z;

                    if(tempPc.x < pc.x) pc.x = tempPc.x;
                    if(tempPc.y > pc.y) pc.y = tempPc.y;
                    if(tempPc.z > pc.z) pc.z = tempPc.z;
                }
            }

            //add buffer room
            pa.x *= 1.2f;
            pa.y *= 1.2f;

            pb.x *= 1.2f;
            pb.y *= 1.2f;

            pc.x *= 1.2f;
            pc.y *= 1.2f;

            pa = cameraComponent.transform.TransformPoint(pa);
            pb = cameraComponent.transform.TransformPoint(pb);
            pc = cameraComponent.transform.TransformPoint(pc);


            Vector3 pe = transform.position;
            // eye position
            float n = cameraComponent.nearClipPlane;
            // distance of near clipping plane
            float f = cameraComponent.farClipPlane;
            // distance of far clipping plane

            Vector3 va; // from pe to pa
            Vector3 vb; // from pe to pb
            Vector3 vc; // from pe to pc
            Vector3 vr; // right axis of screen
            Vector3 vu; // up axis of screen
            Vector3 vn; // normal vector of screen

            float l; // distance to left screen edge
            float r; // distance to right screen edge
            float b; // distance to bottom screen edge
            float t; // distance to top screen edge
            float d; // distance from eye to screen 

            vr = pb - pa;
            vu = pc - pa;
            va = pa - pe;
            vb = pb - pe;
            vc = pc - pe;

            // are we looking at the backface of the plane object?
            if (Vector3.Dot(-Vector3.Cross(va, vc), vb) < 0.0)
            {
                // mirror points along the z axis (most users 
                // probably expect the x axis to stay fixed)
                vu = -vu;
                pa = pc;
                pb = pa + vr;
                pc = pa + vu;
                va = pa - pe;
                vb = pb - pe;
                vc = pc - pe;
            }

            vr.Normalize();
            vu.Normalize();
            vn = -Vector3.Cross(vr, vu);
            // we need the minus sign because Unity 
            // uses a left-handed coordinate system
            vn.Normalize();

            d = -Vector3.Dot(va, vn);
            if (setNearClipPlane)
            {
                n = d + nearClipDistanceOffset;
                cameraComponent.nearClipPlane = n;
            }
            l = Vector3.Dot(vr, va) * n / d;
            r = Vector3.Dot(vr, vb) * n / d;
            b = Vector3.Dot(vu, va) * n / d;
            t = Vector3.Dot(vu, vc) * n / d;

            Matrix4x4 p = new Matrix4x4(); // projection matrix 
            p[0, 0] = 2.0f * n / (r - l);
            p[0, 1] = 0.0f;
            p[0, 2] = (r + l) / (r - l);
            p[0, 3] = 0.0f;

            p[1, 0] = 0.0f;
            p[1, 1] = 2.0f * n / (t - b);
            p[1, 2] = (t + b) / (t - b);
            p[1, 3] = 0.0f;

            p[2, 0] = 0.0f;
            p[2, 1] = 0.0f;
            p[2, 2] = (f + n) / (n - f);
            p[2, 3] = 2.0f * f * n / (n - f);

            p[3, 0] = 0.0f;
            p[3, 1] = 0.0f;
            p[3, 2] = -1.0f;
            p[3, 3] = 0.0f;

            Matrix4x4 rm = new Matrix4x4(); // rotation matrix;
            rm[0, 0] = vr.x;
            rm[0, 1] = vr.y;
            rm[0, 2] = vr.z;
            rm[0, 3] = 0.0f;

            rm[1, 0] = vu.x;
            rm[1, 1] = vu.y;
            rm[1, 2] = vu.z;
            rm[1, 3] = 0.0f;

            rm[2, 0] = vn.x;
            rm[2, 1] = vn.y;
            rm[2, 2] = vn.z;
            rm[2, 3] = 0.0f;

            rm[3, 0] = 0.0f;
            rm[3, 1] = 0.0f;
            rm[3, 2] = 0.0f;
            rm[3, 3] = 1.0f;
                          
            Matrix4x4 tm = new Matrix4x4(); // translation matrix;
            tm[0, 0] = 1.0f;
            tm[0, 1] = 0.0f;
            tm[0, 2] = 0.0f;
            tm[0, 3] = -pe.x;

            tm[1, 0] = 0.0f;
            tm[1, 1] = 1.0f;
            tm[1, 2] = 0.0f;
            tm[1, 3] = -pe.y;

            tm[2, 0] = 0.0f;
            tm[2, 1] = 0.0f;
            tm[2, 2] = 1.0f;
            tm[2, 3] = -pe.z;

            tm[3, 0] = 0.0f;
            tm[3, 1] = 0.0f;
            tm[3, 2] = 0.0f;
            tm[3, 3] = 1.0f;

            // set matrices
            cameraComponent.projectionMatrix = p;
            cameraComponent.worldToCameraMatrix = rm * tm;
            // The original paper puts everything into the projection 
            // matrix (i.e. sets it to p * rm * tm and the other 
            // matrix to the identity), but this doesn't appear to 
            // work with Unity's shadow maps.

            if (estimateViewFrustum)
            {
                // rotate camera to screen for culling to work
                Quaternion q = new Quaternion();
                q.SetLookRotation((0.5f * (pb + pc) - pe), vu);
                // look at center of screen
                cameraComponent.transform.rotation = q;

                // set fieldOfView to a conservative estimate 
                // to make frustum tall enough
                if (cameraComponent.aspect >= 1.0)
                {
                    cameraComponent.fieldOfView = Mathf.Rad2Deg *
                       Mathf.Atan(((pb - pa).magnitude + (pc - pa).magnitude)
                       / va.magnitude);
                }
                else
                {
                    // take the camera aspect into account to 
                    // make the frustum wide enough 
                    cameraComponent.fieldOfView =
                       Mathf.Rad2Deg / cameraComponent.aspect *
                       Mathf.Atan(((pb - pa).magnitude + (pc - pa).magnitude)
                       / va.magnitude);
                }
            }
        }



    }


}