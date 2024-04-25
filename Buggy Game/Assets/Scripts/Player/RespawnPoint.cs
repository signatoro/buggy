using UnityEngine;

[RequireComponent(typeof(Collider))]
public class RespawnPoint : MonoBehaviour
{
    [Tooltip("The Respawn Point.")] [SerializeField]
    private Transform respawnPoint;

    private PlayerMovement _playerMovement;

    private void Awake()
    {
        _playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
    }

    private void OnTriggerEnter(Collider other)
    {
        _playerMovement.SetRespawnPoint(respawnPoint);
    }
}
