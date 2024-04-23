using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/**
 * A Book needs to...
 * - Display 2 pages at a time
 * - Be able to go to the next page
 * - Have one way arrows when there are no pages left
 * - Have a blank page at the end if there are an odd number of pages available
 * - Only show mystery pages if the unlock condition has been met
 * - Show full page if the species has been caught
 * - Disable movement, camera, and other interaction when opened
 */
public class BookUI : MonoBehaviour
{
    /// <summary>
    /// The data related to displaying a Species.
    /// </summary>
    [Serializable]
    private class SpeciesPageData
    {
        [Tooltip("The Species that this page is about.")] [SerializeField]
        private Species species;

        [Tooltip("The Species that needs to be caught for the mystery version of the page to display")] [SerializeField]
        private Species unlockSpecies;

        /// <summary>
        /// Has this species been caught?
        /// </summary>
        /// <param name="data">The Bug Inventory we are using.</param>
        /// <returns>True, if we have caught the Species, else false.</returns>
        private bool IsSpeciesCaught(BugInventoryData data) => data.HasCaught(species);

        /// <summary>
        /// Should the mystery page for this species be displayed?
        /// </summary>
        /// <param name="data">The Bug Inventory we are using.</param>
        /// <returns>True, if the species is a mystery, else false.</returns>
        private bool IsSpeciesAMystery(BugInventoryData data) => (!unlockSpecies ||
                                                                  data.HasCaught(unlockSpecies)) &&
                                                                 !IsSpeciesCaught(data);

        /// <summary>
        /// Returns the page layout if there is one.
        /// <param name="data">The Bug Inventory we are using.</param>
        /// </summary>
        /// <returns>The page layout, or null if there is no layout.</returns>
        public SpeciesPageLayout GetPageLayout(BugInventoryData data)
        {
            if (IsSpeciesCaught(data))
            {
                return new SpeciesPageLayout(species.GetSpeciesName(), species.GetFullSprite(),
                    species.GetPostCaughtDescription());
            }

            if (IsSpeciesAMystery(data))
            {
                return new SpeciesPageLayout("???", species.GetSilhouette(),
                    species.GetPreCaughtDescription());
            }

            return null;
        }
    }

    /// <summary>
    /// The display layout for a Species.
    /// </summary>
    private class SpeciesPageLayout
    {
        public string Name { get; private set; }

        public Sprite Sprite { get; private set; }

        public string Description { get; private set; }

        /// <summary>
        /// Constructor for a Species Page Layout
        /// </summary>
        /// <param name="name">The known name of the Species.</param>
        /// <param name="sprite">The known look of the Species</param>
        /// <param name="description">The known information of the Species.</param>
        public SpeciesPageLayout(string name, Sprite sprite, string description)
        {
            Name = name;
            Sprite = sprite;
            Description = description;
        }
    }


    #region Variables

    [Tooltip("List of Species Page Data.")] [SerializeField]
    private List<SpeciesPageData> speciesPageDataList = new();

    [Tooltip("Left Name.")] [SerializeField]
    private TextMeshProUGUI leftName;

    [Tooltip("Right Name.")] [SerializeField]
    private TextMeshProUGUI rightName;

    [Tooltip("Left Description.")] [SerializeField]
    private TextMeshProUGUI leftDescription;

    [Tooltip("Right Description.")] [SerializeField]
    private TextMeshProUGUI rightDescription;

    [Tooltip("Left Image.")] [SerializeField]
    private Image leftImage;

    [Tooltip("Right Image.")] [SerializeField]
    private Image rightImage;

    [Tooltip("Left Button.")] [SerializeField]
    private Button leftButton;

    [Tooltip("Right Button.")] [SerializeField]
    private Button rightButton;

    [Tooltip("Fader.")] [SerializeField] private Image fader;

    [Tooltip("Player Movement.")] [SerializeField]
    private GlobalBool playerMovement;

    [Tooltip("Camera Controller.")] [SerializeField]
    private GlobalBool cameraController;

    [Tooltip("Fade Color.")] [SerializeField]
    private GlobalColor fadeColor;

    [Tooltip("Fade Time.")] [SerializeField]
    private GlobalFloat fadeTime;

