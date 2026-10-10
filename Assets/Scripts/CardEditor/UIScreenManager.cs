using UnityEngine;

public class UIScreenManager : MonoBehaviour
{
    [Tooltip("Все панели-экраны меню (дети одного Canvas)")]
    [SerializeField] private GameObject[] screens;

    private void Start()
    {
        // При старте показываем только первый экран, остальные скрываем
        if (screens.Length > 0)
            ShowScreen(screens[0]);
    }

    public void ShowScreen(GameObject screenToShow)
    {
        foreach (var screen in screens)
        {
            if (screen != null)
                screen.SetActive(screen == screenToShow);
        }

        if (screenToShow != null)
            Debug.Log($"Открыт экран: {screenToShow.name}");
    }
}