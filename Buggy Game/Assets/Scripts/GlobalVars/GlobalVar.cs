using System;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Events;

public class GlobalVar<T> : ScriptableObject
{
    // Needs to be able to...
    // - Be saved
    // - Be loaded
    // - Store an initial value
    // - Store a current value
    // - Allow the current value to be changed
    // - Allow the current value to be reset to the initial value
    // - Have the current value be evaluated
    // - Have an OnChanged
    // - Keep a stacktrace of when it was changed 

    #region Variables

    /// <summary>
    /// What this variable does.
    /// </summary>
    [Tooltip("What this variable does.")] [TextArea]
    public string Description = "";

    /// <summary>
    /// The initial value of the variable.
    /// </summary>
    [Tooltip("The initial value of the variable.")] [SerializeField]
    private T initialValue;

    /// <summary>
    /// Public get for initial value.
    /// </summary>
    public T InitialValue
    {
        get
        {
            if (BuildStackTrace)
            {
                stackTrace =
                    $"[Frames since start: {Time.frameCount}] Got an Initial Value of {initialValue}.\n StackTrace:\n{Environment.StackTrace}\n\n" +
                    stackTrace;
            }

            return initialValue;
        }
    }

    /// <summary>
    /// The current value of the variable.
    /// </summary>
    [Tooltip("The current value of the variable.")] [SerializeField]
    private T currentValue;

    /// <summary>
    /// The public facing current value.
    /// </summary>
    public T CurrentValue
    {
        get
        {
            if (BuildStackTrace)
            {
                stackTrace =
                    $"[Frames since start: {Time.frameCount}] Got an Current Value of {currentValue}.\n StackTrace:\n{Environment.StackTrace}\n\n" +
                    stackTrace;
            }

            return currentValue;
        }
        set
        {
            if (BuildStackTrace)
            {
                stackTrace =
                    $"[Frames since start: {Time.frameCount}] Current Value is being set to {value} from {currentValue}.\n StackTrace:\n{Environment.StackTrace}\n\n" +
                    stackTrace;
            }

            if (!Equals(currentValue, value))
            {
                OnChanged?.Invoke(value);
            }

            currentValue = value;
        }
    }

    /// <summary>
    /// Should this variable be set to the initial value on start or the loaded value.
    /// </summary>
    [Tooltip("Should this variable be set to the initial value on start or the loaded value.")]
    public bool ResetOnStart = false;

    /// <summary>
    /// Should a stack trace be continually built for this variable as it's being saved/loaded/accessed/changed?
    /// </summary>
    [Tooltip(
        "Should a stack trace be continually built for this variable as it's being saved/loaded/accessed/changed?")]
    public bool BuildStackTrace = true;

    /// <summary>
    /// Event for when the variable's current value has been changed to a different value.
    /// </summary>
    [Tooltip("Event for when the variable's current value has been changed to a different value.")]
    public UnityEvent<T> OnChanged = new UnityEvent<T>();

    /// <summary>
    /// The stacktrace of any time something happened on this variable.
    /// </summary>
    [Tooltip("The stacktrace of any time something happened on this variable.")] [TextArea] [SerializeField] [ReadOnly]
    private string stackTrace = "";

    #endregion

    #region Save/Load

    /// <summary>
    /// Save the current value.
    /// </summary>
    /// <returns>The current value.</returns>
    public T Save()
    {
        if (BuildStackTrace)
        {
            stackTrace =
                $"[Frames since start: {Time.frameCount}] Saved current value of {currentValue}.\n StackTrace:\n{Environment.StackTrace}\n\n" +
                stackTrace;
        }

        return currentValue;
    }

    /// <summary>
    /// Sets the current value to the loaded value.
    /// </summary>
    /// <param name="value">The value being loaded in if there is one.</param>
    public void Load(T value = default)
    {
        if (ResetOnStart) return;
        stackTrace = "";
        if (BuildStackTrace)
        {
            stackTrace =
                $"[Frames since start: {Time.frameCount}] Loading the current value of {value}.\n StackTrace:\n{Environment.StackTrace}\n\n" +
                stackTrace;
        }

        currentValue = value;
    }

    /// <summary>
    /// What to do when the game starts.
    /// </summary>
    private void OnEnable()
    {
        if (!ResetOnStart) return;
        stackTrace = "";
        ResetValue();
    }

    #endregion

    #region Methods

    /// <summary>
    /// Resets the current value to the initial value.
    /// </summary>
    public void ResetValue()
    {
        if (BuildStackTrace)
        {
            stackTrace =
                $"[Frames since start: {Time.frameCount}] Current value is being reset to an initial value of {initialValue}.\n StackTrace:\n{Environment.StackTrace}\n\n" +
                stackTrace;
        }

        currentValue = initialValue;
    }

    #endregion
}