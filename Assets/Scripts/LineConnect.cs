using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
[ExecuteInEditMode]
public class LineConnect : MonoBehaviour
{

    [SerializeField]
    private GameObject _ObjectA;

    [SerializeField]
    private Vector3 _OffsetA;

    [SerializeField]
    private GameObject _ObjectB;

    [SerializeField]
    private Vector3 _OffsetB;

    private LineRenderer _line;
    private Transform transformA;
    private Transform transformB;

    void Start()
    {
        _line = gameObject.GetComponent<LineRenderer>();

        _line.positionCount = 2;
        _line.useWorldSpace = true;
    }

    void Update()
    {
        transformA = _ObjectA.transform;
        transformB = _ObjectB.transform;

        CreatePoints();
    }

    void CreatePoints()
    {
        if (_ObjectA == null || _ObjectB == null) return;
        _line.SetPosition(0, transformA.position + _OffsetA);
        _line.SetPosition(1, transformB.position + _OffsetB);
    }
}