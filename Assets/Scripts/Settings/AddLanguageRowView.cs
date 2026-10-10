using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AddLanguageRowView : MonoBehaviour
{
    [SerializeField] private TMP_InputField languageInput;
    [SerializeField] private Button confirmButton; // "галочка"

    private Action<string> onConfirm;

    public void Setup(Action<string> onConfirm)
    {
        this.onConfirm = onConfirm;

        confirmButton.onClick.RemoveAllListeners();
        confirmButton.onClick.AddListener(OnConfirmClicked);
    }

    private void OnConfirmClicked()
    {
        string value = languageInput.text.Trim();
        if (string.IsNullOrEmpty(value))
            return;

        onConfirm?.Invoke(value);
        languageInput.text = "";
    }
}
