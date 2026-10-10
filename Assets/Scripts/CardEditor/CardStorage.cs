using System;
using System.IO;
using UnityEngine;

// Статический сервис: работа с диском не зависит от конкретной сцены/объекта,
// поэтому ничего не наследует MonoBehaviour и доступен отовсюду как CardStorage.Load()
public static class CardStorage
{
    private const string JsonFileName = "cards.json";
    private const string ImagesFolderName = "Images";

    private static string JsonPath => Path.Combine(Application.persistentDataPath, JsonFileName);
    private static string ImagesFolderPath => Path.Combine(Application.persistentDataPath, ImagesFolderName);

    public static CardDatabase Load()
    {
        EnsureImagesFolderExists();

        if (!File.Exists(JsonPath))
        {
            Debug.Log("Файл карточек не найден, создаём пустую базу");
            return new CardDatabase();
        }

        try
        {
            string json = File.ReadAllText(JsonPath);
            CardDatabase database = JsonUtility.FromJson<CardDatabase>(json);
            return database ?? new CardDatabase();
        }
        catch (Exception e)
        {
            Debug.LogError($"Не удалось прочитать cards.json: {e.Message}");
            return new CardDatabase();
        }
    }

    public static void Save(CardDatabase database)
    {
        try
        {
            string json = JsonUtility.ToJson(database, true);
            File.WriteAllText(JsonPath, json);
            Debug.Log($"Карточки сохранены: {JsonPath}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Не удалось сохранить cards.json: {e.Message}");
        }
    }

    // Копирует выбранный пользователем файл картинки в persistentDataPath/Images
    // и возвращает имя файла, которое нужно записать в CardData.imageFileName
    public static string ImportImage(string sourceFilePath, string cardId)
    {
        EnsureImagesFolderExists();

        try
        {
            string extension = Path.GetExtension(sourceFilePath);
            string fileName = $"{cardId}{extension}";
            string destinationPath = Path.Combine(ImagesFolderPath, fileName);

            File.Copy(sourceFilePath, destinationPath, true);
            return fileName;
        }
        catch (Exception e)
        {
            Debug.LogError($"Не удалось импортировать картинку: {e.Message}");
            return null;
        }
    }

    // Полный путь к картинке карточки, готовый для загрузки в Texture2D
    public static string GetImageFullPath(string imageFileName)
    {
        if (string.IsNullOrEmpty(imageFileName))
            return null;

        return Path.Combine(ImagesFolderPath, imageFileName);
    }

    // Удаляет файл картинки карточки с диска (вызывается при удалении самой карточки
    // или при замене картинки на новую)
    public static void DeleteImage(string imageFileName)
    {
        string fullPath = GetImageFullPath(imageFileName);
        if (string.IsNullOrEmpty(fullPath))
            return;

        try
        {
            if (File.Exists(fullPath))
                File.Delete(fullPath);
        }
        catch (Exception e)
        {
            Debug.LogError($"Не удалось удалить картинку: {e.Message}");
        }
    }

    private static void EnsureImagesFolderExists()
    {
        if (!Directory.Exists(ImagesFolderPath))
            Directory.CreateDirectory(ImagesFolderPath);
    }
}