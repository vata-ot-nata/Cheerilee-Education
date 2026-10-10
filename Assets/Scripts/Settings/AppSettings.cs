using System;
using System.Collections.Generic;

// Режим игры: что показываем игроку -> что он должен угадать
public enum GameMode
{
    ImageToTranslation,   // Картинка -> Перевод
    TranslationToImage,   // Перевод -> Картинка
    NativeToTranslation,  // Родной язык -> Перевод
    TranslationToNative   // Перевод -> Родной язык
}

// Режим подбора карточек для тренировки
public enum LearningMode
{
    SpacedRepetition, // Интервальное повторение
    Random,           // Случайно
    MostMistakes,     // Больше всего ошибок
    Familiar          // Знакомое
}

// Глобальные настройки приложения. Сохраняются в отдельный settings.json,
// отдельно от базы карточек, т.к. это разные по смыслу данные.
[Serializable]
public class AppSettings
{
    // Список всех языков, добавленных пользователем в Настройках.
    // Именно из этого списка потом заполняются выпадающие списки в карточках.
    public List<string> knownLanguages = new List<string>();

    public string nativeLanguage = "";
    public string translationLanguage = "";

    public GameMode gameMode = GameMode.ImageToTranslation;
    public LearningMode learningMode = LearningMode.SpacedRepetition;
}
