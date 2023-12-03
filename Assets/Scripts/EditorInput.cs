using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class EditorInput : MonoBehaviour, GameControls.IEditorActions
{
    GameControls controls;
    [Header("按B关闭编辑器")]
    public UnityEvent OnCloseEditorEvent;
    [Header("鼠标左键单击")]
    public UnityEvent<Vector2> OnLeftButtonEvent;
    public UnityEvent OnLeftButtonPerformedEvent;

    [Header("鼠标左键长按")]
    public UnityEvent<Vector2> OnLeftButtonPressedEvent;
    [Header("鼠标左键松开")]
    public UnityEvent OnLeftButtonCanceledEvent;
    bool isLeftButtonPressed = false;

    public EditorInput(GameControls controls)
    {
        this.controls = controls;
    }

    private void OnEnable()
    {
        if (controls == null)
        {
            controls = new GameControls();
            controls.Editor.SetCallbacks(this);
        }
    }
    public void EnableEditor()
    {
        controls.Editor.Enable();
    }
    public void DisableEditor()
    {
        controls.Editor.Disable();
    }
    private void OnDisable()
    {
        DisableEditor();
    }
    private void Update()
    {
        if (isLeftButtonPressed)
        {
            OnLeftButtonPressedEvent?.Invoke(Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()));
        }
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
            case InputActionPhase.Performed:
                // Debug.Log("Performed");
                OnLeftButtonPerformedEvent?.Invoke();
                break;
            case InputActionPhase.Canceled:
                OnLeftButtonCanceledEvent?.Invoke();
                isLeftButtonPressed = false;
                // Debug.Log("Canceled");
                break;
        }
    }


    public void OnCloseEditor(InputAction.CallbackContext context)
    {
        OnCloseEditorEvent?.Invoke();
        DisableEditor();
    }
}
