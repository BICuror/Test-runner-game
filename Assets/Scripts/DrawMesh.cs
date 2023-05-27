using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public sealed class DrawMesh : MonoBehaviour 
{
    [SerializeField] private float _minimalDrawDistance;
    [SerializeField] private float _thickness;

    [SerializeField] private float _sensetivity;

    public UnityEvent<Vector3[]> ShapeFinished;
    private MeshFilter _meshFilter;
    private Mesh _mesh;
    private List<Vector3> _mousePositions;

    private Camera _camera;

    private void Start() 
    {
        _meshFilter = GetComponent<MeshFilter>();

        _camera = Camera.main;
    }

    private void Update() 
    {
        if (Input.GetMouseButtonDown(0)) 
        {
            StartMesh();
        }
        else if (Input.GetMouseButton(0)) 
        {
            TryExpandingMesh();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            _meshFilter.mesh = null;

            FinishShape();
        }
    }

    private void StartMesh()
    {
        _mesh = new Mesh();

        _mousePositions = new List<Vector3>();

        Vector3[] vertices = new Vector3[4];
        Vector2[] uv = new Vector2[4];
        int[] triangles = new int[6];

        vertices[0] = GetPointerPosition();
        vertices[1] = GetPointerPosition();
        vertices[2] = GetPointerPosition();
        vertices[3] = GetPointerPosition();

        uv[0] = Vector2.zero;
        uv[1] = Vector2.zero;
        uv[2] = Vector2.zero;
        uv[3] = Vector2.zero;

        triangles[0] = 0;
        triangles[1] = 3; 
        triangles[2] = 1;

        triangles[3] = 1;
        triangles[4] = 3;
        triangles[5] = 2;

        _mesh.vertices = vertices;
        _mesh.uv = uv;
        _mesh.triangles = triangles;
        _mesh.MarkDynamic();

        _meshFilter.mesh = _mesh;

        _mousePositions.Add(GetPointerPosition());
    }

    private void TryExpandingMesh()
    {
        if (Vector3.Distance(GetPointerPosition(), _mousePositions[_mousePositions.Count - 1]) >_minimalDrawDistance) 
        {
            Vector3[] vertices = new Vector3[_mesh.vertices.Length + 2];
            Vector2[] uv = new Vector2[_mesh.uv.Length + 2];
            int[] triangles = new int[_mesh.triangles.Length + 6];

            _mesh.vertices.CopyTo(vertices, 0);
            _mesh.uv.CopyTo(uv, 0);
            _mesh.triangles.CopyTo(triangles, 0);

            int vIndex = vertices.Length - 4;
            int vIndex0 = vIndex + 0;
            int vIndex1 = vIndex + 1;
            int vIndex2 = vIndex + 2;
            int vIndex3 = vIndex + 3;

            Vector3 mouseForwardVector = (GetPointerPosition() - _mousePositions[_mousePositions.Count - 1]).normalized;
            Vector3 normal2D = new Vector3(0, 0, -1f);

            Vector3 newVertexUp = GetPointerPosition() + Vector3.Cross(mouseForwardVector, normal2D) * _thickness;
            Vector3 newVertexDown = GetPointerPosition() + Vector3.Cross(mouseForwardVector, normal2D * -1f) * _thickness;

            vertices[vIndex2] = newVertexUp;
            vertices[vIndex3] = newVertexDown;

            uv[vIndex2] = Vector2.zero;
            uv[vIndex3] = Vector2.zero;

            int tIndex = triangles.Length - 6;

            triangles[tIndex + 0] = vIndex0;
            triangles[tIndex + 1] = vIndex2;
            triangles[tIndex + 2] = vIndex1;

            triangles[tIndex + 3] = vIndex1;
            triangles[tIndex + 4] = vIndex2;
            triangles[tIndex + 5] = vIndex3;

            _mesh.vertices = vertices;
            _mesh.uv = uv;
            _mesh.triangles = triangles;

            _mousePositions.Add(GetPointerPosition());
        }
    }

    private void FinishShape()
    {
        Vector3[] array = new Vector3[_mousePositions.Count];

        for (int i = 0; i < array.Length; i++)
        {
            array[i] = _mousePositions[i];// / _sensetivity + new Vector3(0f, 0.75f, 0f);
        }

        ShapeFinished.Invoke(array);

        _mousePositions = new List<Vector3>();
    }

    private Vector3 GetPointerPosition()
    {
        Vector3 pointerPositionOnSceen = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1f) * _sensetivity;

        return _camera.ScreenToViewportPoint(pointerPositionOnSceen) - new Vector3(0.5f, 0.5f, 0f); 
    }
}