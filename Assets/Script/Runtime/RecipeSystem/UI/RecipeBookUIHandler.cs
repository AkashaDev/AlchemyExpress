using UnityEngine;
using UnityEngine.UI;
using System.Collections; // BARU: Diperlukan untuk Coroutine
using System.Collections.Generic;
using TMPro;

public class RecipeBookUIHandler : MonoBehaviour
{
    [Header("UI References")]
    public GameObject recipeBookPanel;
    public GameObject recipeDetailPanel;

    // BARU: Tambahkan referensi ke CanvasGroup dari panel detail
    public CanvasGroup recipeDetailCanvasGroup;

    [Header("Recipe Detail UI")]
    public Image potionImageDetail;
    public TMP_Text potionNameDetail;
    public TMP_Text potionEffectDetail;
    public Transform ingredientsParent;
    public GameObject ingredientItemPrefab;

    [Header("Data")]
    public RecipeBookData recipeBookData;

    [Header("Navigation Buttons")]
    public Button previousButton;
    public Button nextButton;

    [Header("Pause Settings")]
    public GameObject pausePanel;
    public GameObject SettingsPanel;
    public Button resumeButton;
    public Button backToMenuButton;
    public Button pauseButton;
    public Button SettingsButton;
    public Button backToPauseButton;
    private bool isPaused = false;

    // BARU: Pengaturan untuk transisi
    [Header("Transition Settings")]
    public float transitionSpeed = 5f;
    private bool isTransitioning = false; // Flag untuk mencegah klik ganda saat transisi

    private List<Potion> allPotions;
    private int currentPotionIndex = 0;

    private void Start()
    {
        recipeBookPanel.SetActive(false);
        allPotions = recipeBookData.GetAllPotions();

        previousButton.onClick.AddListener(ShowPreviousPotion);
        nextButton.onClick.AddListener(ShowNextPotion);
    }

    public void ToggleRecipeBook()
    {
        recipeBookPanel.SetActive(!recipeBookPanel.activeSelf);
        if (recipeBookPanel.activeSelf && allPotions.Count > 0)
        {
            currentPotionIndex = 0;
            // Langsung tampilkan tanpa transisi saat pertama kali dibuka
            ShowRecipeDetails(allPotions[currentPotionIndex]);
            recipeDetailCanvasGroup.alpha = 1; // Pastikan terlihat jelas
        }
    }


    // MODIFIKASI: Fungsi ini sekarang akan memulai Coroutine transisi
    public void ShowNextPotion()
    {
        if (allPotions.Count == 0 || isTransitioning) return;

        currentPotionIndex++;
        if (currentPotionIndex >= allPotions.Count)
        {
            currentPotionIndex = 0;
        }
        StartCoroutine(TransitionToPotion(allPotions[currentPotionIndex]));
    }

    // MODIFIKASI: Fungsi ini juga akan memulai Coroutine transisi
    public void ShowPreviousPotion()
    {
        if (allPotions.Count == 0 || isTransitioning) return;

        currentPotionIndex--;
        if (currentPotionIndex < 0)
        {
            currentPotionIndex = allPotions.Count - 1;
        }
        StartCoroutine(TransitionToPotion(allPotions[currentPotionIndex]));
    }

    // BARU: Coroutine untuk menangani animasi transisi fade-out dan fade-in
    private IEnumerator TransitionToPotion(Potion potion)
    {
        isTransitioning = true;

        // Fase 1: Fade Out (Memudar)
        while (recipeDetailCanvasGroup.alpha > 0)
        {
            recipeDetailCanvasGroup.alpha -= Time.deltaTime * transitionSpeed;
            yield return null; // Tunggu frame berikutnya
        }

        // Fase 2: Ganti Konten Resep (saat tidak terlihat)
        ShowRecipeDetails(potion);

        // Fase 3: Fade In (Muncul Kembali)
        while (recipeDetailCanvasGroup.alpha < 1)
        {
            recipeDetailCanvasGroup.alpha += Time.deltaTime * transitionSpeed;
            yield return null; // Tunggu frame berikutnya
        }

        isTransitioning = false;
    }

    // Fungsi ini tidak berubah, hanya dipanggil di tengah transisi
    public void ShowRecipeDetails(Potion potion)
    {
        if (potion == null) return;

        potionImageDetail.sprite = potion.potionImageBook;
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

    #region Pause UI
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

    public void BackToMainMenu(string sceneName = "Main Menu")
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }
    #endregion
}