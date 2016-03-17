using UnityEngine;
using System.Collections.Generic;

public class PlayerManager : MonoBehaviour {

    enum CommandType {
        MoveForward = 0,
        MoveBack,
        MoveRight,
        MoveLeft,
        RotateRight,
        RotateLeft
    };

    private class CommandState {
        public bool isPressed;
        public bool justPressed;
        public bool justReleased;

        public CommandState() {
            isPressed = false;
            justPressed = false;
            justReleased = false;
        }
    }

    public GameObject mapObject;
    public CombatManager combatManager;

    private Dictionary<CommandType, List<KeyCode>> commandMap;
    private Dictionary<CommandType, CommandState> currentCommandStates;

	// Use this for initialization
	void Start () {
        commandMap = new Dictionary<CommandType,List<KeyCode>>();
        currentCommandStates = new Dictionary<CommandType,CommandState>();

        foreach (CommandType type in System.Enum.GetValues(typeof(CommandType))) {
            currentCommandStates[type] = new CommandState();
        }

        commandMap[CommandType.MoveForward] = new List<KeyCode>(new KeyCode[] { KeyCode.UpArrow, KeyCode.W });
        commandMap[CommandType.MoveBack] = new List<KeyCode>(new KeyCode[] { KeyCode.DownArrow, KeyCode.S });
        commandMap[CommandType.MoveLeft] = new List<KeyCode>(new KeyCode[] { KeyCode.A });
        commandMap[CommandType.MoveRight] = new List<KeyCode>(new KeyCode[] { KeyCode.D });
        commandMap[CommandType.RotateLeft] = new List<KeyCode>(new KeyCode[] { KeyCode.LeftArrow, KeyCode.Q });
        commandMap[CommandType.RotateRight] = new List<KeyCode>(new KeyCode[] { KeyCode.RightArrow, KeyCode.E });
	}

    void UpdateInput() {
        foreach (CommandType type in System.Enum.GetValues(typeof(CommandType))) {
            currentCommandStates[type].isPressed = false;
            currentCommandStates[type].justPressed = false;
            currentCommandStates[type].justReleased = false;

            foreach (KeyCode key in commandMap[type]) {
                if (Input.GetKey(key))
                    currentCommandStates[type].isPressed = true;
                if (Input.GetKeyDown(key))
                    currentCommandStates[type].justPressed = true;
                if (Input.GetKeyUp(key))
                    currentCommandStates[type].justReleased = true;
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
        UpdateInput();

        float step = 2;

        if (!combatManager.isCombatRunning) {
            if (currentCommandStates[CommandType.MoveForward].justPressed) {
                transform.position += step * transform.TransformVector(Vector3.forward);
            }
            if (currentCommandStates[CommandType.MoveBack].justPressed) {
                transform.position += step * transform.TransformVector(Vector3.back);
            }
            if (currentCommandStates[CommandType.MoveLeft].justPressed) {
                transform.position += step * transform.TransformVector(Vector3.left);
            }
            if (currentCommandStates[CommandType.MoveRight].justPressed) {
                transform.position += step * transform.TransformVector(Vector3.right);
            }
            if (currentCommandStates[CommandType.RotateLeft].justPressed) {
                transform.Rotate(Vector3.up, -90);
            }
            if (currentCommandStates[CommandType.RotateRight].justPressed) {
                transform.Rotate(Vector3.up, 90);
            }
        }
	}
}
