using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject panelTutorial;
    [SerializeField] private Image imageDisplay;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button prevButton;
    [SerializeField] private Button exitButton;

    [Header("Tutorial Sprites")]
    [SerializeField] private Sprite[] tutorialImages;

    private int currentIndex = 0;

    private void Start()
    {
        panelTutorial.SetActive(false);

        nextButton.onClick.AddListener(NextImage);
        prevButton.onClick.AddListener(PrevImage);
        exitButton.onClick.AddListener(ClosePanel);
    }

    public void OpenTutorial()
    {
        currentIndex = 0;
        panelTutorial.SetActive(true);
        UpdateImage();
        Time.timeScale = 0f;
    }

    private void ClosePanel()
    {
        panelTutorial.SetActive(false);
        Time.timeScale = 1f;
    }

    private void NextImage()
    {
        if (currentIndex < tutorialImages.Length - 1)
        {
            currentIndex++;
            UpdateImage();
        }
    }

    private void PrevImage()
    {
        if (currentIndex > 0)
        {
            currentIndex--;
            UpdateImage();
        }
    }

    private void UpdateImage()
    {
        imageDisplay.sprite = tutorialImages[currentIndex];
        prevButton.gameObject.SetActive(currentIndex > 0);
        nextButton.gameObject.SetActive(currentIndex < tutorialImages.Length - 1);
    }
}
