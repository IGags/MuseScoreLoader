using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ConvertApiDotNet;

namespace Logic.Services;

public static class KeyStorageService
{
    public const string KeyPath = "key.txt";
    private static List<ApiKey> _keys = new ();
    private static object _syncRoot = new();

    public static int GetTotalConversions()
    {
        lock (_syncRoot)
        {
            return _keys.Select(x => x.UsageCount).Sum();
        }
    }
    
    public static string GetKey(int usageCount)
    {
        try
        {
            lock (_syncRoot)
            {
                var key = _keys.First(x => x.UsageCount >= usageCount);
                _keys.Remove(key);
                key = key with {UsageCount = key.UsageCount - usageCount};
                if (key.UsageCount == 0)
                {
                    RewriteKeys(_keys);
                }
                _keys.Add(key);
                return key.Secret;
            }
        }
        catch (Exception)
        {
            Console.WriteLine($"Нет свободного ключа под количество использований");
            throw;
        }
    }

    public static void AddKey(string key)
    {
        lock (_syncRoot)
        {
            if (_keys.Any(x => x.Secret.Equals(key)))
            {
                Console.WriteLine("Такой ключ уже есть");
                return;
            }
            try
            {
                _keys.Add(new ApiKey(key, ValidateKey(key)));
                RewriteKeys(_keys);
            }
            catch (Exception e) { }
        }
    }
    
    static KeyStorageService()
    {
        var keys = File.ReadAllText(KeyPath).Split('\n', StringSplitOptions.RemoveEmptyEntries);
        _keys = ValidateKeys(keys.ToList());
        if (_keys.Count != 0)
        {
            RewriteKeys(_keys);
            return;
        }
        Console.WriteLine("Добавьте ключей через рест");
    }

    private static void RewriteKeys(List<ApiKey> keyList) =>
        File.WriteAllText(KeyPath, string.Join('\n', _keys.Select(x => x.Secret)));
    
    private static List<ApiKey> ValidateKeys(List<string> keys)
    {
        var keyList = new List<ApiKey>();
        foreach (var key in keys)
        {
            try
            {
                keyList.Add(new ApiKey(key, ValidateKey(key)));
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

    private record ApiKey(string Secret, int UsageCount);
}