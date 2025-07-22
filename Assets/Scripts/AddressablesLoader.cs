using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

/// <summary>
/// Helper class to load sprites from addressables
/// </summary>
public static class AddressablesLoader
{
    // decided to use caching here to prevent unecessary loads.
    private static readonly Dictionary<string, Sprite> _spriteCache = new();

    /// <summary>
    /// Loads a sprite
    /// </summary>
    /// <param name="address">Sprite addressables address</param>
    /// <returns>Awaitable with the sprite</returns>
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

    /// <summary>
    /// Realease the spritre at a given address
    /// </summary>
    /// <param name="address">Addressables address of sprite to release.</param>
    public static void ReleaseSprite(string address)
    {
        if(_spriteCache.TryGetValue(address, out var sprite))
        {
            Addressables.Release(sprite);
            _spriteCache.Remove(address);
        }
    }
}
