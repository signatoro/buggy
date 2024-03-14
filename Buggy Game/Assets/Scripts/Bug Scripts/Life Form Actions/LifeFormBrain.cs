using System;
using System.Collections.Generic;
using UnityEngine;

public class LifeFormBrain : MonoBehaviour
{
    [Serializable]
    private class LifeFormState
    {
        [Flags]
        internal enum LifeFormRelation
        {
            PREDATOR,
            PREY,
            NEUTRAL,
            ENEMY,
        }

        [Tooltip("The Life Form Relations this State Applies to")]
        public LifeFormRelation LifeFormRelations;

        [Tooltip("Only Enter this State if the Light Intensity Value is between these values")]
        public Vector2 LightIntensityRange = new Vector2(0, Mathf.Infinity);
        
        [Tooltip("Only Enter this State if the Sound Intensity Value is between these values")]
        public Vector2 SoundIntensityRange = new Vector2(0, 100);

        [Tooltip("Are we checking if this Life Form was Lost?")]
        public bool WasLost;
        
        [Tooltip("Should this State Loop?")]
        public bool ShouldLoop;

        [Tooltip("The Priority of this state. Higher numbers take Priority over lower numbers.")]
        public int Priority;

        [Tooltip("Should we Wait for all the Life Form Actions in this State to be Completed before we reevaluate?")]
        public bool WaitForCompletion;

        [Tooltip("The Ordered List of Life Form Actions to Perform. Element 0 is the first action to perform.")]
        public List<LifeFormAction> Actions;
    }
    
    [Tooltip("Sight Sense")] [SerializeField]
    private Sense_Sight senseSight;
    
    [Tooltip("Sound Sense")] [SerializeField]
    private Sense_Sound senseSound;

    [Tooltip("Default Actions")] [SerializeField]
    private List<LifeFormAction> defaultActions = new();

    [Tooltip("Life Form States")] [SerializeField]
    private List<LifeFormState> lifeFormStates = new();

    private HashSet<CatchableLifeForm> currentlySeenLifeForms = new();

    private void HandleIncomingSightData(Sense_Sight.SightData sightData)
    {
        
    }
}
