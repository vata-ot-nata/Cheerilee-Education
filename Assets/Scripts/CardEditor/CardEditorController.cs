using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardEditorController : MonoBehaviour
{
    [Header("Картинка")]
    [SerializeField] private RawImage previewImage;
    [SerializeField] private Button changeImageButton;

    [Header("Переводы")]
    [SerializeField] private Transform translationsContent; // объект с Vertical Layout Group
    [SerializeField] private GameObject translationRowPrefab;
    [SerializeField] private Button addTranslationButton;

    [Header("Навигация")]
    [SerializeField] private UIScreenManager screenManager;
    [SerializeField] private GameObject cardsListPanel;
    [SerializeField] private Button backButton;
    [SerializeField] private Button deleteCardButton;
    [SerializeField] private CardsListController listController;

    private CardData currentCard;
    private CardDatabase database;
    private readonly List<TranslationRowView> spawnedRows = new List<TranslationRowView>();

    private void Awake()
    {
        changeImageButton.onClick.AddListener(OnChangeImageClicked);
        addTranslationButton.onClick.AddListener(OnAddTranslationClicked);
        backButton.onClick.AddListener(OnBackClicked);
        deleteCardButton.onClick.AddListener(OnDeleteCardClicked);
    }

    public void LoadCard(CardData card, CardDatabase database)
    {
        currentCard = card;
        this.database = database;

        RefreshImagePreview();
        RefreshTranslationsList();
    }

    private void RefreshImagePreview()
    {
        string fullPath = CardStorage.GetImageFullPath(currentCard.imageFileName);
        if (!string.IsNullOrEmpty(fullPath) && System.IO.File.Exists(fullPath))
        {
            byte[] bytes = System.IO.File.ReadAllBytes(fullPath);
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(bytes);
            previewImage.texture = texture;
        }
        else
        {
            previewImage.texture = null;
        }
    }

    private void RefreshTranslationsList()
    {
        foreach (Transform child in translationsContent)
            Destroy(child.gameObject);
        spawnedRows.Clear();

        foreach (CardTranslation translation in currentCard.translations)
            SpawnTranslationRow(translation);
    }

    private void SpawnTranslationRow(CardTranslation translation)
    {
        GameObject rowGO = Instantiate(translationRowPrefab, translationsContent);
        TranslationRowView row = rowGO.GetComponent<TranslationRowView>();
        row.Setup(translation, OnRemoveTranslationRequested);
        spawnedRows.Add(row);
    }

    private void OnChangeImageClicked()
    {
        ImagePicker.PickImage(OnImagePicked);
    }

    private void OnImagePicked(string sourceFilePath)
    {
        if (string.IsNullOrEmpty(sourceFilePath))
        {
            Debug.Log("Выбор изображения отменён");
            return;
        }

        string savedFileName = CardStorage.ImportImage(sourceFilePath, currentCard.id);
        if (savedFileName != null)
        {
            currentCard.imageFileName = savedFileName;
            RefreshImagePreview();
            SaveDatabase();
        }
    }

    private void OnAddTranslationClicked()
    {
        CardTranslation newTranslation = new CardTranslation("", "");
        currentCard.translations.Add(newTranslation);
        SpawnTranslationRow(newTranslation);
        SaveDatabase();
    }

    private void OnRemoveTranslationRequested(TranslationRowView row)
    {
        int index = spawnedRows.IndexOf(row);
        if (index < 0) return;

        currentCard.translations.RemoveAt(index);
        spawnedRows.RemoveAt(index);
        Destroy(row.gameObject);
        SaveDatabase();
    }

    private void OnDeleteCardClicked()
    {
        if (currentCard == null) return;

        database.cards.Remove(currentCard);
        CardStorage.DeleteImage(currentCard.imageFileName);
        SaveDatabase();

        currentCard = null;

        listController.RefreshGrid();
        screenManager.ShowScreen(cardsListPanel);
    }

    private void OnBackClicked()
    {
        SaveDatabase();
        listController.RefreshGrid();
        screenManager.ShowScreen(cardsListPanel);
    }

    private void SaveDatabase()
    {
        CardStorage.Save(database);
    }
}