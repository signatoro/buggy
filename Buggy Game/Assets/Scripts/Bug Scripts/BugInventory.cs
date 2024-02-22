using UnityEngine;

/// <summary>
/// Required to be on player.
/// </summary>
[RequireComponent(typeof(PlayerMovement))]
public class BugInventory : MonoBehaviour
{
    [Tooltip("Bug Inventory Data.")] [SerializeField]
    private BugInventoryData data;

    /// <summary>
    /// Gets the Bug Inventory Data.
    /// </summary>
    /// <returns>The Bug Inventory Data</returns>
    public BugInventoryData GetBugInventoryData() => data;
}