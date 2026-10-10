using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    public void OnCardClicked()
    {
        Debug.Log("Кнопка: Карточки — нажата");
    }
 
    public void OnStudyClicked()
    {
        Debug.Log("Кнопка: Обучаться — нажата");
    }
 
    public void OnStatisticsClicked()
    {
        Debug.Log("Кнопка: Статистика — нажата");
    }
 
    public void OnSettingsClicked()
    {
        Debug.Log("Кнопка: Настройки — нажата");
    }
}
