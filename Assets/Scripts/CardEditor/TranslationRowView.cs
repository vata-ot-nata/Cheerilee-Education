using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TranslationRowView : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown languageDropdown;
    [SerializeField] private TMP_InputField textInput;
    [SerializeField] private Button removeButton;

    private CardTranslation translation;
    private System.Action<TranslationRowView> onRemoveRequested;

    public void Setup(CardTranslation translation, System.Action<TranslationRowView> onRemoveRequested)
    {
        this.translation = translation;
        this.onRemoveRequested = onRemoveRequested;

        SetupLanguageDropdown();

        textInput.text = translation.text;
        textInput.onValueChanged.AddListener(value => translation.text = value);
        removeButton.onClick.AddListener(() => onRemoveRequested?.Invoke(this));
    }

    private void SetupLanguageDropdown()
    {
        List<string> languages = SettingsStorage.Load().knownLanguages.ToList();

        // На случай старых карточек, где язык не входит в текущий список известных
        // (например, язык потом удалили из настроек) - добавляем его отдельно,
        // чтобы не потерять уже сохранённое значение молча
        if (!string.IsNullOrEmpty(translation.language) && !languages.Contains(translation.language))
            languages.Insert(0, translation.language);

        languageDropdown.onValueChanged.RemoveAllListeners();
        languageDropdown.ClearOptions();
        languageDropdown.AddOptions(languages);

        int currentIndex = Mathf.Max(languages.IndexOf(translation.language), 0);
        languageDropdown.SetValueWithoutNotify(currentIndex);

        if (languages.Count > 0)
            translation.language = languages[currentIndex];

        languageDropdown.onValueChanged.AddListener(index => translation.language = languages[index]);
    }
}
