using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Map : MonoBehaviour
{
    public MapGenerator MapGenerator;

    private List<MeshFilter> _wall;

    // Use this for initialization
    void Start()
    {
        _wall = MapGenerator.GenerateMap(this.transform);
    }

    // Update is called once per frame
    void Update()
    {
        if (_wall != null && _wall.Count > 0)
        {
            foreach (var wallMesh in _wall)
            {
                for (int i = 1; i < wallMesh.mesh.vertexCount; i += 2)
                {
                    int next = i + 2;
                    if (next >= wallMesh.mesh.vertexCount)
                        next = 1;
                    Debug.DrawLine(wallMesh.mesh.vertices[i], wallMesh.mesh.vertices[next], Color.red);
                }
            }
        }
    }
}
