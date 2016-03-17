using UnityEngine;
using System.Collections;

public class Map : MonoBehaviour
{
    public MapGenerator MapGenerator;

    private MeshFilter _wallMesh;

    // Use this for initialization
    void Start()
    {
        _wallMesh = MapGenerator.GenerateMap();
    }

    // Update is called once per frame
    void Update()
    {
        if (_wallMesh != null)
        {
            for (int i = 1; i < _wallMesh.mesh.vertexCount; i += 2)
            {
                int next = i + 2;
                if (next >= _wallMesh.mesh.vertexCount)
                    next = 1;
                Debug.DrawLine(_wallMesh.mesh.vertices[i], _wallMesh.mesh.vertices[next], Color.red);
            }
        }
    }
}
