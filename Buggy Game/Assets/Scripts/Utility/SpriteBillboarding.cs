using UnityEngine;

public class SpriteBillboarding : MonoBehaviour
{
    private Transform _cameraTransform;

    private void Awake()
    {
        _cameraTransform = GameObject.FindGameObjectWithTag("MainCamera").transform;
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.Euler(0f, _cameraTransform.rotation.eulerAngles.y, 0f);
    }
}