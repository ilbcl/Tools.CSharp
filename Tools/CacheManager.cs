using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;

namespace Tools
{
    public class CacheManager
    {
        private static readonly object _synchRoot = new object();
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Получение объекта из кеша по указанному ключу.
        /// </summary>
        /// <typeparam name="T">Тип объекта, который нужно получить из кеша.</typeparam>
        /// <param name="key">Ключ, по которому был ранее сохранен объект в кеше.</param>
        /// <returns>Объект из кеша, либо <b>null</b> если объект не найден.</returns>
        public static T GetItemFromCache<T>(string key)
        {
            try
            {
                var cache = MemoryCache.Default;
                object value = cache[key];
                return Utility.ConvertToTypeOf<T>(value);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Ошибка получения данных из кеша по ключу '{key}' для объекта типа '{typeof(T)}'");
                return default(T);
            }
        }

        /// <summary>
        /// Получение объекта из кеша по идентификатору для указанного пользователя.
        /// </summary>
        /// <typeparam name="T">Тип объекта, который нужно получить из кеша.</typeparam>
        /// <typeparam name="TK">Тип идентификатора объекта, который нужно получить из кеша.</typeparam>
        /// <param name="currentUserId">Текущий пользователь.</param>
        /// <param name="id">Идентификатор объекта, который нужно найти в кеше.</param>
        /// <param name="suffix">Суффикс, который изпользовался при сохранении объекта в кеше. Опционально.</param>
        /// <returns>Объект из кеша, либо <b>null</b> если объект не найден.</returns>
        public static T GetItemFromCache<T, TK>(Guid currentUserId, TK id, string suffix = null)
        {
            string key = GetCacheKey(currentUserId, id, suffix);
            return GetItemFromCache<T>(key);
        }

        /// <summary>
        /// Получение объекта из кеша по идентификатору.
        /// </summary>
        /// <typeparam name="T">Тип объекта, который нужно получить из кеша.</typeparam>
        /// <typeparam name="TK">Тип идентификатора объекта, который нужно получить из кеша.</typeparam>
        /// <param name="id">Идентификатор объекта, который нужно найти в кеше.</param>
        /// <param name="suffix">Суффикс, который изпользовался при сохранении объекта в кеше. Опционально.</param>
        /// <returns>Объект из кеша, либо <b>null</b> если объект не найден.</returns>
        public static T GetItemFromCache<T, TK>(TK id, string suffix = null)
        {
            string key = GetCacheKey(id, suffix);
            return GetItemFromCache<T>(key);
        }

        /// <summary>
        /// Получение списка объектов из кеша по идентификаторам для указанного пользователя.
        /// </summary>
        /// <typeparam name="T">Тип объекта, который нужно получить из кеша.</typeparam>
        /// <typeparam name="TK">Тип идентификатора объекта, который нужно получить из кеша.</typeparam>
        /// <param name="currentUserId">Текущий пользователь.</param>
        /// <param name="idsList">Список идентификаторов объектов, которые нужно получить из кеша.</param>
        /// <param name="suffix">Суффикс, который изпользовался при сохранении объекта в кеше. Опционально.</param>
        /// <returns>Список найденных объектов из кеша и список идентификаторов объектов, которые отсутствуют в кеше.</returns>
        public static CachedItemsSearchResult<T, TK> GetItemsFromCacheByIds<T, TK>(Guid currentUserId, IEnumerable<TK> idsList, string suffix = null)
        {
            var ids = idsList.Distinct().ToArray();
            if (!ids.Any())
            {
                return new CachedItemsSearchResult<T, TK> { CachedItems = new List<T>(), NotCachedItemsIds = new List<TK>() };
            }

            List<T> cachedItems = new List<T>();
            List<TK> notCachedItemIds = new List<TK>();
            foreach (TK id in ids)
            {
                var cachedItem = GetItemFromCache<T, TK>(currentUserId, id, suffix);
                if (cachedItem != null)
                {
                    cachedItems.Add(cachedItem);
                }
                else
                {
                    notCachedItemIds.Add(id);
                }
            }

            return new CachedItemsSearchResult<T, TK>{ CachedItems = cachedItems, NotCachedItemsIds = notCachedItemIds };
        }

