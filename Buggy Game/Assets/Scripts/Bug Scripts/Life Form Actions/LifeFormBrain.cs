using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(CatchableLifeForm))]
public class LifeFormBrain : MonoBehaviour
{
    [Serializable]
    private class LifeFormState
    {
        protected bool Equals(LifeFormState other)
        {
            if (lifeFormRelations != other.lifeFormRelations) return false;
            if (!lightIntensityRange.Equals(other.lightIntensityRange)) return false;
            if (!soundIntensityRange.Equals(other.soundIntensityRange)) return false;
            if (wasLost != other.wasLost) return false;
            if (priority != other.priority) return false;
            if (actions.Count != other.actions.Count) return false;

            return !actions.Where((t, i) => t != other.actions[i]).Any();
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((LifeFormState)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine((int)lifeFormRelations, lightIntensityRange, soundIntensityRange, wasLost, priority,
                actions);
        }

        [Tooltip("The Life Form Relations this State Applies to")] [SerializeField]
        private Species.LifeFormRelation lifeFormRelations;

        [Tooltip("Only Enter this State if the Light Intensity Value is between these values")] [SerializeField]
        private Vector2 lightIntensityRange = new(0, Mathf.Infinity);

        [Tooltip("Only Enter this State if the Sound Intensity Value is between these values")] [SerializeField]
        private Vector2 soundIntensityRange = new(0, 100);

        [Tooltip("Are we checking if this Life Form was Lost?")] [SerializeField]
        private bool wasLost;

        [Tooltip("The Priority of this state. Higher numbers take Priority over lower numbers.")] [SerializeField]
        private int priority;

        [Tooltip("The Ordered List of Life Form Actions to Perform. Element 0 is the first action to perform.")]
        [SerializeField]
        private List<LifeFormAction> actions = new();

        /// <summary>
        /// Gets the Priority
        /// </summary>
        /// <returns>The priority number.</returns>
        public int GetPriority() => priority;

        /// <summary>
        /// Gets the Actions.
        /// </summary>
        /// <returns>This State's Actions.</returns>
        public List<LifeFormAction> GetActions() => actions;

        /// <summary>
        /// Does this Life Form have a relationship to the given Life Form that is valid for this State?
        /// </summary>
        /// <param name="thisLifeForm">This Life Form.</param>
        /// <param name="checkedAgainstLifeForm">The Given Life Form.</param>
        /// <returns>True if the relationship is valid, else false.</returns>
        public bool HasLifeFormRelation(CatchableLifeForm thisLifeForm, CatchableLifeForm checkedAgainstLifeForm)
        {
            return (thisLifeForm.Species.GetLifeFormRelation(checkedAgainstLifeForm.Species) & lifeFormRelations) > 0;
        }

        /// <summary>
        /// Can we use this State?
        /// </summary>
        /// <param name="thisLifeForm">This Life Form.</param>
        /// <param name="currentlySeen">The list of currently seen Life Forms.</param>
        /// <param name="noLongerSeen">The list of no longer seen Life Forms.</param>
        /// <param name="currentlyHeard">The list of currently heard Life Forms.</param>
        /// <param name="noLongerHeard">The list of no longer heard Life Forms.</param>
        /// <param name="currentPriority">The priority of the currently running state.</param>
        /// <returns>The Catchable Life Form we can use if we can use this State, else null.</returns>
        public CatchableLifeForm IsStateCurrentlyPossible(CatchableLifeForm thisLifeForm,
            HashSet<SenseSystem.SenseData> currentlySeen,
            HashSet<SenseSystem.SenseData> noLongerSeen,
            HashSet<SenseSystem.SenseData> currentlyHeard,
            HashSet<SenseSystem.SenseData> noLongerHeard,
            int currentPriority = -1)
        {
            if (priority <= currentPriority) return null;

            if (wasLost)
            {
                foreach (SenseSystem.SenseData lost in noLongerSeen)
                {
                    if (HasLifeFormRelation(thisLifeForm, lost.CatchableLifeForm))
                    {
                        if ((lost.Value <= lightIntensityRange.y &&
                             lost.Value >= lightIntensityRange.x) ||
                            (lost.Value <= lightIntensityRange.x &&
                             lost.Value >= lightIntensityRange.y))
                        {
                            return lost.CatchableLifeForm;
                        }
                    }
                }

                foreach (SenseSystem.SenseData lost in noLongerHeard)
                {
                    if (HasLifeFormRelation(thisLifeForm, lost.CatchableLifeForm))
                    {
                        if ((lost.Value <= soundIntensityRange.y &&
                             lost.Value >= soundIntensityRange.x) ||
                            (lost.Value <= soundIntensityRange.x &&
                             lost.Value >= soundIntensityRange.y))
                        {
                            return lost.CatchableLifeForm;
                        }
                    }
                }
            }
            else
            {
                foreach (SenseSystem.SenseData seen in currentlySeen)
                {
                    if (HasLifeFormRelation(thisLifeForm, seen.CatchableLifeForm))
                    {
                        if ((seen.Value <= lightIntensityRange.y &&
                             seen.Value >= lightIntensityRange.x) ||
                            (seen.Value <= lightIntensityRange.x &&
                             seen.Value >= lightIntensityRange.y))
                        {
                            return seen.CatchableLifeForm;
                        }
                    }
                }

                foreach (SenseSystem.SenseData heard in currentlyHeard)
                {
                    if (HasLifeFormRelation(thisLifeForm, heard.CatchableLifeForm))
                    {
                        if ((heard.Value <= soundIntensityRange.y &&
                             heard.Value >= soundIntensityRange.x) ||
                            (heard.Value <= soundIntensityRange.x &&
                             heard.Value >= soundIntensityRange.y))
                        {
                            return heard.CatchableLifeForm;
                        }
                    }
                }
            }

            return null;
        }
    }

