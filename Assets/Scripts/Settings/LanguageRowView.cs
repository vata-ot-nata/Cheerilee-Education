using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LanguageRowView : MonoBehaviour
{
    [SerializeField] private TMP_Text languageLabel;
    [SerializeField] private Button removeButton;

    private string language;
    private Action<string> onRemoveRequested;

    public void Setup(string language, Action<string> onRemoveRequested)
    {
        this.language = language;
        this.onRemoveRequested = onRemoveRequested;

        languageLabel.text = language;

        removeButton.onClick.RemoveAllListeners();
        removeButton.onClick.AddListener(() => onRemoveRequested?.Invoke(this.language));
    }
}