        /// <summary>
        /// Получение списка объектов из кеша по идентификаторам.
        /// </summary>
        /// <typeparam name="T">Тип объекта, который нужно получить из кеша.</typeparam>
        /// <typeparam name="TK">Тип идентификатора объекта, который нужно получить из кеша.</typeparam>
        /// <param name="idsList">Список идентификаторов объектов, которые нужно получить из кеша.</param>
        /// <param name="suffix">Суффикс, который изпользовался при сохранении объекта в кеше. Опционально.</param>
        /// <returns>Список найденных объектов из кеша и список идентификаторов объектов, которые отсутствуют в кеше.</returns>
        public static CachedItemsSearchResult<T, TK> GetItemsFromCacheByIds<T, TK>(IEnumerable<TK> idsList, string suffix = null)
        {
            var ids = idsList.Distinct().ToArray();
            if (!ids.Any())
            {
                return new CachedItemsSearchResult<T, TK> { CachedItems = new List<T>(), NotCachedItemsIds = new List<TK>() };
            }

            List<T> cachedItems = new List<T>();
            List<TK> notCachedItemIds = new List<TK>();
            foreach (TK id in ids)
            {
                var cachedItem = GetItemFromCache<T, TK>(id, suffix);
                if (cachedItem != null)
                {
                    cachedItems.Add(cachedItem);
                }
                else
                {
                    notCachedItemIds.Add(id);
                }
            }

            return new CachedItemsSearchResult<T, TK>{ CachedItems = cachedItems, NotCachedItemsIds = notCachedItemIds };
        }

        /// <summary>
        /// Добавление объекта в кеш по указанному ключу.
        /// </summary>
        /// <typeparam name="T">Тип объекта, который нужно добавить в кеш.</typeparam>
        /// <param name="key">Ключ, по которому необходимо сохранить объект в кеше.</param>
        /// <param name="item">Объект, который необходимо сохранить в кеше.</param>
        /// <param name="durationInSeconds">Продролжительность хранения закешированного объекта.</param>
        /// <param name="synchronize">Указывает, нужно ли использовать механизм синхронизации при добавлении объекта в кеш.</param>
        /// <param name="useSlidingExpiration"></param>
        public static void AddItemToCache<T>(string key, T item, int durationInSeconds = 30, bool synchronize = false, bool useSlidingExpiration = true)
        {
            if (!string.IsNullOrWhiteSpace(key) && item != null)
            {
                try
                {
                    var cachePolicy = new CacheItemPolicy();
                    if (useSlidingExpiration)
                    {
                        cachePolicy.SlidingExpiration = TimeSpan.FromSeconds(durationInSeconds);
                    }
                    else
                    {
                        cachePolicy.AbsoluteExpiration = DateTimeOffset.UtcNow.AddSeconds(durationInSeconds);
                    }

                    if (synchronize)
                    {
                        lock (_synchRoot)
                        {
                            var cache = MemoryCache.Default;
                            cache.Set(key, item, cachePolicy);
                        }
                    }
                    else
                    {
                        var cache = MemoryCache.Default;
                        cache.Set(key, item, cachePolicy);
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, $"Ошибка сохранения данных в кеше по ключу '{key}' для объекта типа '{typeof(T)}' {(synchronize ? "с синхронизацией" : "без синхронизации")}");
                }
            }
        }

        /// <summary>
        /// Добавление объекта в кеш по идентификатору объекта для указанного пользователя.
        /// </summary>
        /// <typeparam name="T">Тип объекта, который нужно добавить в кеш.</typeparam>
        /// <typeparam name="TK">Тип идентификатора объекта, который нужно добавить в кеш.</typeparam>
        /// <param name="currentUserId">Текущий пользователь.</param>
        /// <param name="suffix">Суффикс для формирования ключа кеша. Опционально.</param>
        /// <param name="id">Идентификатор объекта, который нужно добавить в кеш.</param>
        /// <param name="item">Объект, который нужно добавить в кеш.</param>
        /// <param name="durationInSeconds">Продролжительность хранения закешированного объекта.</param>
        /// <param name="synchronize">Указывает, нужно ли использовать механизм синхронизации при добавлении объекта в кеш.</param>
        /// <param name="useSlidingExpiration"></param>
        public static void AddItemToCache<T, TK>(Guid currentUserId, TK id, T item, string suffix = null,
                                                 int durationInSeconds = 30, bool synchronize = false, bool useSlidingExpiration = true)
        {
            string key = GetCacheKey(currentUserId, id, suffix);
            AddItemToCache(key, item, durationInSeconds, synchronize, useSlidingExpiration);
        }

