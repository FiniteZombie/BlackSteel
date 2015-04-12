using UnityEngine;
using System.Collections;

public enum ActorType {
    Player,
    Creature,
    Item
}

public class ActorData {
    public int health;
    public ActorType type;
    public GameObject objectReference;
}

public class GameManager : MonoBehaviour {

    public CombatManager combatManager;
    ActorData playerData;

	// Use this for initialization
	void Start () {
        // Init player
        playerData = new ActorData();
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
