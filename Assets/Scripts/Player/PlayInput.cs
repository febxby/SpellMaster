using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Events;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
// [CreateAssetMenu(fileName = "PlayerInput", menuName = "ScriptableObjects/PlayerInput")]
public struct SwitchActionMap
{
    public string actionMapName;
}
public class PlayInput : MonoBehaviour, GameControls.IPlayActions
{
    GameControls controls;
    [Header("方向键移动")]
    public UnityEvent<Vector2> OnMoveEvent;
    [Header("空格/W键飞行")]
    public UnityEvent<bool> OnFlyEvent;
    [Header("鼠标左键单击")]
    public UnityEvent<Vector2> OnLeftButtonEvent;

    [Header("鼠标左键长按")]
    public UnityEvent<Vector2> OnLeftButtonPressedEvent;
    [Header("按B打开编辑器")]
    public UnityEvent OnOpenEditorEvent;
    [Header("按E拾取物品")]
    public UnityEvent OnPickUpEvent;
    public UnityEvent OpenSettingEvent;
    bool isLeftButtonPressed = false;
    private void Awake()
    {
        MEventSystem.Instance.Register<SwitchActionMap>(e =>
        {
            switch (e.actionMapName)
            {
                case "Play":
                    {
                        controls.Play.Enable();
                        controls.Editor.Disable();

                    }
                    break;
                case "Editor":
                    {
                        controls.Editor.Enable();
                        controls.Play.Disable();
                    }
                    break;
            }
        }).UnRegisterWhenGameObjectDestroy(gameObject);
    }
    private void OnEnable()
    {
        if (controls == null)
        {
            controls = new GameControls();
            controls.Play.SetCallbacks(this);
        }
        EnablePlay();
    }

    public void EnablePlay()
    {
        Time.timeScale = 1;
        controls.Play.Enable();
    }
    public void DisablePlay()
    {
        controls.Play.Disable();
    }
    private void OnDisable()
    {
        DisablePlay();
    }
    public void OnLeftButton(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Started:
                // Debug.Log("Started");
                isLeftButtonPressed = true;
                OnLeftButtonEvent?.Invoke(Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()));
                break;
            // case InputActionPhase.Performed:
            //     // Debug.Log("Performed");
            //     OnLeftButtonPerformedEvent?.Invoke();
            //     break;
            case InputActionPhase.Canceled:
                isLeftButtonPressed = false;
                // Debug.Log("Canceled");
                break;
        }
    }

    private void Update()
    {
        if (isLeftButtonPressed)
        {
            OnLeftButtonPressedEvent?.Invoke(Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()));
        }
    }
    public void OnMove(InputAction.CallbackContext context)
    {
        OnMoveEvent?.Invoke(context.ReadValue<Vector2>());
    }
    public void OnOpenEditor(InputAction.CallbackContext context)
    {
        OnOpenEditorEvent?.Invoke();
        DisablePlay();
    }
    void GameControls.IPlayActions.OnNum1(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
            MEventSystem.Instance.Send<ChangeCastWand>(new ChangeCastWand { index = 0 });
    }

    void GameControls.IPlayActions.OnNum2(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
            MEventSystem.Instance.Send<ChangeCastWand>(new ChangeCastWand { index = 1 });


    }

    void GameControls.IPlayActions.OnNum3(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
            MEventSystem.Instance.Send<ChangeCastWand>(new ChangeCastWand { index = 2 });


    }

    void GameControls.IPlayActions.OnNum4(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
            MEventSystem.Instance.Send<ChangeCastWand>(new ChangeCastWand { index = 3 });


    }

    public void OnFly(InputAction.CallbackContext context)
    {
        OnFlyEvent?.Invoke(context.ReadValueAsButton());
    }
    public bool isPickUp = false;
    public void OnPickUp(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            OnPickUpEvent?.Invoke();
        }
    }
}
