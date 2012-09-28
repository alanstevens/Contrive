namespace Contrive.Common.Web
{
  public interface ICache
  {
    object Get(string key);

    object Add(string key, object content);
  }
}