using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.MapUtil;

public class MapGenerator : MonoBehaviour
{
    public float WallHeight;
    public float WallWidth;
    public MeshFilter WallPrefab;

    public List<MeshFilter> GenerateMap(Transform parent)
    {
        var walls = new List<MeshFilter>();
        List<List<Vector2>> wallIslands = GenerateEdgeIslands(LoadMap("Default"));

        foreach (var island in wallIslands)
        {
            var rotateInc = 0;

            for (var i = 0; i < island.Count - 1; i++)
            {
                var currentPoint = island[i];
                var nextPoint = (i + 1) >= island.Count
                    ? island[0]
                    : island[i + 1];

                var currentWorldPoint = new Vector3(WallWidth * currentPoint.x, .5f * WallHeight, WallWidth * currentPoint.y);
                var nextWorldPoint = new Vector3(WallWidth * nextPoint.x, .5f * WallHeight, WallWidth * nextPoint.y);

                Debug.DrawLine(currentWorldPoint + Vector3.up, nextWorldPoint + Vector3.up, Color.yellow, 20f);

                var worldMidpoint = Vector3.Lerp(currentWorldPoint, nextWorldPoint, .5f);
                var worldDirection = nextWorldPoint - currentWorldPoint;
                var wallForward = Vector3.Cross(Vector3.up, worldDirection.normalized);
                
                var wall = Instantiate(WallPrefab);
                wall.transform.SetParent(parent);
                wall.transform.localPosition = worldMidpoint;
                wall.transform.forward = wallForward;
                wall.transform.localScale = new Vector3(WallWidth, WallHeight, 1);

                var rotateMult = rotateInc % 2;
                rotateInc++;

                wall.transform.RotateAround(wall.transform.position, wall.transform.forward, rotateMult * 180f);

                walls.Add(wall);
            }
        }

        return walls;
    }

    /* 
     * Old Map Generation
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
    public MeshFilter OldGenerateMap()
    {
        var wallMeshFilter = Instantiate(WallPrefab);

        List<List<Vector2>> wallIslands = GenerateEdgeIslands(LoadMap("Default"));
        //Debug.Log(edgeIslandsToString(wallIslands));
        Debug.Log("Map generated.");

        wallMeshFilter.mesh.vertices = GenerateVertices(wallIslands);
        wallMeshFilter.mesh.uv = GenerateUVs(wallIslands);
        wallMeshFilter.mesh.triangles = GenerateTriangles(wallIslands);

        wallMeshFilter.mesh.RecalculateBounds();
        wallMeshFilter.mesh.Optimize();
        wallMeshFilter.mesh.RecalculateNormals();
        return wallMeshFilter;
    }

    private List<List<Vector2>> GenerateEdgeIslands(HashSet<Edge2> edgeSet)
    {
        // Run though edge set and get individual edge islands
        List<List<Vector2>> edgeIslands = new List<List<Vector2>>();
        while (edgeSet.Count > 0)
        {
            List<Vector2> wallPath = new List<Vector2>();

            IEnumerator<Edge2> en = edgeSet.GetEnumerator();
            en.MoveNext();
            Edge2 start = en.Current;
            Edge2 current = start;
            wallPath.Add(current.first);
            edgeSet.Remove(current);

            while (current.second != start.first)
            {
                wallPath.Add(current.second);

                Edge2 oldCurrent = new Edge2(current.first, current.second);
                foreach (Edge2 edge in edgeSet)
                {
                    if (edge == start)
                        continue;

                    if (edge.first == current.second)
                    {
                        current = edge;
                        break;
                    }
                    else if (edge.second == current.second)
                    {
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

    private Vector3[] GenerateVertices(List<List<Vector2>> wallIslands)
    {
        int numVerts = 0;
        foreach (List<Vector2> wallPath in wallIslands)
        {
            numVerts += 2 * wallPath.Count;
        }

        Vector3[] vertices = new Vector3[numVerts];

        int i = 0;
        foreach (List<Vector2> wallPath in wallIslands)
        {
            foreach (Vector2 point in wallPath)
            {
                vertices[i++] = new Vector3(WallWidth * point.x, 0, WallWidth * point.y);
                vertices[i++] = new Vector3(WallWidth * point.x, WallHeight, WallWidth * point.y);
            }
        }

        return vertices;
    }

    private int[] GenerateTriangles(List<List<Vector2>> wallIslands)
    {
        int numTriangles = 0;
        foreach (List<Vector2> wallPath in wallIslands)
        {
            numTriangles += 2 * (wallPath.Count - 1);
        }

        int[] triangles = new int[3 * numTriangles];
        int triIndex = 0;
        foreach (List<Vector2> wallPath in wallIslands)
        {
            for (int pathIndex = 0; pathIndex < (wallPath.Count - 1); pathIndex++)
            {
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

    private Vector2[] GenerateUVs(List<List<Vector2>> wallIslands)
    {
        int numVerts = 0;
        foreach (List<Vector2> wallPath in wallIslands)
        {
            numVerts += 2 * wallPath.Count;
        }

        Vector2[] UVs = new Vector2[numVerts];
        int i = 0;
        foreach (List<Vector2> island in wallIslands)
        {
            for (int j = 0; j < 2 * island.Count; j++)
            {
                UVs[i++] = new Vector2(j / 2, j % 2);
            }
        }
        return UVs;
    }

    private HashSet<Edge2> LoadMap(string mapName)
    {
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
        for (int i = 0; i < mapWidth; i++)
        {
            Vector2 a = new Vector2(i, 0);
            Vector2 b = new Vector2(i + 1, 0);
            edgeSet.Add(new Edge2(a, b));

            a = new Vector2(i, mapHeight);
            b = new Vector2(i + 1, mapHeight);
            edgeSet.Add(new Edge2(a, b));
        }

        // West and East
        for (int i = 0; i < mapHeight; i++)
        {
            Vector2 a = new Vector2(0, i);
            Vector2 b = new Vector2(0, i + 1);
            edgeSet.Add(new Edge2(a, b));

            a = new Vector2(mapWidth, i);
            b = new Vector2(mapWidth, i + 1);
            edgeSet.Add(new Edge2(a, b));
        }

        // Get edges from all wall blocks
        for (int y = 0; y < mapHeight; y++)
        {
            int lineNum = mapHeight - y;
            for (int x = 0; x < mapWidth; x++)
            {
                if (mapLines[lineNum][x] == 'w')
                {
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

        return edgeSet;
    }

    private string edgeIslandsToString(List<List<Vector2>> edgeIslands)
    {
        string str = "";

        int count = 0;
        foreach (List<Vector2> island in edgeIslands)
        {
            str += "Island: " + count + "\n";
            int pointCount = 0;
            foreach (Vector2 point in island)
            {
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
