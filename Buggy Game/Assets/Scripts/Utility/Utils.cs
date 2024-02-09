using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Utils : MonoBehaviour
{
    /// <summary>
    /// Instance of this class that allows any class to use it.
    /// </summary>
    public static Utils UtilsInstance;

    [Header("Light Detection Values")]
    [Tooltip("How long should the ray be when checking for stuff in the way of a directional light")]
    [SerializeField]
    private GlobalFloat directionalLightCheckDistance;

    private void Awake()
    {
        UtilsInstance = this;
    }

    /// <summary>
    /// Gets the intensity of all lights that reach the point.
    /// </summary>
    /// <param name="point">The point we want to find the intensity at.</param>
    /// <returns>The intensity of the light at the point</returns>
    public float GetLightIntensityAtPoint(Vector3 point)
    {
        List<Light> allEnabledLight = FindObjectsOfType<Light>().ToList();

        List<float> lightIntensities = new();
        int layerMask = LayerMask.NameToLayer("Player") & LayerMask.NameToLayer("LifeForm");
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
}