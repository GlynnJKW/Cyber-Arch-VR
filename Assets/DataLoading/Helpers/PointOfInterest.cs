using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointOfInterest : MonoBehaviour
{

    public float baseHeight = 2;
    public float tapeWidth = 0.5f;


    public List<Vector3> boundaries;

    public List<GameObject> poles;
    private GameObject miasma;

    public GameObject poleprefab;
    public Material ribbonMat;

    // Start is called before the first frame update
    void Start()
    {
        //Create ribbon
        Mesh mesh = new Mesh();
        MeshFilter meshFilter = this.gameObject.AddComponent<MeshFilter>();
        
        List<Vector3> verts = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> indices = new List<int>();
        for(int curr = 0; curr < boundaries.Count; ++curr){
            int next = (curr + 1) % boundaries.Count;

            Vector3 bl = boundaries[curr];
            bl.y += baseHeight;
            Vector3 tl = boundaries[curr];
            tl.y += baseHeight + tapeWidth;
            Vector3 br = boundaries[next];
            br.y += baseHeight;
            Vector3 tr = boundaries[next];
            tr.y += baseHeight + tapeWidth;

            verts.Add(tl);
            verts.Add(bl);
            verts.Add(br);

            verts.Add(br);
            verts.Add(tr);
            verts.Add(tl);

            Vector2 bluv, tluv, bruv, truv;
            bluv = new Vector2(0, 0);
            tluv = new Vector2(0, 1);
            bruv = new Vector2(1, 0);
            truv = new Vector2(1, 1);

            uvs.Add(tluv);
            uvs.Add(bluv);
            uvs.Add(bruv);

            uvs.Add(bruv);
            uvs.Add(truv);
            uvs.Add(tluv);

            int[] ind = { curr * 6, curr * 6 + 1, curr * 6 + 2, curr * 6 + 3, curr * 6 + 4, curr * 6 + 5 };
            indices.AddRange(ind);
        }

        mesh.vertices = verts.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.triangles = indices.ToArray();
        meshFilter.mesh = mesh;
        var meshRenderer = this.gameObject.AddComponent<MeshRenderer>();
        meshRenderer.material = ribbonMat;


        //Create miasma
        miasma = new GameObject("miasma");
        miasma.transform.parent = this.transform;
        Mesh miasmaMesh = new Mesh();
        MeshFilter miasmaFilter = miasma.AddComponent<MeshFilter>();

        List<Vector3> miasmaVerts = new List<Vector3>();
        List<int> miasmaIndices = new List<int>();
        for(int curr = 0; curr < boundaries.Count; ++curr){
            Vector3 bot = boundaries[curr];
            Vector3 top = boundaries[curr];
            top.y += 1000;
            miasmaVerts.Add(bot);
            miasmaVerts.Add(top);

            int currInd = curr * 2;
            int nextInd = ((curr + 1) % boundaries.Count) * 2;

            // tl, bl, br,   br, tr, tl
            int[] currRect = new int[]{currInd+1, currInd, nextInd, nextInd, nextInd+1, currInd+1};
            miasmaIndices.AddRange(currRect);
        }
        miasmaMesh.vertices = miasmaVerts.ToArray();
        miasmaMesh.triangles = miasmaIndices.ToArray();
        miasmaFilter.mesh = miasmaMesh;
        var miasmaRenderer = miasma.AddComponent<MeshRenderer>();
        miasmaRenderer.material = Resources.Load<Material>("Materials/miasma");



        //Create poles
        poles = new List<GameObject>();
        foreach(Vector3 pole in boundaries){
            GameObject obj = Instantiate<GameObject>(poleprefab, pole, Quaternion.identity);
            obj.transform.localScale = new Vector3(1, baseHeight + tapeWidth + 0.1f, 1);
            poles.Add(obj);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnDestroy(){
        foreach(var pole in poles){
            Destroy(pole);
        }
        Destroy(miasma);
    }
}
