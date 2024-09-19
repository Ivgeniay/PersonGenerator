using Assets._Project.Test;
using AtomEngine.Testing.Inspector;
using System;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
[ExecuteInEditMode]
public class TestScr : MonoBehaviour, IPublicMethodsInspector
{
    public static TestScr Instance { get; private set; }
    [SerializeField] private Transform[] _pathPoints;
    [SerializeField] private Section section;

    int[] triangles;
    int createdQuads = 0;
    //void Start()
    //{
    //    Mesh mesh = new Mesh();
    //    mesh.name = "MyMesh";

    //    Vector3[] vertices = new Vector3[_pathPoints.Length * section.Points.Count];
    //    triangles = new int[(_pathPoints.Length - 1) * (section.Points.Count - 1) * 2 * 3];

    //    for (int p = 0; p < _pathPoints.Length; p++)
    //    {
    //        for (int s = 0; s < section.Points.Count; s++)
    //        {
    //            vertices[s + p * section.Points.Count] = _pathPoints[p].TransformPoint(section.Points[s].localPosition);
    //        }
    //    }

    //    for (int p = 0; p < _pathPoints.Length - 1; p++)
    //    {
    //        for (int s = 0; s < section.Points.Count - 1; s++)
    //        {
    //            int v0 = s + p * section.Points.Count;
    //            int v1 = s + 1 + p * section.Points.Count;
    //            int v2 = s + 1 + (p + 1) * section.Points.Count;
    //            int v3 = s + (p + 1) * section.Points.Count; ;

    //            CreateQuad(v0, v1, v2, v3);
    //        }
    //    }

    //    mesh.vertices = vertices;
    //    mesh.triangles = triangles;

    //    mesh.RecalculateNormals();
    //    mesh.RecalculateBounds(); 
    //    GetComponent<MeshFilter>().mesh = mesh;
    //}

    private void OnEnable()
    {
        TestScr.Instance = this;
    }
    private void Update()
    {
        if (TestScr.Instance != null) TestScr.Instance = this;
    }

    private void CreateQuad(int v0, int v1, int v2, int v3)
    {
        triangles[0 + createdQuads * 6] = v3;
        triangles[1 + createdQuads * 6] = v2;
        triangles[2 + createdQuads * 6] = v1;
 
        triangles[3 + createdQuads * 6] = v0;
        triangles[4 + createdQuads * 6] = v2;
        triangles[5 + createdQuads * 6] = v1;

        createdQuads++;
    }

    private void CreateOtherSideQuad(int v0, int v1, int v2, int v3)
    {
        //CreateQuad
    }

    [SerializeField]Vector3 intersectionPoint = Vector3.zero;

    private void OnDrawGizmos()
    {
        if (TestScr.Instance != null) TestScr.Instance = this;
        Gizmos.DrawSphere(intersectionPoint, 0.3f);  
    }
    internal static void Data(Vector3 intersectionPoint)
    {
        TestScr.Instance.intersectionPoint = intersectionPoint;
    }
}
