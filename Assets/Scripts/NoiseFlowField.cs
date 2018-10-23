using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseFlowField : MonoBehaviour {

    FastNoise _fastNoise;
    public Vector3Int _gridSize;
    public float _increment;
    public Vector3 _offSet, _offSetSpeed;
    public Transform sphereTransform;


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnDrawGizmos()
    {
        _fastNoise = new FastNoise();

        float xOff = 0;
        for (int x = 0; x < _gridSize.x; x++) {
            float yOff = 0;
            for (int y = 0; y < _gridSize.y; y++) {
                float zOff = 0;
                for (int z = 0; z < _gridSize.z; z++) {

                    //here we do the noise calculations
                    float noise = _fastNoise.GetSimplex(xOff + _offSet.x, yOff + _offSet.y, zOff + _offSet.z) + 1;

                    //Vector3 noiseDirection = new Vector3(Mathf.Cos(noise * Mathf.PI), Mathf.Sin(noise * Mathf.PI), Mathf.Cos(noise * Mathf.PI));
                    //Vector3 noiseDirection = new Vector3(sphereTransform.position.x/100 * x, sphereTransform.position.y/100 * y,sphereTransform.position.z/100 * z);
                    Vector3 noiseDirection = new Vector3(sphereTransform.position.x - x * Mathf.Cos(noise * Mathf.PI), sphereTransform.position.y - y * Mathf.Sin(noise * Mathf.PI), sphereTransform.position.z - z * Mathf.Cos(noise * Mathf.PI));

                    Gizmos.color = new Color(noiseDirection.normalized.x, noiseDirection.normalized.y, noiseDirection.normalized.z, 0.7f) ;
                    Vector3 pos = new Vector3(x, y, z) + transform.position;
                    //Vector3 size = new Vector3(1,1,1);
                    Vector3 endPos = pos + Vector3.Normalize(noiseDirection);

                    Gizmos.DrawLine(pos, endPos);
                    //Gizmos.DrawSphere(endPos, 0.1f);

                    zOff += _increment;
                }
                yOff += _increment;
            }
            xOff += _increment;
        }
    }
}
