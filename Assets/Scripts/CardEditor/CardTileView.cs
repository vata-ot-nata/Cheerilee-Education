using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Button))]
public class CardTileView : MonoBehaviour
{
    [SerializeField] private RawImage previewImage;
    [SerializeField] private TMP_Text labelText;

    private CardData card;
    private CardsListController listController;

    public void Setup(CardData card, CardsListController listController)
    {
        this.card = card;
        this.listController = listController;

        labelText.text = card.translations.Count > 0 ? card.translations[0].text : "(без текста)";

        string fullPath = CardStorage.GetImageFullPath(card.imageFileName);
        if (!string.IsNullOrEmpty(fullPath) && System.IO.File.Exists(fullPath))
        {
            byte[] bytes = System.IO.File.ReadAllBytes(fullPath);
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(bytes);
            previewImage.texture = texture;
        }

        GetComponent<Button>().onClick.AddListener(OnTileClicked);
    }

    private void OnTileClicked()
    {
        Debug.Log($"Открываем карточку: {card.id}");
        listController.OpenCardEditor(card);
    }
}