using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class createLakeMesh : MonoBehaviour
{
    public string lakeObjectName = "Lake";

    void Start()
    {
        // Get the "Lake" GameObject
        GameObject lakeObject = GameObject.Find(lakeObjectName);

        // Get the MeshFilter components from the child objects
        MeshFilter[] meshFilters = lakeObject.GetComponentsInChildren<MeshFilter>();

        // Check if there are at least three child MeshFilter components
        if (meshFilters.Length >= 3)
        {
            // Create a new combined mesh
            Mesh combinedMesh = new Mesh();

            // Create lists to store the combined vertices, triangles, and UVs
            List<Vector3> combinedVertices = new List<Vector3>();
            List<int> combinedTriangles = new List<int>();
            List<Vector2> combinedUVs = new List<Vector2>();

            int vertexOffset = 0; // Offset for vertex indices when combining meshes

            // Loop through the child MeshFilters
            for (int i = 0; i < meshFilters.Length; i++)
            {
                MeshFilter meshFilter = meshFilters[i];
                Mesh mesh = meshFilter.sharedMesh;

                // Add the vertices and UVs to the combined lists
                combinedVertices.AddRange(mesh.vertices);
                combinedUVs.AddRange(mesh.uv);

                // Add the triangles to the combined list with vertex offsets
                int[] triangles = mesh.triangles;
                for (int j = 0; j < triangles.Length; j++)
                {
                    combinedTriangles.Add(triangles[j] + vertexOffset);
                }

                // Update the vertex offset for the next mesh
                vertexOffset += mesh.vertexCount;

                // Optionally, you can remove the original child objects if desired
                Destroy(meshFilter.gameObject);
            }

            // Assign the combined vertices, triangles, and UVs to the combined mesh
            combinedMesh.vertices = combinedVertices.ToArray();
            combinedMesh.triangles = combinedTriangles.ToArray();
            combinedMesh.uv = combinedUVs.ToArray();

            // Recalculate normals and bounds for proper rendering
            combinedMesh.RecalculateNormals();
            combinedMesh.RecalculateBounds();

            // Connect vertices by creating additional triangles
            ConnectVertices(combinedMesh);

            // Create a new GameObject to hold the connected mesh
            GameObject connectedObject = new GameObject("ConnectedObject");
            connectedObject.AddComponent<MeshFilter>().sharedMesh = combinedMesh;

            // Assign the material from one of the child objects to the MeshRenderer component
            MeshRenderer meshRenderer = connectedObject.AddComponent<MeshRenderer>();
            meshRenderer.sharedMaterial = meshFilters[0].GetComponent<MeshRenderer>().sharedMaterial;
        }
        else
        {
            Debug.LogError("The 'Lake' object does not have at least three child objects with MeshFilter components.");
        }
    }

    private void ConnectVertices(Mesh mesh)
    {
        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;

        // Connect vertices by creating additional triangles
        for (int i = 0; i < vertices.Length; i += 3)
        {
            int vertexIndex1 = i;
            int vertexIndex2 = i + 1;
            int vertexIndex3 = i + 2;

            triangles = AddTriangle(triangles, vertexIndex1, vertexIndex2, vertexIndex3);
        }

        // Update the triangles in the mesh
        mesh.triangles = triangles;
    }

    private int[] AddTriangle(int[] triangles, int index1, int index2, int index3)
    {
        int[] newTriangles = new int[triangles.Length + 3];
        triangles.CopyTo(newTriangles, 0);

        newTriangles[newTriangles.Length - 3] = index1;
        newTriangles[newTriangles.Length - 2] = index2;
        newTriangles[newTriangles.Length - 1] = index3;

        return newTriangles;
    }
}