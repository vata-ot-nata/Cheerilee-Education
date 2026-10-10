using System;
using System.IO;
using UnityEngine;

// Статический сервис для настроек, по аналогии с CardStorage.
// Отдельный файл settings.json в persistentDataPath.
public static class SettingsStorage
{
    private const string JsonFileName = "settings.json";
    private static string JsonPath => Path.Combine(Application.persistentDataPath, JsonFileName);

    public static AppSettings Load()
    {
        if (!File.Exists(JsonPath))
        {
            Debug.Log("Файл настроек не найден, создаём настройки по умолчанию");
            return new AppSettings();
        }

        try
        {
            string json = File.ReadAllText(JsonPath);
            AppSettings settings = JsonUtility.FromJson<AppSettings>(json);
            return settings ?? new AppSettings();
        }
        catch (Exception e)
        {
            Debug.LogError($"Не удалось прочитать settings.json: {e.Message}");
            return new AppSettings();
        }
    }

    public static void Save(AppSettings settings)
    {
        try
        {
            string json = JsonUtility.ToJson(settings, true);
            File.WriteAllText(JsonPath, json);
            Debug.Log($"Настройки сохранены: {JsonPath}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Не удалось сохранить settings.json: {e.Message}");
        }
    }
}
