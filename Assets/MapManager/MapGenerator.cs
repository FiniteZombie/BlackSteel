using UnityEngine;
using System.Collections;

public class MapGenerator : MonoBehaviour {
    //public Vector3[] newVertices;
    //public Vector2[] newUV;
    //public int[] newTriangles;
    //public Mesh wallMesh;

    // Use this for prep
    void Wake() {
        //newVertices = new Vector3[4];
        //newVertices[0] = new Vector3(0, 0, 0);
        //newVertices[1] = new Vector3(0, 1, 0);
        //newVertices[2] = new Vector3(1, 1, 0);
        //newVertices[3] = new Vector3(1, 0, 0);
        //newUV = new Vector2[newVertices.Length];

        //for (int i = 0; i < newUV.Length; i++) {
        //    newUV[i] = new Vector2(newVertices[i].x, newVertices[i].y);
        //}

        //newTriangles = new int[6];
        //newTriangles[0] = 0;
        //newTriangles[1] = 1;
        //newTriangles[2] = 2;
        //newTriangles[3] = 2;
        //newTriangles[4] = 3;
        //newTriangles[5] = 0;
    }

    // Use this for initialization
    void Start() {
        GenerateMap();

        //wallMesh = new Mesh();
        //GetComponent<MeshFilter>().mesh = wallMesh;
        //wallMesh.vertices = newVertices;
        //wallMesh.uv = newUV;
        //wallMesh.triangles = newTriangles;

        // You can change that line to provide another MeshFilter
        //MeshFilter filter = gameObject.AddComponent<MeshFilter>();
        //Mesh mesh = filter.mesh;
        //mesh.Clear();

        //float length = 1f;
        //float width = 1f;
        //int resX = 2; // 2 minimum
        //int resZ = 2;

        //#region Vertices
        //Vector3[] vertices = new Vector3[resX * resZ];
        //for (int z = 0; z < resZ; z++) {
        //    // [ -length / 2, length / 2 ]
        //    float zPos = ((float)z / (resZ - 1) - .5f) * length;
        //    for (int x = 0; x < resX; x++) {
        //        // [ -width / 2, width / 2 ]
        //        float xPos = ((float)x / (resX - 1) - .5f) * width;
        //        vertices[x + z * resX] = new Vector3(xPos, 0f, zPos);
        //    }
        //}
        //#endregion

        //#region Normales
        //Vector3[] normales = new Vector3[vertices.Length];
        //for (int n = 0; n < normales.Length; n++)
        //    normales[n] = Vector3.up;
        //#endregion

        //#region UVs
        //Vector2[] uvs = new Vector2[vertices.Length];
        //for (int v = 0; v < resZ; v++) {
        //    for (int u = 0; u < resX; u++) {
        //        uvs[u + v * resX] = new Vector2((float)u / (resX - 1), (float)v / (resZ - 1));
        //    }
        //}
        //#endregion

        //#region Triangles
        //int nbFaces = (resX - 1) * (resZ - 1);
        //int[] triangles = new int[nbFaces * 6];
        //int t = 0;
        //for (int face = 0; face < nbFaces; face++) {
        //    // Retrieve lower left corner from face ind
        //    int i = face % (resX - 1) + (face / (resZ - 1) * resX);

        //    triangles[t++] = i + resX;
        //    triangles[t++] = i + 1;
        //    triangles[t++] = i;

        //    triangles[t++] = i + resX;
        //    triangles[t++] = i + resX + 1;
        //    triangles[t++] = i + 1;
        //}
        //#endregion

        //mesh.vertices = vertices;
        //mesh.normals = normales;
        //mesh.uv = uvs;
        //mesh.triangles = triangles;

        //mesh.RecalculateBounds();
        //mesh.Optimize();
    }

    // Update is called once per frame
    void Update() {
        //Debug.DrawLine(Vector3.zero, new Vector3(1, 0, 0), Color.red);
        //Debug.DrawLine(Vector3.zero, new Vector3(0, 1, 0), Color.blue);
    }

    void GenerateMap() {
        MeshFilter filter = transform.FindChild("Wall").GetComponent<MeshFilter>();
        //foreach (Vector3 vert in filter.mesh.vertices) {
        //    Debug.Log(vert, gameObject);
        //}

        Debug.Log("Verts:", gameObject);
        foreach (Vector3 vert in filter.mesh.vertices) {
            Debug.Log(vert, gameObject);
        }

        Debug.Log("UVs:", gameObject);
        foreach (Vector2 uv in filter.mesh.uv) {
            Debug.Log(uv, gameObject);
        }

        Vector3[] vertices = filter.mesh.vertices;
        vertices[0] = new Vector3(0, 0, 0);
        vertices[1] = new Vector3(0, 1, 0);
        vertices[2] = new Vector3(5, 1, 0);
        vertices[3] = new Vector3(5, 0, 0);
        filter.mesh.vertices = vertices;

        Vector2[] newUVs = new Vector2[filter.mesh.vertices.Length];
        newUVs[0] = new Vector2(0, 0);
        newUVs[1] = new Vector2(0, 1);
        newUVs[2] = new Vector2(5, 1);
        newUVs[3] = new Vector2(5, 0);
        filter.mesh.uv = newUVs;

        int[] newTriangles = new int[6];
        newTriangles[0] = 0;
        newTriangles[1] = 1;
        newTriangles[2] = 2;
        newTriangles[3] = 2;
        newTriangles[4] = 3;
        newTriangles[5] = 0;
        filter.mesh.triangles = newTriangles;
    }
}
