using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MemorialAppApi.Core.Helpers
{
    public static class JsonHelper
    {
        // 🔧 Helpers

        public static List<string> Parse(string? json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return [];

            try
            {
                return JsonSerializer.Deserialize<List<string>>(json) ?? [];
            }
            catch
            {
                return [];
            }
        }

        public static List<string> Normalize(IEnumerable<string> urls)
        {
            return urls
                .Where(u => !string.IsNullOrWhiteSpace(u))
                .Select(u => u.Split('?')[0].Trim().ToLowerInvariant()) // 🔥 remove SAS + normalize
                .Distinct()
                .ToList();
        }
    }
}
