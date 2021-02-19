using System.Collections.Generic;

public static class Util
{
    public static bool TryFindInDictionary<K, V>(this Dictionary<K, V> dictionary, K key, out V val)
    {
        foreach (var entry in dictionary)
        {
            if (entry.Key.Equals(key))
            {
                val = entry.Value;
                return true;
            }
        }

        val = default(V);
        return false;
    }
}