using UnityEngine;
using System.Collections;
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

    Dictionary<CommandType, List<KeyCode>> commandMap;
    Dictionary<CommandType, CommandState> currentCommandStates;

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

        if (currentCommandStates[CommandType.MoveForward].justPressed) {
            transform.position += 2 * transform.TransformVector(Vector3.forward);
        }
        if (currentCommandStates[CommandType.MoveBack].justPressed) {
            transform.position -= 2 * transform.TransformVector(Vector3.forward);
        }
        if (currentCommandStates[CommandType.RotateLeft].justPressed) {
            transform.Rotate(Vector3.up, -90);
        }
        if (currentCommandStates[CommandType.RotateRight].justPressed) {
            transform.Rotate(Vector3.up, 90);
        }
	}
}
