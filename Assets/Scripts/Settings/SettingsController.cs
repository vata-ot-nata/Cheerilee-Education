using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsController : MonoBehaviour
{
    [Header("Список языков")]
    [SerializeField] private Transform languagesContent;        // объект с Vertical Layout Group
    [SerializeField] private GameObject languageRowPrefab;      // префаб строки языка (LanguageRowView)
    [SerializeField] private GameObject addLanguageRowPrefab;   // префаб строки добавления (AddLanguageRowView), всегда последний в списке

    [Header("Выбор родного языка / языка перевода")]
    [SerializeField] private TMP_Dropdown nativeLanguageDropdown;
    [SerializeField] private TMP_Dropdown translationLanguageDropdown;

    [Header("Режимы")]
    [SerializeField] private TMP_Dropdown gameModeDropdown;
    [SerializeField] private TMP_Dropdown learningModeDropdown;

    [Header("Навигация")]
    [SerializeField] private UIScreenManager screenManager;
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private Button backButton;

    private AppSettings settings;
    private readonly List<LanguageRowView> spawnedLanguageRows = new List<LanguageRowView>();

    // Подписи для выпадающих списков режимов. Порядок ОБЯЗАТЕЛЬНО должен
    // совпадать с порядком значений в enum GameMode / LearningMode,
    // т.к. индекс в выпадающем списке напрямую приводится к enum.
    private static readonly string[] GameModeLabels =
    {
        "Картинка -> Перевод",
        "Перевод -> Картинка",
        "Родной язык -> Перевод",
        "Перевод -> Родной язык"
    };

    private static readonly string[] LearningModeLabels =
    {
        "Интервальное повторение",
        "Случайно",
        "Больше всего ошибок",
        "Знакомое"
    };

    private void Awake()
    {
        backButton.onClick.AddListener(OnBackClicked);

        nativeLanguageDropdown.onValueChanged.AddListener(OnNativeLanguageChanged);
        translationLanguageDropdown.onValueChanged.AddListener(OnTranslationLanguageChanged);
        gameModeDropdown.onValueChanged.AddListener(OnGameModeChanged);
        learningModeDropdown.onValueChanged.AddListener(OnLearningModeChanged);
    }

    private void OnEnable()
    {
        settings = SettingsStorage.Load();

        RefreshLanguagesList();
        RefreshLanguageDropdowns();
        RefreshModeDropdown(gameModeDropdown, GameModeLabels, (int)settings.gameMode);
        RefreshModeDropdown(learningModeDropdown, LearningModeLabels, (int)settings.learningMode);
    }

    // ---------- Список языков ----------

    private void RefreshLanguagesList()
    {
        foreach (Transform child in languagesContent)
            Destroy(child.gameObject);
        spawnedLanguageRows.Clear();

        foreach (string language in settings.knownLanguages)
        {
            GameObject rowGO = Instantiate(languageRowPrefab, languagesContent);
            LanguageRowView row = rowGO.GetComponent<LanguageRowView>();
            row.Setup(language, OnRemoveLanguageRequested);
            spawnedLanguageRows.Add(row);
        }

        // строка "добавить язык" всегда последней в списке, как плюсик в гриде карточек
        GameObject addRowGO = Instantiate(addLanguageRowPrefab, languagesContent);
        AddLanguageRowView addRow = addRowGO.GetComponent<AddLanguageRowView>();
        addRow.Setup(OnAddLanguageConfirmed);
    }

    private void OnAddLanguageConfirmed(string newLanguage)
    {
        if (settings.knownLanguages.Contains(newLanguage))
        {
            Debug.Log($"Язык '{newLanguage}' уже есть в списке");
            return;
        }

        settings.knownLanguages.Add(newLanguage);
        SettingsStorage.Save(settings);

        RefreshLanguagesList();
        RefreshLanguageDropdowns();
    }

    private void OnRemoveLanguageRequested(string language)
    {
        settings.knownLanguages.Remove(language);

        // Если удаляемый язык был выбран как родной или как язык перевода - сбрасываем выбор,
        // чтобы не остаться со ссылкой на язык, которого больше нет в списке
        if (settings.nativeLanguage == language)
            settings.nativeLanguage = "";
        if (settings.translationLanguage == language)
            settings.translationLanguage = "";

        SettingsStorage.Save(settings);

        RefreshLanguagesList();
        RefreshLanguageDropdowns();
    }

    // ---------- Родной язык / язык перевода ----------

    private void RefreshLanguageDropdowns()
    {
        // Снимаем слушателей на время перестройки списка, чтобы SetValueWithoutNotify
        // не спровоцировал случайный вызов OnNativeLanguageChanged/OnTranslationLanguageChanged
        nativeLanguageDropdown.onValueChanged.RemoveListener(OnNativeLanguageChanged);
        translationLanguageDropdown.onValueChanged.RemoveListener(OnTranslationLanguageChanged);

        nativeLanguageDropdown.ClearOptions();
        translationLanguageDropdown.ClearOptions();
        nativeLanguageDropdown.AddOptions(settings.knownLanguages);
        translationLanguageDropdown.AddOptions(settings.knownLanguages);

        int nativeIndex = Mathf.Max(settings.knownLanguages.IndexOf(settings.nativeLanguage), 0);
        int translationIndex = Mathf.Max(settings.knownLanguages.IndexOf(settings.translationLanguage), 0);

        nativeLanguageDropdown.SetValueWithoutNotify(nativeIndex);
        translationLanguageDropdown.SetValueWithoutNotify(translationIndex);

        // Если значение было сброшено (язык удалили), подхватываем то, что теперь показывает dropdown
        if (settings.knownLanguages.Count > 0)
        {
            if (string.IsNullOrEmpty(settings.nativeLanguage))
                settings.nativeLanguage = settings.knownLanguages[nativeIndex];
            if (string.IsNullOrEmpty(settings.translationLanguage))
                settings.translationLanguage = settings.knownLanguages[translationIndex];
        }

        nativeLanguageDropdown.onValueChanged.AddListener(OnNativeLanguageChanged);
        translationLanguageDropdown.onValueChanged.AddListener(OnTranslationLanguageChanged);
    }

    private void OnNativeLanguageChanged(int index)
    {
        if (index < 0 || index >= settings.knownLanguages.Count) return;
        settings.nativeLanguage = settings.knownLanguages[index];
        SettingsStorage.Save(settings);
    }

    private void OnTranslationLanguageChanged(int index)
    {
        if (index < 0 || index >= settings.knownLanguages.Count) return;
        settings.translationLanguage = settings.knownLanguages[index];
        SettingsStorage.Save(settings);
    }

    // ---------- Режимы ----------

    private void RefreshModeDropdown(TMP_Dropdown dropdown, string[] labels, int currentIndex)
    {
        dropdown.ClearOptions();
        dropdown.AddOptions(labels.ToList());
        dropdown.SetValueWithoutNotify(currentIndex);
    }

    private void OnGameModeChanged(int index)
    {
        settings.gameMode = (GameMode)index;
        SettingsStorage.Save(settings);
    }

    private void OnLearningModeChanged(int index)
    {
        settings.learningMode = (LearningMode)index;
        SettingsStorage.Save(settings);
    }

    // ---------- Навигация ----------

    private void OnBackClicked()
    {
        screenManager.ShowScreen(mainMenuPanel);
    }
}