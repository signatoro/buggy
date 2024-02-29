using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class DynamicSpriteSorting : MonoBehaviour
{
    [Serializable]
    private class ParallelLayer
    {
        [Tooltip("The layers that are all the same sorting layer.")]
        public List<SpriteRenderer> layers = new();
    }

    [Tooltip("The distance of a Sprite Renderer when the Sorting Order is -2^16.")] [SerializeField]
    private GlobalFloat maxDistanceForSortingOrder;

    [Tooltip("Sorting layers, 0 is top layer.")] [SerializeField]
    private List<ParallelLayer> layers = new();

    [Tooltip("Root for Distance Calculation.")] [SerializeField]
    private Transform root;

    // Cached Camera Transform used for distance calculations.
    private Transform _cameraTransform;

    // Constants for Sorting Order
    private const Int16 Max = Int16.MaxValue;
    private const Int16 Min = Int16.MinValue;

    private void Update()
    {
        StartCoroutine(SetupSortingOrder());
    }

    /// <summary>
    /// Sets the sorting order for each child Sprite based on the distance to the Camera.
    /// </summary>
    private IEnumerator SetupSortingOrder()
    {
        // Set camera transform
        if (!_cameraTransform)
        {
            _cameraTransform = GameObject.FindGameObjectWithTag("MainCamera").transform;
        }

        // Setup the base Order in Layer to be distance based
        float xValue = Vector3.Distance(_cameraTransform.position, root.position);
        float bValue = Max;
        float mValue = -2f * (bValue / maxDistanceForSortingOrder.CurrentValue);
        float sortingOrderValue = Mathf.Max((mValue * xValue + bValue), Min);
        int baseSortingOrder = (int)sortingOrderValue;

        // Setup the sorting order for the layers
        for (int i = 0; i < layers.Count; i++)
        {
            foreach (SpriteRenderer sprite in layers[i].layers)
            {
                sprite.sortingOrder = baseSortingOrder - i;
            }
        }

        yield return null;
    }
}