        /// <summary>
        /// Добавление объекта в кеш по идентификатору объекта.
        /// </summary>
        /// <typeparam name="T">Тип объекта, который нужно добавить в кеш.</typeparam>
        /// <typeparam name="TK">Тип идентификатора объекта, который нужно добавить в кеш.</typeparam>
        /// <param name="suffix">Суффикс для формирования ключа кеша. Опционально.</param>
        /// <param name="id">Идентификатор объекта, который нужно добавить в кеш.</param>
        /// <param name="item">Объект, который нужно добавить в кеш.</param>
        /// <param name="durationInSeconds">Продролжительность хранения закешированного объекта.</param>
        /// <param name="synchronize">Указывает, нужно ли использовать механизм синхронизации при добавлении объекта в кеш.</param>
        /// <param name="useSlidingExpiration"></param>
        public static void AddItemToCache<T, TK>(TK id, T item, string suffix = null, int durationInSeconds = 30, bool synchronize = false, bool useSlidingExpiration = true)
        {
            string key = GetCacheKey(id, suffix);
            AddItemToCache(key, item, durationInSeconds, synchronize, useSlidingExpiration);
        }

        /// <summary>
        /// Удаление объекта из кеша по указанному ключу.
        /// </summary>
        /// <typeparam name="T">Тип объекта, который нужно удалить из кеша.</typeparam>
        /// <param name="key">Ключ, по которому был ранее сохранен объект в кеше.</param>
        /// <returns>Объект из кеша, либо <b>null</b> если объект не найден.</returns>
        public static T RemoveItemFromCache<T>(string key)
        {
            try
            {
                var cache = MemoryCache.Default;
                object value = cache.Remove(key);
                return Utility.ConvertToTypeOf<T>(value);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Ошибка удаления данных из кеша по ключу '{key}' для объекта типа '{typeof(T)}'");
                return default(T);
            }
        }

        /// <summary>
        /// Удаление объекта из кеша по идентификатору для указанного пользователя.
        /// </summary>
        /// <typeparam name="T">Тип объекта, который нужно удалить из кеша.</typeparam>
        /// <typeparam name="TK">Тип идентификатора объекта, который нужно удалить из кеша.</typeparam>
        /// <param name="currentUserId">Текущий пользователь.</param>
        /// <param name="id">Идентификатор объекта, который нужно удалить из кеша.</param>
        /// <param name="suffix">Суффикс, который изпользовался при сохранении объекта в кеше. Опционально.</param>
        /// <returns>Объект из кеша, либо <b>null</b> если объект не найден.</returns>
        public static T RemoveItemFromCache<T, TK>(Guid currentUserId, TK id, string suffix = null)
        {
            string key = GetCacheKey(currentUserId, id, suffix);
            return GetItemFromCache<T>(key);
        }

        /// <summary>
        /// Удаление объекта из кеша по идентификатору.
        /// </summary>
        /// <typeparam name="T">Тип объекта, который нужно удалить из кеша.</typeparam>
        /// <typeparam name="TK">Тип идентификатора объекта, который нужно удалить из кеша.</typeparam>
        /// <param name="id">Идентификатор объекта, который нужно удалить из кеша.</param>
        /// <param name="suffix">Суффикс, который изпользовался при сохранении объекта в кеше. Опционально.</param>
        /// <returns>Объект из кеша, либо <b>null</b> если объект не найден.</returns>
        public static T RemoveItemFromCache<T, TK>(TK id, string suffix = null)
        {
            string key = GetCacheKey(id, suffix);
            return GetItemFromCache<T>(key);
        }

        private static string GetCacheKey<TK>(Guid currentUserId, TK id, string suffix)
        {
            return $"{currentUserId}{suffix}{id}";
        }

        private static string GetCacheKey<TK>(TK id, string suffix)
        {
            return $"{suffix}{id}";
        }
    }
}