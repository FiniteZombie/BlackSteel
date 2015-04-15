using UnityEngine;
using System.Collections;

public class CombatManager : MonoBehaviour {

    protected class CombatData {
        public Actor playerData;
        public Actor enemyData;

        public CombatData(Actor _playerData, Actor _enemyData) {
            playerData = _playerData;
            enemyData = _enemyData;
        }
    }

    private CombatData combatData;

    public bool isCombatRunning {
        get {
            return combatData != null;
        }
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	    InputUpdate();
	}

    void InputUpdate() {
        if (isCombatRunning) {
            if (Input.GetKeyDown(KeyCode.Escape))
                EndCombat();
            if (Input.GetKeyDown(KeyCode.RightArrow))
                StepCombat();
        }
    }

    public void StartCombat(Actor playerData, Actor enemyData) {
        Debug.Log("Combat Starting");
        combatData = new CombatData(playerData, enemyData);
    }

    public void EndCombat() {
        Debug.Log("Combat Ending");
        combatData = null;
    }

    public void StepCombat() {
        Debug.Log("Combat Stepping");
    }
}
