using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

class RemoveDublicate
{
    static void Main(string[] args)
    {
        try
        {
            // Загрузка JSON из файла
            string json = File.ReadAllText("ShortSummary.json");

            LogMessage("Загружен JSON из файла.");

            // Десериализация JSON в список JsonElement
            List<JsonElement> elements = DeserializeJson<List<JsonElement>>(json);

            LogMessage("Произведена десериализация JSON.");

            // Удаление дублирующих объектов
            List<JsonElement> uniqueElements = RemoveDuplicates(elements);

            LogMessage("Дублирующие объекты успешно удалены.");

            // Сериализация списка уникальных элементов в JSON
            string outputJson = SerializeJson(uniqueElements);

            LogMessage("Произведена сериализация списка уникальных объектов в JSON.");

            // Запись результата в файл
            File.WriteAllText("output.json", outputJson);

            LogMessage("Результат записан в файл output.json.");

            LogMessage("Программа успешно выполнена.");
            Environment.Exit(0);
        }
        catch (Exception ex)
        {
            LogError("Произошла ошибка: " + ex.Message);
        }

        Console.ReadLine();
    }

    static T DeserializeJson<T>(string json)
    {
        return JsonSerializer.Deserialize<T>(json);
    }

    static string SerializeJson<T>(T objects)
    {
        return JsonSerializer.Serialize(objects, new JsonSerializerOptions { WriteIndented = true });
    }

    static List<JsonElement> RemoveDuplicates(List<JsonElement> elements)
    {
        Dictionary<string, JsonElement> uniqueElements = new Dictionary<string, JsonElement>();

        foreach (var element in elements)
        {
            string key = GetPropertyValue(element, "rec_id").ToString() + "_" + GetPropertyValue(element, "ts").ToString();

            // Проверка, есть ли уже ключ в словаре
            if (!uniqueElements.ContainsKey(key))
            {
                uniqueElements.Add(key, element);
            }
        }

        return uniqueElements.Values.ToList();
    }

    static JsonElement GetPropertyValue(JsonElement element, string propertyName)
    {
        if (element.TryGetProperty(propertyName, out var property))
        {
            return property;
        }

        return default;
    }

    static void LogMessage(string message)
    {
        string logMessage = $"[INFO] {DateTime.Now}: {message}";
        Console.WriteLine(logMessage);
        File.AppendAllText("log.txt", logMessage + Environment.NewLine);
    }

    static void LogError(string errorMessage)
    {
        string logMessage = $"[ERROR] {DateTime.Now}: {errorMessage}";
        Console.WriteLine(logMessage);
        File.AppendAllText("log.txt", logMessage + Environment.NewLine);
    }
}
