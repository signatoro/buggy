using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Sense_Sight : SenseSystem
{
    public class SightData : SenseData
    {
        [Tooltip("The Light Intensity at the Point Seen")]
        public float LightIntensity;

        public SightData(CatchableLifeForm catchableLifeForm, Vector3 sensePosition, float lightIntensity) : base(
            catchableLifeForm, sensePosition)
        {
            LightIntensity = lightIntensity;
        }
    }

    [Tooltip("Field of View Angle")] [SerializeField]
    private GlobalFloat fieldOfViewAngle;

    [Tooltip("How long should the ray be when checking for stuff in the way of a directional light")] [SerializeField]
    private GlobalFloat directionalLightCheckDistance;

    internal override void Update()
    {
        //CheckFOV();
        base.Update();
    }

    /// <summary>
    /// Checks the Life Form's FOV.
    /// </summary>
    /// <returns>Returns the sight data for everything seen.</returns>
    [ContextMenu("Check FOV")]
    private List<SightData> CheckFOV()
    {
        List<SightData> sightDatas = new();
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
                    Vector3 position = lifeForm.transform.position;
                    sightDatas.Add(new SightData(lifeForm, position,
                        GetLightIntensityAtPoint(position)));
                    Debug.Log(
                        $"{name} saw: {lifeForm.name} with a light intensity of {GetLightIntensityAtPoint(position)}",
                        this);
                }
            }
        }

        return sightDatas;
    }

    /// <summary>
    /// Gets the intensity of all lights that reach the point.
    /// </summary>
    /// <param name="point">The point we want to find the intensity at.</param>
    /// <returns>The intensity of the light at the point</returns>
    private float GetLightIntensityAtPoint(Vector3 point)
    {
        List<Light> allEnabledLight = FindObjectsOfType<Light>().ToList();

        List<float> lightIntensities = new();
        int layerMask = LayerMask.NameToLayer("Player") & LayerMask.NameToLayer("LifeForm");
        layerMask = ~layerMask;
        float addToIntensity = 0;

        foreach (Light light in allEnabledLight)
        {
            float dist = Vector3.Distance(point, light.transform.position);
            switch (light.type)
            {
                case LightType.Point:
                {
                    if (light.range >= dist)
                    {
                        if (!Physics.Raycast(point, (light.transform.position - point).normalized, dist, layerMask))
                        {
                            // normalized linear distance, 0.0 at light, 1.0 at range
                            float normDist = dist / light.range;

                            // Unity’s default attention function
                            float atten = Mathf.Clamp01(1.0f / (1.0f + 25.0f * normDist * normDist) *
                                                        ((1.0f - normDist) * 5.0f));
                            addToIntensity = light.intensity * atten;
                            lightIntensities.Add(addToIntensity);
                        }
                    }

                    break;
                }
                case LightType.Spot:
                {
                    if (light.range >= dist)
                    {
                        if (!Physics.Raycast(point, (light.transform.position - point).normalized, dist, layerMask))
                        {
                            // normalized linear distance, 0.0 at light, 1.0 at range
                            float normDist = dist / light.range;

                            // Unity’s default attention function
                            float atten = Mathf.Clamp01(1.0f / (1.0f + 25.0f * normDist * normDist) *
                                                        ((1.0f - normDist) * 5.0f));

                            float angle = Vector3.Angle(point - light.transform.position, light.transform.forward);

                            // basic approximation of default spotlight cookie
                            float spotFalloff = Mathf.Clamp01((1.0f - (angle / light.spotAngle)) * 5f);

                            addToIntensity = light.intensity * atten * spotFalloff;
                            lightIntensities.Add(addToIntensity);
                        }
                    }

                    break;
                }
                case LightType.Directional:
                {
                    Vector3 normalizedFromDirectional = light.transform.forward.normalized;
                    if (!Physics.Raycast(point, normalizedFromDirectional * -1,
                            directionalLightCheckDistance.CurrentValue, layerMask))
                    {
                        addToIntensity = light.intensity;
                        lightIntensities.Add(addToIntensity);
                    }

                    break;
                }
                case LightType.Area:
                case LightType.Disc:
                default:
                    // Not Supported
                    break;
            }
        }

        return lightIntensities.Sum();
    }

    internal override void OnDrawGizmos()
    {
        // Draw FOV
        if (enableGizmos)
        {
            Gizmos.color = Color.yellow;
            float halfFOV = fieldOfViewAngle.CurrentValue / 2.0f;
            Quaternion leftRayRotation = Quaternion.AngleAxis(-halfFOV, Vector3.up);
            Quaternion rightRayRotation = Quaternion.AngleAxis(halfFOV, Vector3.up);
            Vector3 leftRayDirection = leftRayRotation * transform.forward;
            Vector3 rightRayDirection = rightRayRotation * transform.forward;
            Gizmos.DrawRay(root.position, leftRayDirection * radius.CurrentValue);
            Gizmos.DrawRay(root.position, rightRayDirection * radius.CurrentValue);
        }

        base.OnDrawGizmos();
    }
}