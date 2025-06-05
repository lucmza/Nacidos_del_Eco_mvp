using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class VisionConeDrawer : MonoBehaviour
{
    [SerializeField] private float _visionRange = 5f;
    [SerializeField] private float _visionAngle = 45f;
    [SerializeField] private int _segments = 30;

    private Mesh _mesh;

    private void Awake()
    {
        _mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = _mesh;
        GenerateMesh();
    }

    private void Update()
    {
        GenerateMesh();
    }

    private void GenerateMesh()
    {
        Vector3[] vertices = new Vector3[_segments + 2];
        int[] triangles = new int[_segments * 3];

        vertices[0] = Vector3.zero;

        float angleStep = (_visionAngle * 2) / _segments;

        for (int i = 0; i <= _segments; i++)
        {
            float angle = -_visionAngle + i * angleStep;
            float rad = angle * Mathf.Deg2Rad;

            vertices[i + 1] = new Vector3(Mathf.Sin(rad), 0, Mathf.Cos(rad)) * _visionRange;
        }

        for (int i = 0; i < _segments; i++)
        {
            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = i + 2;
        }

        _mesh.Clear();
        _mesh.vertices = vertices;
        _mesh.triangles = triangles;
        _mesh.RecalculateNormals();
        _mesh.RecalculateBounds();

    }
}
