using System;
using System.Web;
using System.Web.Caching;
using Contrive.Common;

namespace Contrive.Web.Common
{
  public class Cache : ICache
  {
    int _lifeSpan = 5;
    public int LifeSpan { get { return _lifeSpan; } set { _lifeSpan = value; } }

    public object Get(string key)
    {
      return HttpContext.Current.Cache.Get(key);
    }

    public object Add(string key, object value)
    {
      var cache = HttpContext.Current.Cache;
      var absoluteExpiration = DateTime.Now.AddMinutes(_lifeSpan);
      var noSlidingExpiration = System.Web.Caching.Cache.NoSlidingExpiration;
      return cache.Add(key, value, null, absoluteExpiration, noSlidingExpiration, CacheItemPriority.Normal, null);
    }
  }
}