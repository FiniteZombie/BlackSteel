using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapManager : MonoBehaviour {
    public float wallHeight = 1;
    public float wallWidth = 2;

    private MapGenerator mapGenerator;

    // Use this for prep
    void Wake() {
    }

    // Use this for initialization
    void Start() {
        mapGenerator = GetComponent<MapGenerator>();
        mapGenerator.wallHeight = wallHeight;
        mapGenerator.wallWidth = wallWidth;
        mapGenerator.GenerateMap();
    }

    // Update is called once per frame
    void Update() {
    }
}
