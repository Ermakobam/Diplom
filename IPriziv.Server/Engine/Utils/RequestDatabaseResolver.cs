using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace IPriziv.Server.Engine.Utils
{
    /// <summary>
    /// Выбор сезонной БД для запроса: query database/DataBase → header DataBase → cookie DataBase.
    /// </summary>
    public static class RequestDatabaseResolver
    {
        public static string Resolve(IQueryCollection query, IHeaderDictionary headers, IRequestCookieCollection cookies)
        {
            return Resolve(
                FirstNonEmpty(query, "database", "DataBase"),
                FirstNonEmpty(headers, "DataBase"),
                FirstNonEmpty(cookies, "DataBase"));
        }

        public static string Resolve(string queryDatabase, string headerDatabase, string cookieDatabase)
        {
            return FirstNonEmpty(queryDatabase, headerDatabase, cookieDatabase);
        }

        private static string FirstNonEmpty(IQueryCollection query, params string[] keys)
        {
            foreach (var key in keys)
            {
                if (query.TryGetValue(key, out var values))
                {
                    var normalized = Normalize(values);
                    if (normalized != null)
                        return normalized;
                }
            }

            return null;
        }

        private static string FirstNonEmpty(IHeaderDictionary headers, string key)
        {
            if (!headers.TryGetValue(key, out var values))
                return null;
            return Normalize(values);
        }

        private static string FirstNonEmpty(IRequestCookieCollection cookies, string key)
        {
            if (!cookies.TryGetValue(key, out var value))
                return null;
            return Normalize(value);
        }

        private static string FirstNonEmpty(params string[] values)
        {
            foreach (var value in values)
            {
                var normalized = Normalize(value);
                if (normalized != null)
                    return normalized;
            }

            return null;
        }

        private static string Normalize(StringValues values)
        {
            if (StringValues.IsNullOrEmpty(values))
                return null;

            // Query keys are case-insensitive: database/DataBase may merge into one StringValues.
            foreach (var value in values)
            {
                var normalized = Normalize(value);
                if (normalized != null)
                    return normalized;
            }

            return null;
        }

        private static string Normalize(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;
            return value.Trim();
        }
    }
}
