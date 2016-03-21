using UnityEngine;
using System.Collections.Generic;

public class Player : MonoBehaviour
{
    enum MoveState
    {
        None,
        Step,
        Rotate
    }

    enum CommandType
    {
        MoveForward = 0,
        MoveBack,
        MoveRight,
        MoveLeft,
        RotateRight,
        RotateLeft
    };

    private class CommandState
    {
        public bool isPressed;
        public bool justPressed;
        public bool justReleased;

        public CommandState()
        {
            isPressed = false;
            justPressed = false;
            justReleased = false;
        }
    }

    public GameObject MapObject;
    public CombatManager CombatManager;

    public Vector3 StartingPosition;

    public float Step;
    public float StepTimeInterval;
    public float StepHeight;
    public AnimationCurve StepHeightCurve;

    public float RotateTimeInterval;

    private Dictionary<CommandType, List<KeyCode>> _commandMap;
    private Dictionary<CommandType, CommandState> _currentCommandStates;
    private MoveState _moveState = MoveState.None;

    private Vector3 _startPosition;
    private Vector3 _targetPosition;
    private Vector3 _startDirection;
    private Vector3 _targetDirection;
    private float _startTimeStamp;

    // Use this for initialization
    void Start()
    {
        gameObject.transform.position = StartingPosition;
        _commandMap = new Dictionary<CommandType, List<KeyCode>>();
        _currentCommandStates = new Dictionary<CommandType, CommandState>();

        foreach (CommandType type in System.Enum.GetValues(typeof(CommandType)))
        {
            _currentCommandStates[type] = new CommandState();
        }

        _commandMap[CommandType.MoveForward] = new List<KeyCode>(new KeyCode[] { KeyCode.UpArrow, KeyCode.W });
        _commandMap[CommandType.MoveBack] = new List<KeyCode>(new KeyCode[] { KeyCode.DownArrow, KeyCode.S });
        _commandMap[CommandType.MoveLeft] = new List<KeyCode>(new KeyCode[] { KeyCode.A });
        _commandMap[CommandType.MoveRight] = new List<KeyCode>(new KeyCode[] { KeyCode.D });
        _commandMap[CommandType.RotateLeft] = new List<KeyCode>(new KeyCode[] { KeyCode.LeftArrow, KeyCode.Q });
        _commandMap[CommandType.RotateRight] = new List<KeyCode>(new KeyCode[] { KeyCode.RightArrow, KeyCode.E });
    }

    void UpdateInput()
    {
        foreach (CommandType type in System.Enum.GetValues(typeof(CommandType)))
        {
            _currentCommandStates[type].isPressed = false;
            _currentCommandStates[type].justPressed = false;
            _currentCommandStates[type].justReleased = false;

            foreach (KeyCode key in _commandMap[type])
            {
                if (Input.GetKey(key))
                    _currentCommandStates[type].isPressed = true;
                if (Input.GetKeyDown(key))
                    _currentCommandStates[type].justPressed = true;
                if (Input.GetKeyUp(key))
                    _currentCommandStates[type].justReleased = true;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateInput();

        if (!CombatManager.isCombatRunning)
        {
            if (_moveState == MoveState.None)
            {
                InputMovement();
            }

            UpdateMovement();
        }
    }

    private void InputMovement()
    {
        if (_currentCommandStates[CommandType.MoveForward].isPressed)
        {
            StartStep(transform.position + transform.TransformVector(Step * Vector3.forward));
        }
        if (_currentCommandStates[CommandType.MoveBack].isPressed)
        {
            StartStep(transform.position + transform.TransformVector(Step * Vector3.back));
        }
        if (_currentCommandStates[CommandType.MoveLeft].isPressed)
        {
            StartStep(transform.position + transform.TransformVector(Step * Vector3.left));
        }
        if (_currentCommandStates[CommandType.MoveRight].isPressed)
        {
            StartStep(transform.position + transform.TransformVector(Step * Vector3.right));
        }
        if (_currentCommandStates[CommandType.RotateLeft].isPressed)
        {
            StartRotation(-transform.right);
        }
        if (_currentCommandStates[CommandType.RotateRight].isPressed)
        {
            StartRotation(transform.right);
        }
    }

    private void UpdateMovement()
    {
        switch (_moveState)
        {
            case MoveState.Step:
            {
                UpdateStep();
                break;
            }
            case MoveState.Rotate:
            {
                UpdateRotation();
                break;
            }
        }
    }

    private void StartStep(Vector3 targetPosition)
    {
        _moveState = MoveState.Step;
        _targetPosition = targetPosition;
        _startPosition = transform.position;
        _startTimeStamp = Time.time;
    }

    private void UpdateStep()
    {
        var lerpRatio = (Time.time - _startTimeStamp) / StepTimeInterval;

        if (lerpRatio >= 1f)
        {
            _moveState = MoveState.None;
            transform.position = _targetPosition;
        }

        var heightRatio = StepHeightCurve.Evaluate(lerpRatio);
        var height = heightRatio * StepHeight;

        transform.position = Vector3.Lerp(_startPosition, _targetPosition, lerpRatio);
        transform.position += height * Vector3.up;
    }

    private void StartRotation(Vector3 targetDirection)
    {
        _moveState = MoveState.Rotate;
        _targetDirection = targetDirection;
        _startDirection = transform.forward;
        _startTimeStamp = Time.time;
    }

    private void UpdateRotation()
    {
        var lerpRatio = (Time.time - _startTimeStamp) / RotateTimeInterval;

        if (lerpRatio >= 1f)
        {
            _moveState = MoveState.None;
            transform.forward = _targetDirection;
        }

        transform.forward = Vector3.Slerp(_startDirection, _targetDirection, lerpRatio);
    }
}
