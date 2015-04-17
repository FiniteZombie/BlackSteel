using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

    public CombatManager combatManager;
    Actor playerData;

	// Use this for initialization
	void Start () {
        CombatActionGraph.GenerateGraph();

        // Init player
        playerData = new Actor();
        playerData.health = 50;
        playerData.type = ActorType.Player;
        playerData.objectReference = GameObject.Find("Player");
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.C)) {
            combatManager.StartCombat(playerData, null);
        }
	}
}
