using System.Text.Json;

namespace MemorialAppApi.Core.Helpers
{
    public static class MediaHelper
    {
        public static List<string> ExtractUrls(string? mediaJson)
        {
            var result = new List<string>();

            if (string.IsNullOrWhiteSpace(mediaJson))
                return result;

            try
            {
                var dict = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(mediaJson);

                if (dict == null)
                    return result;

                foreach (var value in dict.Values)
                {
                    if (value.ValueKind == JsonValueKind.Null)
                        continue;

                    // CASE 1: already array
                    if (value.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var item in value.EnumerateArray())
                        {
                            if (item.ValueKind == JsonValueKind.String)
                                result.Add(item.GetString()!);
                        }
                    }
                    // CASE 2: string → may be JSON array
                    else if (value.ValueKind == JsonValueKind.String)
                    {
                        var str = value.GetString();

                        if (string.IsNullOrWhiteSpace(str))
                            continue;

                        try
                        {
                            var urls = JsonSerializer.Deserialize<List<string>>(str);
                            if (urls != null)
                                result.AddRange(urls);
                        }
                        catch
                        {
                            // fallback: plain string
                            result.Add(str);
                        }
                    }
                }
            }
            catch
            {
                // corrupted JSON fallback
                return result;
            }

            return result.Distinct().ToList();
        }
    }
}