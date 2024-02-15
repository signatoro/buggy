using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Test_RaycastToLights : MonoBehaviour
{
    public List<Light> lights = new();
    public float clearanceDistance = 100f;
    public List<float> lightIntensities = new();

    [ContextMenu("Raycast To Lights")]
    public float RaycastToLights()
    {
        lights.Clear();
        lightIntensities.Clear();

        lights = FindObjectsOfType<Light>().ToList();

        foreach (Light light in lights)
        {
            switch (light.type)
            {
                case LightType.Point:
                {
                    float dist = Vector3.Distance(transform.position, light.transform.position);
                    if (light.range >= dist)
                    {
                        int layermask = LayerMask.NameToLayer("Player");
                        Debug.DrawRay(transform.position,
                            (light.transform.position - transform.position).normalized * dist, Color.blue,
                            Mathf.Infinity);
                        RaycastHit hit;
                        if (!Physics.Raycast(transform.position,
                                (light.transform.position - transform.position).normalized, out hit, dist, layermask))
                        {
                            // normalized linear distance, 0.0 at light, 1.0 at range
                            float normDist = dist / light.range;

                            // Unity’s default attention function
                            float atten = Mathf.Clamp01(1.0f / (1.0f + 25.0f * normDist * normDist) *
                                                        ((1.0f - normDist) * 5.0f));
                            Debug.Log($"Atten for light {light.name}: {atten}", this);
                            lightIntensities.Add(light.intensity * atten);
                        }

                        Debug.Log($"Hits for light {light.name}: {hit.transform}", this);
                    }

                    break;
                }
                case LightType.Spot:
                {
                    float dist = Vector3.Distance(transform.position, light.transform.position);
                    if (light.range >= dist)
                    {
                        int layermask = LayerMask.NameToLayer("Player");
                        Debug.DrawRay(transform.position,
                            (light.transform.position - transform.position).normalized * dist, Color.green,
                            Mathf.Infinity);
                        RaycastHit hit;
                        if (!Physics.Raycast(transform.position,
                                (light.transform.position - transform.position).normalized, out hit, dist, layermask))
                        {
                            // normalized linear distance, 0.0 at light, 1.0 at range
                            float normDist = dist / light.range;

                            // Unity’s default attention function
                            float atten = Mathf.Clamp01(1.0f / (1.0f + 25.0f * normDist * normDist) *
                                                        ((1.0f - normDist) * 5.0f));
                            Debug.Log($"Atten for light {light.name}: {atten}", this);

                            float angle = Vector3.Angle(transform.position - light.transform.position,
                                light.transform.forward);

                            // basic approximation of default spotlight cookie
                            float spotFalloff = Mathf.Clamp01((1.0f - (angle / light.spotAngle)) * 5f);
                            Debug.Log($"Spot Falloff for light {light.name}: {spotFalloff}", this);
                            lightIntensities.Add(light.intensity * atten * spotFalloff);
                        }

                        Debug.Log($"Hits for light {light.name}: {hit.transform}", this);
                    }

                    break;
                }
                case LightType.Directional:
                {
                    Vector3 normalizedFromDirectional = light.transform.forward.normalized;
                    int layermask = LayerMask.NameToLayer("Player");
                    Debug.DrawRay(transform.position, normalizedFromDirectional * -1 * clearanceDistance, Color.red,
                        Mathf.Infinity);
                    RaycastHit hit;
                    if (!Physics.Raycast(transform.position, normalizedFromDirectional * -1, out hit, clearanceDistance,
                            layermask))
                    {
                        lightIntensities.Add(light.intensity);
                    }

                    Debug.Log($"Hits for light {light.name}: {hit.transform}", this);

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