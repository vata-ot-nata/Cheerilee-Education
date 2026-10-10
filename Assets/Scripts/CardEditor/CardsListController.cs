using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardsListController : MonoBehaviour
{
    [Header("Ссылки на UI")]
    [SerializeField] private Transform gridContent;      // объект с GridLayoutGroup, куда спавнятся плитки
    [SerializeField] private GameObject cardTilePrefab;   // префаб плитки карточки (с CardTileView)
    [SerializeField] private GameObject addCardTilePrefab; // префаб плитки "+"

    [Header("Навигация")]
    [SerializeField] private UIScreenManager screenManager;
    [SerializeField] private GameObject cardsListPanel;
    [SerializeField] private GameObject cardEditorPanel;
    [SerializeField] private CardEditorController cardEditorController;

    private CardDatabase database;

    private void OnEnable()
    {
        RefreshGrid();
    }

    public void RefreshGrid()
    {
        database = CardStorage.Load();

        // очищаем грид перед перерисовкой
        foreach (Transform child in gridContent)
            Destroy(child.gameObject);

        foreach (CardData card in database.cards)
        {
            GameObject tileGO = Instantiate(cardTilePrefab, gridContent);
            tileGO.GetComponent<CardTileView>().Setup(card, this);
        }

        // плюсик всегда последней плиткой в гриде
        GameObject addTileGO = Instantiate(addCardTilePrefab, gridContent);
        Button addButton = addTileGO.GetComponent<Button>();
        addButton.onClick.AddListener(OnAddCardClicked);
    }

    private void OnAddCardClicked()
    {
        CardData newCard = new CardData();
        database.cards.Add(newCard);
        CardStorage.Save(database);

        Debug.Log($"Создана новая карточка: {newCard.id}");
        OpenCardEditor(newCard);
    }

    public void OpenCardEditor(CardData card)
    {
        cardEditorController.LoadCard(card, database);
        screenManager.ShowScreen(cardEditorPanel);
    }

    // вешается на кнопку "Назад" в списке карточек, ведёт в главное меню
    public void OnBackClicked(GameObject mainMenuPanel)
    {
        screenManager.ShowScreen(mainMenuPanel);
    }
}
