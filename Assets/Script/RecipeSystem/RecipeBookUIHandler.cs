using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class RecipeBookUIHandler : MonoBehaviour
{
    [Header("UI References")]
    public GameObject recipeBookPanel;
    public GameObject recipeDetailPanel; // Ini akan menjadi panel utama yang menampilkan resep

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

    public void ToggleRecipeBook()
    {
        recipeBookPanel.SetActive(!recipeBookPanel.activeSelf);
        if (recipeBookPanel.activeSelf)
        {
            // Tampilkan resep pertama saat buku dibuka
            currentPotionIndex = 0;
            ShowRecipeDetails(allPotions[currentPotionIndex]);
        }
    }
    
    // Fungsi untuk menampilkan resep berikutnya
    public void ShowNextPotion()
    {
        if (allPotions.Count == 0) return;

        currentPotionIndex++;
        if (currentPotionIndex >= allPotions.Count)
        {
            currentPotionIndex = 0; // Kembali ke resep pertama jika sudah di akhir
        }
        ShowRecipeDetails(allPotions[currentPotionIndex]);
    }
    
    // Fungsi untuk menampilkan resep sebelumnya
    public void ShowPreviousPotion()
    {
        if (allPotions.Count == 0) return;

        currentPotionIndex--;
        if (currentPotionIndex < 0)
        {
            currentPotionIndex = allPotions.Count - 1; // Kembali ke resep terakhir jika sudah di awal
        }
        ShowRecipeDetails(allPotions[currentPotionIndex]);
    }
    
    public void ShowRecipeDetails(Potion potion)
    {
        if (potion == null) return;
        
        // Update informasi detail
        potionImageDetail.sprite = potion.potionImage;
        potionNameDetail.text = potion.potionName;
        potionEffectDetail.text = potion.potionDescription;

        // Hapus item bahan lama
        foreach (Transform child in ingredientsParent)
        {
            Destroy(child.gameObject);
        }

        // Tampilkan daftar bahan baru
        for (int i = 0; i < potion.requiredIngredients.Length; i++)
        {
            Ingredient ingredient = potion.requiredIngredients[i];
           

            GameObject ingredientItem = Instantiate(ingredientItemPrefab, ingredientsParent);
            ingredientItem.GetComponentInChildren<Image>().sprite = ingredient.ingredientImage;
            ingredientItem.GetComponentInChildren<TMP_Text>().text = $"{ingredient.ingredientName}";
        }
    }
}