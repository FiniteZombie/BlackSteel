using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Pair<X, Y> {
    private X _x;
    private Y _y;

    public Pair(X first, Y second) {
        _x = first;
        _y = second;
    }

    public X first { get { return _x; } }

    public Y second { get { return _y; } }

    public override bool Equals(object obj) {
        if (obj == null)
            return false;
        if (obj == this)
            return true;
        Pair<X, Y> other = obj as Pair<X, Y>;
        if (other == null)
            return false;

        return
            (((first == null) && (other.first == null))
                || ((first != null) && first.Equals(other.first)))
                &&
            (((second == null) && (other.second == null))
                || ((second != null) && second.Equals(other.second)));
    }

    public override int GetHashCode() {
        int hashcode = 0;
        if (first != null)
            hashcode += first.GetHashCode();
        if (second != null)
            hashcode += second.GetHashCode();

        return hashcode;
    }

    public override string ToString() {
        return "<" + _x + ", " + _y + ">";
    }
}

public class Edge<X, Y> : Pair<X, Y> {
    private X _x;
    private Y _y;

    public Edge(X first, Y second)
        : base(first, second) { }

    public override bool Equals(object obj) {
        if (obj == null)
            return false;
        if (obj == this)
            return true;
        Edge<X, Y> other = obj as Edge<X, Y>;
        if (other == null)
            return false;

        return
            (
                (((first == null) && (other.first == null))
                    || ((first != null) && first.Equals(other.first)))
                  &&
                (((second == null) && (other.second == null))
                    || ((second != null) && second.Equals(other.second)))
            )
            ||
            (
                (((first == null) && (other.second == null))
                    || ((first != null) && first.Equals(other.second)))
                  &&
                (((second == null) && (other.first == null))
                    || ((second != null) && second.Equals(other.first)))
            );
    }
}

public class Edge2 : Edge<Vector2, Vector2> {
    public Edge2(Vector2 first, Vector2 second)
        : base(first, second) { }
}

public class Edge3 : Edge<Vector3, Vector3> {
    public Edge3(Vector3 first, Vector3 second)
        : base(first, second) { }
}

public class MapGenerator : MonoBehaviour {
    public float wallHeight;
    public float wallWidth;
    MeshFilter filter;

    void Awake() {
    }

	// Use this for initialization
    void Start() {
	}
	
	// Update is called once per frame
    void Update() {
        for (int i = 1; i < filter.mesh.vertexCount; i += 2) {
            int next = i + 2;
            if (next >= filter.mesh.vertexCount)
                next = 1;
            Debug.DrawLine(filter.mesh.vertices[i], filter.mesh.vertices[next], Color.red);
        }

    }

    /*
     * Map Generation
     * --------------
     * 
     *       -----
     * quad= | / |
     *       -----
     * path index:      0   1   2   3   0
     *      
     * vertex index:    1   3   5   7   1
     *                  -----------------
     * triangles:       | / | / | / | / |
     *                  -----------------
     * vertex index:    0   2   4   6   0
    */
    public void GenerateMap() {
        filter = transform.FindChild("Wall").GetComponent<MeshFilter>();
        List<List<Vector2>> wallIslands = GenerateEdgeIslands();
        //Debug.Log(edgeIslandsToString(wallIslands));
        Debug.Log("Map generated.");

        filter.mesh.vertices = GenerateVertices(wallIslands);
        filter.mesh.uv = GenerateUVs(wallIslands);
        filter.mesh.triangles = GenerateTriangles(wallIslands);

        filter.mesh.RecalculateBounds();
        filter.mesh.Optimize();
        filter.mesh.RecalculateNormals();
    }

    private List<List<Vector2>> GenerateEdgeIslands() {
        return LoadMap("Default");
    }

    private Vector3[] GenerateVertices(List<List<Vector2>> wallIslands) {
        int numVerts = 0;
        foreach (List<Vector2> wallPath in wallIslands) {
            numVerts += 2 * wallPath.Count;
        }

        Vector3[] vertices = new Vector3[numVerts];

        int i = 0;
        foreach (List<Vector2> wallPath in wallIslands) {
            foreach (Vector2 point in wallPath) {
                vertices[i++] = new Vector3(wallWidth * point.x, 0, wallWidth * point.y);
                vertices[i++] = new Vector3(wallWidth * point.x, wallHeight, wallWidth * point.y);
            }
        }

        return vertices;
    }

    private int[] GenerateTriangles(List<List<Vector2>> wallIslands) {
        int numTriangles = 0;
        foreach (List<Vector2> wallPath in wallIslands) {
            numTriangles += 2 * (wallPath.Count - 1);
        }

        int[] triangles = new int[3 * numTriangles];
        int triIndex = 0;
        foreach (List<Vector2> wallPath in wallIslands) {
            for (int pathIndex = 0; pathIndex < (wallPath.Count - 1); pathIndex++) {
                int vertIndex = 2 * pathIndex;

                triangles[triIndex++] = vertIndex + 3; // 1 ---- 0
                triangles[triIndex++] = vertIndex + 1; //   | / 
                triangles[triIndex++] = vertIndex;     // 2 -

                triangles[triIndex++] = vertIndex;     //      - 5
                triangles[triIndex++] = vertIndex + 2; //    / |
                triangles[triIndex++] = vertIndex + 3; // 3 ---- 4
            }
        }

        return triangles;
    }

