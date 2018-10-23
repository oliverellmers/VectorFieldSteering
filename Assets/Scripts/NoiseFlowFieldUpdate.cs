using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseFlowFieldUpdate : MonoBehaviour {

    FastNoise _fastNoise;

    public Vector3Int _gridSize;
    public float _cellSize;
    public Vector3[,,] _flowFieldDrection;
    public float _increment;
    public Vector3 _offSet, _offSetSpeed;
    public Transform sphereTransform;

    //particles
    public GameObject _particlePrefab;
    public int _amountOfParticles;
    [HideInInspector]
    public List<FlowFieldParticle> _particles;

    public float _spawnRadius;
    public float _particleScale, _particleMoveSpeed, _particleRotateSpeed;



    bool _particleSpawnValidation(Vector3 position) {

        bool valid = true;

        foreach (FlowFieldParticle particle in _particles)
        {
            if (Vector3.Distance(position, particle.transform.position) < _spawnRadius)
            {
                valid = false;
                break;

            }
        }

        if (valid)
        {
            return true;
        }
        else {
            return false;
        }
    }


	// Use this for initialization
	void Start () {
        _fastNoise = new FastNoise();

        _flowFieldDrection = new Vector3[_gridSize.x, _gridSize.y, _gridSize.z];
        _particles = new List<FlowFieldParticle>();

        for (int i = 0; i < _amountOfParticles; i++) {

            int attempt = 0;

            while (attempt < 100) {
                Vector3 randomPos = new Vector3(
                Random.Range(this.transform.position.x, this.transform.position.x + _gridSize.x * _cellSize),
                Random.Range(this.transform.position.y, this.transform.position.y + _gridSize.y * _cellSize),
                Random.Range(this.transform.position.z, this.transform.position.z + _gridSize.z * _cellSize));

                bool isValid = _particleSpawnValidation(randomPos);

                if (isValid)
                {
                    GameObject particleInstane = (GameObject)Instantiate(_particlePrefab);
                    particleInstane.transform.position = randomPos;
                    particleInstane.transform.parent = this.transform;
                    particleInstane.transform.localScale = new Vector3(_particleScale, _particleScale, _particleScale);
                    _particles.Add(particleInstane.GetComponent<FlowFieldParticle>());
                    break;
                }
                if (!isValid) {
                    attempt++;
                }
            }
        }

        Debug.Log(_particles.Count);

	}

    private void Update()
    {
        CalculateFlowFieldDirections();
        ParticleBehaviour();
    }

    // Update is called once per frame
    public void CalculateFlowFieldDirections () {

        _offSet = new Vector3(_offSet.x + (_offSetSpeed.x * Time.deltaTime), _offSet.y + (_offSetSpeed.y * Time.deltaTime), _offSet.z + (_offSetSpeed.z * Time.deltaTime) );

        float xOff = 0;
        for (int x = 0; x < _gridSize.x; x++)
        {
            float yOff = 0;
            for (int y = 0; y < _gridSize.y; y++)
            {
                float zOff = 0;
                for (int z = 0; z < _gridSize.z; z++)
                {

                    float noise = _fastNoise.GetSimplex(xOff + _offSet.x, yOff + _offSet.y, zOff + _offSet.z) + 1;

                    Vector3 noiseDirection = new Vector3(sphereTransform.position.x - x * Mathf.Cos(noise * Mathf.PI), sphereTransform.position.y - y * Mathf.Sin(noise * Mathf.PI), sphereTransform.position.z - z * Mathf.Cos(noise * Mathf.PI));
                    _flowFieldDrection[x, y, z] = Vector3.Normalize(noiseDirection);

                    Vector3 pos = new Vector3(x, y, z) + transform.position;
                    Vector3 endPos = pos + Vector3.Normalize(noiseDirection);

                    zOff += _increment;
                }
                yOff += _increment;
            }
            xOff += _increment;
        }
    }

    private void ParticleBehaviour() {
        foreach (FlowFieldParticle p in _particles) {

            //X edges
            if (p.transform.position.x > this.transform.position.x + (_gridSize.x * _cellSize)) {
                p.transform.position = new Vector3(this.transform.position.x, p.transform.position.y, p.transform.position.z);
            }
            if (p.transform.position.x < this.transform.position.x) {
                p.transform.position = new Vector3(this.transform.position.x + (_gridSize.x * _cellSize), p.transform.position.y, p.transform.position.z);
            }

            //Y edges
            if (p.transform.position.y > this.transform.position.y + (_gridSize.y * _cellSize))
            {
                p.transform.position = new Vector3(p.transform.position.x, this.transform.position.y, p.transform.position.z);
            }
            if (p.transform.position.y < this.transform.position.y)
            {
                p.transform.position = new Vector3(p.transform.position.x, this.transform.position.y + (_gridSize.y * _cellSize), p.transform.position.z);
            }

            //Z edges
            if (p.transform.position.z > this.transform.position.z + (_gridSize.z * _cellSize))
            {
                p.transform.position = new Vector3(p.transform.position.x, p.transform.position.y, this.transform.position.z);
            }
            if (p.transform.position.z < this.transform.position.z)
            {
                p.transform.position = new Vector3(p.transform.position.x, p.transform.position.y, this.transform.position.z + (_gridSize.z * _cellSize));
            }

            Vector3Int particlePos = new Vector3Int(
                Mathf.FloorToInt(Mathf.Clamp((p.transform.position.x - this.transform.position.x)/_cellSize, 0, _gridSize.x - 1)),
                Mathf.FloorToInt(Mathf.Clamp((p.transform.position.y - this.transform.position.y) / _cellSize, 0, _gridSize.y - 1)),
                Mathf.FloorToInt(Mathf.Clamp((p.transform.position.z - this.transform.position.z) / _cellSize, 0, _gridSize.z - 1))
                );

            p.ApplyRotation(_flowFieldDrection[particlePos.x, particlePos.y, particlePos.z], _particleRotateSpeed);
            p._moveSpeed = _particleMoveSpeed;
            p.transform.localScale = new Vector3(_particleScale, _particleScale, _particleScale);
            

            
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(this.transform.position + new Vector3( (_gridSize.x * _cellSize) * 0.5f, (_gridSize.y * _cellSize) * 0.5f, (_gridSize.z * _cellSize) * 0.5f), new Vector3(_gridSize.x * _cellSize, _gridSize.y * _cellSize, _gridSize.z * _cellSize));
    }

}
