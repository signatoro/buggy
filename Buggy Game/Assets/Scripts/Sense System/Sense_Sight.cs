using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Sense_Sight : SenseSystem
{
    [Tooltip("Field of View Angle")] [SerializeField]
    private GlobalFloat fieldOfViewAngle;

    [Tooltip("Does this Life Form care about light levels")] [SerializeField]
    private GlobalBool useLightIntensity;

    internal override void Update()
    {
        //CheckFOV();
        base.Update();
    }

    [ContextMenu("Check FOV")]
    private List<CatchableLifeForm> CheckFOV()
    {
        List<CatchableLifeForm> seenValidLifeForms = new();
        int layerMask = LayerMask.NameToLayer("Player") & LayerMask.NameToLayer("LifeForm");
        layerMask = ~layerMask;
        List<Collider> hitColliders = Physics.OverlapSphere(root.position, radius.CurrentValue, layerMask).ToList();

        foreach (Collider collider in hitColliders)
        {
            if (collider.GetComponent<CatchableLifeForm>() &&
                collider.GetComponent<CatchableLifeForm>() != GetComponent<CatchableLifeForm>())
            {
                CatchableLifeForm lifeForm = collider.GetComponent<CatchableLifeForm>();
                Vector3 directionToLifeForm = lifeForm.transform.position - root.position;
                if (_catchableLifeForm.Species.HasConnectionToSpecies(lifeForm.Species) &&
                    Physics.Raycast(root.position, directionToLifeForm, out RaycastHit hit, radius.CurrentValue) &&
                    hit.transform.GetComponent<CatchableLifeForm>() &&
                    hit.transform.GetComponent<CatchableLifeForm>() == lifeForm &&
                    Vector3.Angle(directionToLifeForm, root.forward) <= fieldOfViewAngle.CurrentValue)
                {
                    seenValidLifeForms.Add(lifeForm);
                    Debug.Log($"{name} saw: {lifeForm.name}", this);
                }
            }
        }

        return seenValidLifeForms;
    }

    internal override void OnDrawGizmos()
    {
        if (enableGizmos)
        {
        }

        base.OnDrawGizmos();
    }
}