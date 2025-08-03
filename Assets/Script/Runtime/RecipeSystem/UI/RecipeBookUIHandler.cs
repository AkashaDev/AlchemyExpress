using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using NPC;

public class RecipeBookUIHandler : MonoBehaviour
{
    [Header("UI References")]
    public GameObject recipeBookPanel;
    public GameObject recipeDetailPanel; // Ini akan menjadi panel utama yang menampilkan resep
    public WaveManager waveManager;

    [Header("Recipe Detail UI")]
    public Image potionImageDetail;
    public TMP_Text potionNameDetail;
    public TMP_Text potionEffectDetail;
    public Transform ingredientsParent;
    public GameObject ingredientItemPrefab;
    public Sprite BookOpenImage;
    public Sprite BookClosedImage;
    public GameObject BookImageTarget;

    [Header("Pause Settings")]
    public GameObject pausePanel;
    public GameObject SettingsPanel;
    public Button resumeButton;
    public Button backToMenuButton;
    public Button pauseButton;
    public Button SettingsButton;
    public Button backToPauseButton;
    private bool isPaused = false;

    [Header("Data")]
    public RecipeBookData recipeBookData;

    [Header("Navigation Buttons")]
    public Button previousButton;
    public Button nextButton;

    [Header("UI Text")]
    public TMP_Text currentDayText;
    public TMP_Text npcCountText;
    public TMP_Text playerIncomeText;
    public TMP_Text targetIncomeText;



    private List<Potion> allPotions;
    private int currentPotionIndex = 0;

    private void Start()
    {
        recipeBookPanel.SetActive(false);
        allPotions = recipeBookData.GetAllPotions();

        // Hubungkan tombol navigasi
        previousButton.onClick.AddListener(ShowPreviousPotion);
        nextButton.onClick.AddListener(ShowNextPotion);
    }

    private void Update()
    {
        if (waveManager == null) return;

        currentDayText.text = $"Day {waveManager.CurrentDay}";
        npcCountText.text = $"{waveManager.NPCsSpawnedThisDay}/{waveManager.NPCsToSpawnThisDay}";
        playerIncomeText.text = $"{waveManager.PlayerIncomeThisDay}";
        targetIncomeText.text = string.Format("Gain target at least {0} gold today", waveManager.TargetIncomeThisDay);
    }

    public void ToggleRecipeBook()
    {
        recipeBookPanel.SetActive(!recipeBookPanel.activeSelf);
        if (recipeBookPanel.activeSelf)
        {
            BookImageTarget.GetComponent<Image>().sprite = BookOpenImage;
            // currentPotionIndex = 0;
            ShowRecipeDetails(allPotions[currentPotionIndex]);
        }
        else
        {
            BookImageTarget.GetComponent<Image>().sprite = BookClosedImage;
        }
    }
    
    public void ShowNextPotion()
    {
        if (allPotions.Count == 0) return;

        currentPotionIndex++;
        if (currentPotionIndex >= allPotions.Count)
        {
            currentPotionIndex = 0;
        }
        ShowRecipeDetails(allPotions[currentPotionIndex]);
    }
    
    public void ShowPreviousPotion()
    {
        if (allPotions.Count == 0) return;

        currentPotionIndex--;
        if (currentPotionIndex < 0)
        {
            currentPotionIndex = allPotions.Count - 1;
        }
        ShowRecipeDetails(allPotions[currentPotionIndex]);
    }
    
    public void ShowRecipeDetails(Potion potion)
    {
        if (potion == null) return;
        
        potionImageDetail.sprite = potion.potionImage;
        potionNameDetail.text = potion.potionName;
        potionEffectDetail.text = potion.potionDescription;

        foreach (Transform child in ingredientsParent)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < potion.requiredIngredients.Length; i++)
        {
            IngredientSO ingredient = potion.requiredIngredients[i];
           

            GameObject ingredientItem = Instantiate(ingredientItemPrefab, ingredientsParent);
            ingredientItem.GetComponentInChildren<Image>().sprite = ingredient.itemIcon;
            ingredientItem.GetComponentInChildren<TMP_Text>().text = $"{ingredient.ingredientName}";
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        pausePanel.SetActive(isPaused);

        Time.timeScale = isPaused ? 0f : 1f;
    }

    public void ResumeGame()
    {
        isPaused = false;
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void SettingsGame() 
    {
        pausePanel.SetActive(false);
        SettingsPanel.SetActive(true);
    }

    public void BackToPause() 
    {
        SettingsPanel.SetActive(false);
        pausePanel.SetActive(true);
    }

    public void BackToMainMenu()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene("Main Menu");
    }
}