    private List<SpeciesPageLayout> _currentPageLayouts = new();

    private int _currentLeftPageIndex;

    private GameObject _player;

    private BugInventoryData _bugInventoryData;

    #endregion

    #region Methods

    void Awake()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _bugInventoryData = _player.GetComponent<BugInventory>().GetBugInventoryData();

        UpdatePageLayouts();
    }

    private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        DisplayBook();
    }

    /// <summary>
    /// Updates the layout of the pages that can be displayed.
    /// </summary>
    [ContextMenu("Update Page Layouts")]
    public void UpdatePageLayouts()
    {
        _currentPageLayouts.Clear();
        foreach (SpeciesPageLayout layout in speciesPageDataList
                     .Select(speciesPageData => speciesPageData.GetPageLayout(_bugInventoryData))
                     .Where(layout => layout != null))
        {
            _currentPageLayouts.Add(layout);
        }
    }

    /// <summary>
    /// Displays the beginning of the book, disables movement, and fades to a transparent black.
    /// </summary>
    private void DisplayBook()
    {
        StartCoroutine(FadeToBlack());
        playerMovement.CurrentValue = false;
        cameraController.CurrentValue = true;
        _currentLeftPageIndex = 0;
        DisplayPages();
    }

    /// <summary>
    /// Fades to Black.
    /// </summary>
    /// <returns>Nothing.</returns>
    private IEnumerator FadeToBlack()
    {
        float timer = 0;

        while (timer < fadeTime.CurrentValue)
        {
            Color color = fader.color;
            color.a = Mathf.Lerp(0, fadeColor.CurrentValue.a, timer / fadeTime.CurrentValue);
            fader.color = color;
            timer += Time.deltaTime;
            yield return null;
        }

        yield return null;
    }

    /// <summary>
    /// Shows the next 2 pages.
    /// </summary>
    public void FlipRight()
    {
        _currentLeftPageIndex += 2;
        DisplayPages();
    }

    /// <summary>
    /// Shows the previous 2 pages.
    /// </summary>
    public void FlipLeft()
    {
        _currentLeftPageIndex -= 2;
        DisplayPages();
    }

    /// <summary>
    /// Determines what to display on each page.
    /// </summary>
    private void DisplayPages()
    {
        if (_currentPageLayouts.Count <= _currentLeftPageIndex)
        {
            leftName.text = "";
            leftImage.color = Color.clear;
            leftDescription.text = "";

            rightName.text = "";
            rightImage.color = Color.clear;
            rightDescription.text = "";
        }
        else if (_currentPageLayouts.Count == _currentLeftPageIndex + 1)
        {
            leftName.text = _currentPageLayouts[_currentLeftPageIndex].Name;
            leftImage.color = Color.white;
            leftImage.sprite = _currentPageLayouts[_currentLeftPageIndex].Sprite;
            leftDescription.text = _currentPageLayouts[_currentLeftPageIndex].Description;

            rightName.text = "";
            rightImage.color = Color.clear;
            rightDescription.text = "";
        }
        else
        {
            leftName.text = _currentPageLayouts[_currentLeftPageIndex].Name;
            leftImage.color = Color.white;
            leftImage.sprite = _currentPageLayouts[_currentLeftPageIndex].Sprite;
            leftDescription.text = _currentPageLayouts[_currentLeftPageIndex].Description;

            rightName.text = _currentPageLayouts[_currentLeftPageIndex + 1].Name;
            rightImage.color = Color.white;
            rightImage.sprite = _currentPageLayouts[_currentLeftPageIndex + 1].Sprite;
            rightDescription.text = _currentPageLayouts[_currentLeftPageIndex + 1].Description;
        }

        leftButton.gameObject.SetActive(_currentLeftPageIndex - 2 >= 0);
        rightButton.gameObject.SetActive(_currentLeftPageIndex + 2 < _currentPageLayouts.Count);
    }

    /// <summary>
    /// Closes the book, enables movement, and fades from a transparent black.
    /// </summary>
    public void CloseBook()
    {
        fader.color = Color.clear;
        playerMovement.CurrentValue = true;
        cameraController.CurrentValue = false;
        _currentLeftPageIndex = 0;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        gameObject.SetActive(false);
    }

    #endregion
}