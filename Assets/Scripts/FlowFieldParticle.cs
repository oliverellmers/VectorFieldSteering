using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowFieldParticle : MonoBehaviour {

    const int POINT_COUNT = 150; //amount of points  = length
    const float POINT_SPACING = 0.2f;
    const float DISPERSION = 0.3f;
    const float RADIUS = 0.05f;
    const int EDGE_COUNT = 6;

    private TubeRenderer tube;

    private List<MeshFilter> meshesToCombine;
    private GameObject combinedMesh;

    public float _moveSpeed;
    public Material tubeMaterial;

    void Start () {
        
        tube = new GameObject("Tube").AddComponent<TubeRenderer>();
        tube.transform.parent = GameObject.Find("NoiseFlowField").transform;
        tube.GetComponent<MeshRenderer>().material = tubeMaterial;

        tube.MarkDynamic();
        //tube.caps = TubeRenderer.CapMode.None;
        tube.caps = TubeRenderer.CapMode.Both;
        tube.edgeCount = EDGE_COUNT;

        // Define uv mapping for the end caps.
        //tube.uvRectCap = new Rect(0, 0, 4 / 12f, 4 / 12f);

        // Define points and radiuses.
        tube.points = new Vector3[POINT_COUNT];
        tube.radiuses = new float[POINT_COUNT];
        
        Vector3 pos = this.transform.position;

        for (int p = 0; p < POINT_COUNT; p++)
        {
            tube.points[p] = pos;
            tube.radiuses[p] = RADIUS;
        }

        combinedMesh = new GameObject("CombinedMesh");
        combinedMesh.transform.parent = GameObject.Find("NoiseFlowField").transform;
        combinedMesh.AddComponent<MeshFilter>();
        combinedMesh.AddComponent<MeshRenderer>().material = tubeMaterial;

        meshesToCombine = new List<MeshFilter>();
    }
	
	public void Update () {
        
        this.transform.position += transform.forward * _moveSpeed * Time.deltaTime;
        Vector3 pos = this.transform.position;
        tube.points[0] = pos;

        for (int p = 1; p < tube.points.Length; p++)
        {
            Vector3 forward = tube.points[p] - tube.points[p - 1];
            forward.Normalize();
            forward *= POINT_SPACING;
            tube.points[p] = tube.points[p - 1] + forward;
        }

        tube.points = tube.points;

    }

    public void ApplyRotation(Vector3 rotation, float rotateSpeed) {
        Quaternion targetRotation = Quaternion.LookRotation(rotation.normalized);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
    }

    public void ResetTubeMesh() {
        meshesToCombine.Clear();
        CombineMeshes(tube.GetComponent<MeshFilter>());

        Vector3 pos = this.transform.position;
        tube.points[0] = pos;

        
        for (int p = 0; p < POINT_COUNT; p++)
        {
            tube.points[p] = pos;
            tube.radiuses[p] = RADIUS;
        }
        
        
    }

    private void CombineMeshes(MeshFilter meshFilter) {
        

        MeshFilter currentMesh = combinedMesh.transform.GetComponent<MeshFilter>();
        MeshFilter meshToAdd = meshFilter;

        meshesToCombine.Add(currentMesh);
        meshesToCombine.Add(meshToAdd);

        CombineInstance[] combine = new CombineInstance[meshesToCombine.Count];
        int i = 0;
        while (i < meshesToCombine.Count)
        {
            MeshFilter mf = meshesToCombine[i];
            combine[i].mesh = mf.sharedMesh;
            combine[i].transform = mf.transform.localToWorldMatrix;
            i++;
        }

        Mesh combinedMeshMesh = new Mesh();
        combinedMeshMesh.CombineMeshes(combine);

        combinedMesh.GetComponent<MeshFilter>().mesh = combinedMeshMesh;
    }
}