    [Tooltip("Sight Sense")] [SerializeField]
    private Sense_Sight senseSight;

    [Tooltip("Sound Sense")] [SerializeField]
    private Sense_Sound senseSound;

    [Tooltip("Default Actions")] [SerializeField]
    private List<LifeFormAction> defaultActions = new();

    [Tooltip("Life Form States")] [SerializeField]
    private List<LifeFormState> lifeFormStates = new();

    private LifeFormState _currentState = null;

    private CatchableLifeForm _lifeFormFollowed = null;

    private HashSet<SenseSystem.SenseData> _currentlySeenLifeForms = new();
    private HashSet<SenseSystem.SenseData> _becameUnseenLastFrame = new();

    private HashSet<SenseSystem.SenseData> _currentlyHeardLifeForms = new();
    private HashSet<SenseSystem.SenseData> _becameUnheardLastFrame = new();

    private CatchableLifeForm _catchableLifeForm;

    private LifeFormState _newLifeFormState = new LifeFormState();

    private void Awake()
    {
        _catchableLifeForm = GetComponent<CatchableLifeForm>();
    }

    private void Start()
    {
        UpdateBrain();
    }

    /// <summary>
    /// Updates the perceived data, sets the state, and starts actions.
    /// </summary>
    private void UpdateBrain()
    {
        // Empty out the current State
        _currentState = null;

        // Handle Incoming Perceived Data
        if (senseSight && senseSight.enabled)
        {
            HandleIncomingSightData(senseSight.CheckFOV());
        }

        if (senseSound && senseSound.enabled)
        {
            HandleIncomingSoundData(senseSound.CheckSoundRadius());
        }

        // Set the State
        SetState();

        Debug.Log($"Current State for {name}: {_currentState}", this);

        // Perform the State's Actions
        StartCoroutine(PerformStateActions());
    }

    /// <summary>
    /// Sets up what data is currently seen and just got lost.
    /// </summary>
    /// <param name="sightDataList">The data that we just saw.</param>
    private void HandleIncomingSightData(List<SenseSystem.SenseData> sightDataList)
    {
        _becameUnseenLastFrame = new HashSet<SenseSystem.SenseData>();
        foreach (SenseSystem.SenseData sightData in _currentlySeenLifeForms.Where(sightData =>
                     !sightDataList.Contains(sightData)))
        {
            _becameUnseenLastFrame.Add(sightData);
        }

        _currentlySeenLifeForms = new HashSet<SenseSystem.SenseData>(sightDataList);
    }

    /// <summary>
    /// Sets up what data is currently heard and just got lost.
    /// </summary>
    /// <param name="soundDataList">The data that we just heard.</param>
    private void HandleIncomingSoundData(List<SenseSystem.SenseData> soundDataList)
    {
        _becameUnheardLastFrame = new HashSet<SenseSystem.SenseData>();
        foreach (SenseSystem.SenseData soundData in _currentlyHeardLifeForms.Where(soundData =>
                     !soundDataList.Contains(soundData)))
        {
            _becameUnheardLastFrame.Add(soundData);
        }

        _currentlyHeardLifeForms = new HashSet<SenseSystem.SenseData>(soundDataList);
    }

    /// <summary>
    /// Sets the current State to the highest possible priority State if we can.
    /// </summary>
    private void SetState()
    {
        // Get the Highest Priority State that we can
        LifeFormState tempState = null;
        CatchableLifeForm catchableLifeForm = null;
        foreach (LifeFormState state in lifeFormStates)
        {
            CatchableLifeForm tempLifeForm = state.IsStateCurrentlyPossible(
                _catchableLifeForm,
                _currentlySeenLifeForms,
                _becameUnseenLastFrame,
                _currentlyHeardLifeForms,
                _becameUnheardLastFrame,
                tempState?.GetPriority() ?? -1);
            catchableLifeForm = tempLifeForm ? tempLifeForm : catchableLifeForm;
            tempState = tempLifeForm ? state : tempState;
        }

        // If we have a new State then we want to set it up
        if (tempState != null)
        {
            _currentState = tempState;
            _lifeFormFollowed = catchableLifeForm;
        }
    }

    /// <summary>
    /// Performs all actions for the current state if we have one, otherwise performs the default actions.
    /// </summary>
    /// <returns>Nothing.</returns>
    private IEnumerator PerformStateActions()
    {
        if (_currentState == null || Equals(_currentState, _newLifeFormState))
        {
            // Wait Until all Default Actions Are Complete
            foreach (LifeFormAction action in defaultActions)
            {
                yield return StartCoroutine(action.PerformAction());
            }
        }
        else
        {
            // Get the Data needed for the State
            SenseSystem.SenseData data =
                ((_currentlySeenLifeForms.FirstOrDefault(senseData =>
                      _lifeFormFollowed == senseData.CatchableLifeForm) ??
                  _currentlyHeardLifeForms.FirstOrDefault(senseData =>
                      _lifeFormFollowed == senseData.CatchableLifeForm)) ?? 
                 _becameUnseenLastFrame.FirstOrDefault(senseData =>
                        _lifeFormFollowed == senseData.CatchableLifeForm)) ?? 
                _becameUnheardLastFrame.FirstOrDefault(senseData =>
                        _lifeFormFollowed == senseData.CatchableLifeForm);

            // Wait Until all Life Form Actions are Complete
            if (data != null)
            {
                foreach (LifeFormAction action in _currentState.GetActions())
                {
                    yield return StartCoroutine(action.PerformAction(data.SensePosition));
                }
            }
        }

        yield return new WaitForEndOfFrame();

        // Now that the actions are done we Update the Brain again
        UpdateBrain();

        yield return null;
    }
}