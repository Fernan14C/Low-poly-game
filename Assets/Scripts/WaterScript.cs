using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class XolotlanCPU : MonoBehaviour
{
    [Header("Matemática de Ondas")]
    public float amplitude1 = 0.3f;
    public float length1 = 10f;
    public float speed1 = 1f;

    public float amplitude2 = 0.15f;
    public float length2 = 5f;
    public float speed2 = 1.5f;

    // Agregamos una tercera onda para romper el patrón
    public float amplitude3 = 0.1f;
    public float length3 = 3f;
    public float speed3 = 2.5f;

    [Header("Afilado (Gerstner)")]
    [Range(0f, 1f)] public float steepness = 0.5f;

    [Header("Ajuste de Color (UV)")]
    // Este valor determina qué tan rápido cambia el degradado de color según la altura
    public float colorSensibility = 2f;

    private MeshFilter meshFilter;
    private Mesh mesh;
    private Vector3[] originalVertices;
    private Vector3[] workingVertices;
    private Vector2[] uvs;
    private float _offset;

    public float GetWaveHeight(Vector3 _position)
    {
        float _totalHeight = 0;

        // Onda 1: Movimiento mayormente en X
        float _phase1 = (_position.x * 0.9f + _position.z * 0.1f) / length1 + _offset * speed1;
        _totalHeight += amplitude1 * Mathf.Sin(_phase1);

        // Onda 2: Movimiento mayormente en Z
        float _phase2 = (_position.z * 0.9f - _position.x * 0.1f) / length2 + _offset * speed2;
        _totalHeight += amplitude2 * Mathf.Sin(_phase2);

        // Onda 3: Movimiento Diagonal "Ruido"
        // Esta es la que romperá el patrón repetitivo
        float _phase3 = (_position.x * 0.5f + _position.z * 0.5f) / length3 + _offset * speed3;
        _totalHeight += amplitude3 * Mathf.Sin(_phase3);

        return _totalHeight;
    }

    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();

        // 1. INGENIERÍA CRÍTICA: Flat Shading
        // Forzamos a la malla a tener vértices únicos por triángulo para el look low-poly.
        MakeMeshFlatShaded(meshFilter.sharedMesh);

        // Obtenemos la malla de trabajo ya flat-shaded
        mesh = meshFilter.mesh;
        originalVertices = mesh.vertices;
        workingVertices = new Vector3[originalVertices.Length];
        uvs = new Vector2[originalVertices.Length];
    }

    void Update()
    {
        _offset += Time.deltaTime;

        // 1. Calculamos el rango de altura total para el degradado
        float _totalMaxHeight = amplitude1 + amplitude2 + amplitude3;

        for (int i = 0; i < originalVertices.Length; i++)
        {
            Vector3 _vertex = originalVertices[i];
            Vector3 _worldPos = transform.TransformPoint(_vertex);

            float _yDisplacement = 0;
            float _xDisplacement = 0;
            float _zDisplacement = 0;

            // --- ONDA 1 (Diagonal suave) ---
            Vector2 _dir1 = new Vector2(0.8f, 0.2f).normalized;
            float _k1 = 2 * Mathf.PI / length1;
            float _phase1 = _k1 * ((_worldPos.x * _dir1.x + _worldPos.z * _dir1.y) + _offset * speed1);

            _yDisplacement += amplitude1 * Mathf.Sin(_phase1);
            _xDisplacement += _dir1.x * steepness * amplitude1 * Mathf.Cos(_phase1);
            _zDisplacement += _dir1.y * steepness * amplitude1 * Mathf.Cos(_phase1);

            // --- ONDA 2 (Cruzada) ---
            Vector2 _dir2 = new Vector2(-0.3f, 0.9f).normalized;
            float _k2 = 2 * Mathf.PI / length2;
            float _phase2 = _k2 * ((_worldPos.x * _dir2.x + _worldPos.z * _dir2.y) + _offset * speed2);

            _yDisplacement += amplitude2 * Mathf.Sin(_phase2);
            _xDisplacement += _dir2.x * steepness * amplitude2 * Mathf.Cos(_phase2);
            _zDisplacement += _dir2.y * steepness * amplitude2 * Mathf.Cos(_phase2);

            // --- ONDA 3 (Ruido para romper la lasaña) ---
            Vector2 _dir3 = new Vector2(0.5f, 0.5f).normalized;
            float _k3 = 2 * Mathf.PI / length3;
            float _phase3 = _k3 * ((_worldPos.x * _dir3.x + _worldPos.z * _dir3.y) + _offset * speed3);

            _yDisplacement += amplitude3 * Mathf.Sin(_phase3);
            _xDisplacement += _dir3.x * steepness * amplitude3 * Mathf.Cos(_phase3);
            _zDisplacement += _dir3.y * steepness * amplitude3 * Mathf.Cos(_phase3);

            // Aplicamos posición final
            workingVertices[i] = _vertex + new Vector3(_xDisplacement, _yDisplacement, _zDisplacement);

            // 2. Mapeo UV por Altura (para el color)
            float _normalizedHeight = Mathf.InverseLerp(-_totalMaxHeight, _totalMaxHeight, _yDisplacement * colorSensibility);
            uvs[i] = new Vector2(0.5f, _normalizedHeight);
        }

        mesh.vertices = workingVertices;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }

    // Función auxiliar para duplicar vértices y lograr Flat Shading
    void MakeMeshFlatShaded(Mesh originalMesh)
    {
        if (originalMesh == null) return;

        Vector3[] oldVertices = originalMesh.vertices;
        int[] triangles = originalMesh.triangles;
        Vector3[] newVertices = new Vector3[triangles.Length];

        // Duplicamos vértices basándonos en los índices de los triángulos
        for (int i = 0; i < triangles.Length; i++)
        {
            newVertices[i] = oldVertices[triangles[i]];
            triangles[i] = i; // Reset de índices
        }

        // Reasignamos a la malla
        originalMesh.vertices = newVertices;
        originalMesh.triangles = triangles;

        // Limpiamos datos viejos que ya no sirven
        originalMesh.uv = null;
        originalMesh.normals = null;
    }
}