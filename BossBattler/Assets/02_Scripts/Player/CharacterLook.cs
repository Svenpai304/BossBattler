using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterLook : MonoBehaviour
{
    public float MouseSensitivity;
    public float StickLookDeadzone;

    [SerializeField] private GameObject keyboardLookReticle;
    [SerializeField] private GameObject gamepadLookArrow;
    public float gamepadArrowOffset;
    private GameObject lookObject;
    private Transform gamepadArrow;
    private bool isKeyboard = false;
    public Vector2 LookDirection;

    public void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        PlayerInput playerInput = GetComponent<PlayerInput>();
        if (playerInput != null && playerInput.devices.Contains(Keyboard.current))
        {
            isKeyboard = true;
            lookObject = Instantiate(keyboardLookReticle, Camera.main.transform);
            CursorSpriteController cursor = lookObject.GetComponent<CursorSpriteController>();
            if (cursor != null) cursor.Setup(GetComponent<SciencePack>());
            KeyboardLook(transform.position);
        }
        else
        {
            Debug.Log("Non-keyboard player added");
            lookObject = Instantiate(gamepadLookArrow, transform);
            gamepadArrow = lookObject.GetComponentsInChildren<Transform>()[1];
            GamepadLook(Vector2.right);
        }
    }

    private void Update()
    {
        if (isKeyboard)
        {
            LookDirection = (Vector2)(lookObject.transform.position - transform.position).normalized;
        }
        else
        {
            lookObject.transform.localPosition = (Vector3)(LookDirection * gamepadArrowOffset);
        }
    }

    public void OnLook(InputAction.CallbackContext c)
    {
        Vector2 input = c.ReadValue<Vector2>();
        if (isKeyboard) { KeyboardLook(input); }
        else { GamepadLook(input); }
    }

    private void KeyboardLook(Vector2 pos)
    {
        lookObject.transform.localPosition += (Vector3)(pos * MouseSensitivity);
        if (Mathf.Abs(lookObject.transform.localPosition.x) > Camera.main.orthographicSize / Display.main.renderingHeight * Display.main.renderingWidth)
        {
            lookObject.transform.localPosition = new Vector3(
                Camera.main.orthographicSize / Display.main.renderingHeight * Display.main.renderingWidth * Mathf.Sign(lookObject.transform.localPosition.x),
                lookObject.transform.localPosition.y, 10);
        }
        if (Mathf.Abs(lookObject.transform.localPosition.y) > Camera.main.orthographicSize)
        {
            lookObject.transform.localPosition = new Vector3(lookObject.transform.localPosition.x, Camera.main.orthographicSize * Mathf.Sign(lookObject.transform.localPosition.y), 10);
        }
        LookDirection = (Vector2)(lookObject.transform.position - transform.position).normalized;
    }

    public void GamepadLook(Vector2 dir)
    {
        if (dir.sqrMagnitude < Mathf.Pow(StickLookDeadzone, 2)) { return; }

        dir = dir.normalized;
        lookObject.transform.localPosition = (Vector3)(dir * gamepadArrowOffset);
        Debug.Log(Vector2.SignedAngle(Vector2.zero, dir));
        gamepadArrow.rotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.up, dir));
        LookDirection = dir;
    }
}
