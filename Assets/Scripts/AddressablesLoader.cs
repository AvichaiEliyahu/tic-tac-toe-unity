using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public static class AddressablesLoader
{
    // decided to use caching here to prevent unecessary loads.
    private static readonly Dictionary<string, Sprite> _spriteCache = new();

    public static async UniTask<Sprite> LoadSpriteAsync(string address)
    {
        if (_spriteCache.TryGetValue(address, out var sprite))
        {
            return sprite;
        }
        var op = Addressables.LoadAssetAsync<Sprite>(address);
        await op.Task;

        if(op.Status == AsyncOperationStatus.Succeeded)
        {
            var loadedSprite = op.Result;
            _spriteCache[address] = loadedSprite;
            return loadedSprite;
        }
        else
        {
            Debug.LogError($"Failed to load sprite at address: {address}");
            return null;
        }
    }

    public static void ReleaseSprite(string address)
    {
        if(_spriteCache.TryGetValue(address, out var sprite))
        {
            Addressables.Release(sprite);
            _spriteCache.Remove(address);
        }
    }
}
