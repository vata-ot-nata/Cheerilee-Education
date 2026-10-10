using System;
using UnityEngine;

// Единая точка входа для "выбрать картинку с устройства".
// Логика разделена по платформам через #if, чтобы один и тот же вызов
// ImagePicker.PickImage(callback) работал и в редакторе/Windows/Mac, и на Android/iOS.
public static class ImagePicker
{
    // callback вызывается с полным путём к выбранному файлу,
    // или с null, если пользователь отменил выбор
    public static void PickImage(Action<string> onPicked)
    {
#if UNITY_EDITOR
        PickImageEditor(onPicked);
#elif UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_STANDALONE_LINUX
        PickImageStandalone(onPicked);
#elif UNITY_ANDROID || UNITY_IOS
        PickImageMobile(onPicked);
#else
        Debug.LogWarning("Выбор файла не реализован для этой платформы");
        onPicked?.Invoke(null);
#endif
    }

#if UNITY_EDITOR
    private static void PickImageEditor(Action<string> onPicked)
    {
        string path = UnityEditor.EditorUtility.OpenFilePanel(
            "Выберите изображение", "", "png,jpg,jpeg");

        onPicked?.Invoke(string.IsNullOrEmpty(path) ? null : path);
    }
#endif

#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_STANDALONE_LINUX
    private static void PickImageStandalone(Action<string> onPicked)
    {
        // В чистом Unity нет встроенного нативного диалога выбора файла для билда (не редактора).
        // Самый надёжный вариант - бесплатный пакет "Standalone File Browser":
        // https://github.com/gkngkc/UnityStandaloneFileBrowser
        // После импорта пакета вызов выглядит примерно так:
        //
        // var extensions = new [] { new SFB.ExtensionFilter("Images", "png", "jpg", "jpeg") };
        // string[] paths = SFB.StandaloneFileBrowser.OpenFilePanel("Выберите изображение", "", extensions, false);
        // onPicked?.Invoke(paths.Length > 0 ? paths[0] : null);

        Debug.LogWarning("Standalone File Browser не подключен - см. комментарий в ImagePicker.cs");
        onPicked?.Invoke(null);
    }
#endif

#if UNITY_ANDROID || UNITY_IOS
    private static void PickImageMobile(Action<string> onPicked)
    {
        // Для Android/iOS системную галерею/файловый пикер открывает нативный код,
        // которого нет в чистом Unity API. Стандартное решение - бесплатный пакет
        // "NativeFilePicker" (Asset Store / GitHub: yasirkula/UnityNativeFilePicker).
        // После импорта пакета вызов выглядит примерно так:
        //
        // NativeFilePicker.PickFile((path) => onPicked?.Invoke(path), new[] { "image/*" });
        //
        // Пакет сам запрашивает разрешения на доступ к галерее/файлам на обеих платформах.

        Debug.LogWarning("NativeFilePicker не подключен - см. комментарий в ImagePicker.cs");
        onPicked?.Invoke(null);
    }
#endif
}
