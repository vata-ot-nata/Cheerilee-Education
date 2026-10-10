using System;
using System.Collections.Generic;

// Один перевод: язык + текст
[Serializable]
public class CardTranslation
{
    public string language; // например "RU", "EN", "JP" - произвольная строка
    public string text;

    public CardTranslation(string language, string text)
    {
        this.language = language;
        this.text = text;
    }
}

// Данные одной карточки
[Serializable]
public class CardData
{
    public string id;                 // уникальный идентификатор (GUID)
    public string imageFileName;      // имя файла картинки внутри persistentDataPath/Images/, может быть пустым
    public List<CardTranslation> translations = new List<CardTranslation>();

    public CardData()
    {
        id = Guid.NewGuid().ToString();
    }
}

// JsonUtility не умеет сериализовать List<> на верхнем уровне напрямую,
// поэтому оборачиваем список карточек в класс
[Serializable]
public class CardDatabase
{
    public List<CardData> cards = new List<CardData>();
}
