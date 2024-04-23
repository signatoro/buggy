using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
[RequireComponent(typeof(RectTransform))]
public class AdjustImageToHeight : MonoBehaviour
{
    #region Variables

    [Tooltip("Height the image should conform to.")] [SerializeField]
    private float height = 150f;

    [Tooltip("The image.")] [SerializeField]
    private Image image;

    [Tooltip("The rectTransform to adjust.")] [SerializeField]
    private RectTransform rectTransform;

    #endregion

    #region Methods

    private void OnValidate()
    {
        AdjustImage();
    }

    private void Update()
    {
        AdjustImage();
    }

    /// <summary>
    /// Adjusts the images size so that the proportions are maintained and the heights match.
    /// </summary>
    private void AdjustImage()
    {
        if (image)
        {
            Vector2 imageVector = image.sprite.rect.size;
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, imageVector.x * height / imageVector.y);
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, imageVector.y * height / imageVector.y);
        }
    }

    #endregion
}