    private Vector2[] GenerateUVs(List<List<Vector2>> wallIslands) {
        int numVerts = 0;
        foreach (List<Vector2> wallPath in wallIslands) {
            numVerts += 2 * wallPath.Count;
        }

        Vector2[] UVs = new Vector2[numVerts];
        int i = 0;
        foreach (List<Vector2> island in wallIslands) {
            for (int j = 0; j < 2 * island.Count; j++) {
                UVs[i++] = new Vector2(j / 2, j % 2);
            }
        }
        return UVs;
    }

    private List<List<Vector2>> LoadMap(string mapName) {
        TextAsset mapTextAsset = Resources.Load(mapName) as TextAsset;
        string[] mapLines = mapTextAsset.text.Split('\n');
        string[] dimensions = mapLines[0].Split('x');
        int mapWidth = int.Parse(dimensions[0]);
        int mapHeight = int.Parse(dimensions[1]);

        // Create a hash set of all edges. This is a quick way to get a set of all the edges
        // and ignore all duplicates in one swing.
        HashSet<Edge2> edgeSet = new HashSet<Edge2>();

        // Start with adding the edges which make up the border
        // North and South
        for (int i = 0; i < mapWidth; i++) {
            Vector2 a = new Vector2(i, 0);
            Vector2 b = new Vector2(i + 1, 0);
            edgeSet.Add(new Edge2(a, b));

            a = new Vector2(i, mapHeight);
            b = new Vector2(i + 1, mapHeight);
            edgeSet.Add(new Edge2(a, b));
        }

        // West and East
        for (int i = 0; i < mapHeight; i++) {
            Vector2 a = new Vector2(0, i);
            Vector2 b = new Vector2(0, i + 1);
            edgeSet.Add(new Edge2(a, b));

            a = new Vector2(mapWidth, i);
            b = new Vector2(mapWidth, i + 1);
            edgeSet.Add(new Edge2(a, b));
        }

        // Get edges from all wall blocks
        for (int y = 0; y < mapHeight; y++) {
            int lineNum = mapHeight - y;
            for (int x = 0; x < mapWidth; x++) {
                if (mapLines[lineNum][x] == 'w') {
                    // North edge
                    Vector2 a = new Vector2(x, y + 1);
                    Vector2 b = new Vector2(x + 1, y + 1);
                    Edge2 edge = new Edge2(a, b);
                    if (!edgeSet.Remove(edge))
                        edgeSet.Add(edge);

                    // South edge
                    a = new Vector2(x, y);
                    b = new Vector2(x + 1, y);
                    edge = new Edge2(a, b);
                    if (!edgeSet.Remove(edge))
                        edgeSet.Add(edge);

                    // West edge
                    a = new Vector2(x, y);
                    b = new Vector2(x, y + 1);
                    edge = new Edge2(a, b);
                    if (!edgeSet.Remove(edge))
                        edgeSet.Add(edge);

                    // East edge
                    a = new Vector2(x + 1, y);
                    b = new Vector2(x + 1, y + 1);
                    edge = new Edge2(a, b);
                    if (!edgeSet.Remove(edge))
                        edgeSet.Add(edge);
                }
            }
        }

        // Run though edge set and get individual edge islands
        List<List<Vector2>> edgeIslands = new List<List<Vector2>>();
        while (edgeSet.Count > 0) {
            List<Vector2> wallPath = new List<Vector2>();

            IEnumerator<Edge2> en = edgeSet.GetEnumerator();
            en.MoveNext();
            Edge2 start = en.Current;
            Edge2 current = start;
            wallPath.Add(current.first);
            edgeSet.Remove(current);

            while (current.second != start.first) {
                wallPath.Add(current.second);

                Edge2 oldCurrent = new Edge2(current.first, current.second);
                foreach (Edge2 edge in edgeSet) {
                    if (edge == start)
                        continue;

                    if (edge.first == current.second) {
                        current = edge;
                        break;
                    }
                    else if (edge.second == current.second) {
                        current = new Edge2(edge.second, edge.first);
                        break;
                    }
                }

                if (current == oldCurrent)
                    throw new System.Exception("Infinite loop while iterating through edges.");

                edgeSet.Remove(current);
            }

            wallPath.Add(start.first);
            edgeIslands.Add(wallPath);
        }

        return edgeIslands;
    }

    private string edgeIslandsToString(List<List<Vector2>> edgeIslands) {
        string str = "";

        int count = 0;
        foreach (List<Vector2> island in edgeIslands) {
            str += "Island: " + count + "\n";
            int pointCount = 0;
            foreach (Vector2 point in island) {
                str += point + ", ";
                pointCount++;
                if (pointCount % 10 == 0)
                    str += "\n";
            }
            str += "\n\n";
            count++;
        }

        return str;
    }
}
