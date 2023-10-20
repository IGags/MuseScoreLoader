using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ConvertApiDotNet;

namespace Logic.Services;

public static class KeyStorageService
{
    private const string KeyPath = "key.txt";
    private static List<(int, string)> _keys = new ();

    public static string GetKey(int usageCount)
    {
        try
        {
            var key = _keys.First(x => x.Item1 > usageCount);
            key.Item1 -= usageCount;
            if (key.Item1 == 0)
            {
                _keys.Remove(key);
                RewriteKeys(_keys);
            }
            return key.Item2;
        }
        catch (Exception)
        {
            Console.WriteLine($"Нет свободного ключа под количество использований");
            throw;
        }
    }

    public static void AddKey(string key)
    {
        if (_keys.Any(x => x.Item2.Equals(key)))
        {
            Console.WriteLine("Такой ключ уже есть");
            return;
        }
        try
        {
            _keys.Add((ValidateKey(key), key));
            RewriteKeys(_keys);
        }
        catch (Exception e) { }
    }
    
    static KeyStorageService()
    {
        if (File.Exists(KeyPath))
        {
            var keys = File.ReadAllText(KeyPath).Split('\n', StringSplitOptions.RemoveEmptyEntries);
            _keys = ValidateKeys(keys.ToList());
            if (_keys.Count != 0)
            {
                RewriteKeys(_keys);
                return;
            }
        }
        
        Console.WriteLine("Добавьте ключей через рест");
    }

    private static void RewriteKeys(List<(int, string)> keyList) =>
        File.WriteAllText(KeyPath, string.Join('\n', _keys.Select(x => x.Item2)));
    
    private static List<(int, string)> ValidateKeys(List<string> keys)
    {
        var keyList = new List<(int, string)>();
        foreach (var key in keys)
        {
            try
            {
                keyList.Add((ValidateKey(key), key));
            }
            catch { }
        }

        return keyList;
    }

    private static int ValidateKey(string key)
    {
        try
        {
            var api = new ConvertApi(key);
            var user = api.GetUserAsync().Result;
            if (user.ConversionsTotal - user.ConversionsConsumed == 0)
            {
                Console.WriteLine($"Путой ключ {key}");
                throw new Exception();
            }

            return user.ConversionsTotal - user.ConversionsConsumed;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Неверный ключ {key}");
            throw;
        }
    }
}