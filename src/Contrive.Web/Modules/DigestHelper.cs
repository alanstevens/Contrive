using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;
using System.Web;
using Contrive.Core;
using Contrive.Core.Extensions;
using Microsoft.Practices.ServiceLocation;

namespace Contrive.Web.Modules
{
  public class DigestHelper
  {
    static readonly IConfigurationProvider _config;
    static readonly IUserService _userAuth;
    static readonly ICryptographer _cryptographer;

    static DigestHelper()
    {
      _config = ServiceLocator.Current.GetInstance<IConfigurationProvider>();
      _userAuth = ServiceLocator.Current.GetInstance<IUserService>();
      _cryptographer = ServiceLocator.Current.GetInstance<ICryptographer>();
    }

    const string AUTHENTICATION_METHOD_NAME = "HTTPDigest.Components.AuthDigest";

    static string GenerateNonce()
    {
      // Now + 3 minutes, encoded base64
      // The nonce validity check will be performed also against the time
      // More strong example of nonce - 
      // use additionally ETag and unique key, which is
      // known by the server
      DateTime expiration = DateTime.UtcNow + TimeSpan.FromMinutes(3);

      byte[] buffer = new ASCIIEncoding().GetBytes(expiration.ToString("G"));

      string nonce = Convert.ToBase64String(buffer);

      // Strip "=" characters added by Base64 encoding 
      // which are forbidden by the server
      nonce = nonce.TrimEnd(new[] { '=' });
      return nonce;
    }

    static bool IsValidNonce(string encodedNonce)
    {
      DateTime expiration;
      int padCharCount = encodedNonce.Length % 4;

      if (padCharCount > 0)
        padCharCount = 4 - padCharCount;

      encodedNonce = encodedNonce.PadRight(encodedNonce.Length + padCharCount, '=');

      try
      {
        byte[] buffer = Convert.FromBase64String(encodedNonce);
        string nonce = new ASCIIEncoding().GetString(buffer);
        expiration = DateTime.Parse(nonce);
      }
      catch (FormatException)
      {
        return false;
      }
      return (expiration >= DateTime.UtcNow);
    }

    static Dictionary<string, string> ParseAuthHeader(string authHeader)
    {
      string[] parts = authHeader.Substring(7).Split(new[] { ',' });
      var results = new Dictionary<string, string>();
      foreach (string part in parts)
      {
        string[] subParts = part.Split(new[] { '=' }, 2);
        string key = subParts[0].Trim(new[] { ' ', '\"' });
        string val = subParts[1].Trim(new[] { ' ', '\"' });
        results.Add(key, val);
      }
      return results;
    }

    public static bool Authenticate(HttpApplication app)
    {
      var request = app.Request;
      var context = app.Context;
      string authHeader = request.Headers[AuthenticationModuleBase.RESPONSE_HEADER_NAME].Trim();
      var httpMethod = request.HttpMethod;

      if (authHeader.IsEmpty() || authHeader.IndexOf("Digest", 0) != 0)
        return false;

      var authHeaderContents = ParseAuthHeader(authHeader);

      var username = authHeaderContents["username"];

      string ha1 = GetDigestFor(username);

      if (ha1.IsEmpty()) return false;

      string digest = GenerateDigest(ha1, authHeaderContents, httpMethod);

      bool isNonceStale = !IsValidNonce(authHeaderContents["nonce"]);

      context.Items["staleNonce"] = isNonceStale;

      var authResponse = authHeaderContents["response"];

      if (!isNonceStale && authResponse == digest)
      {
        var identity = new GenericIdentity(username, AUTHENTICATION_METHOD_NAME);
        context.User = new GenericPrincipal(identity, null);
        return true;
      }
      return false;
    }

    static string GenerateDigest(string ha1, Dictionary<string, string> authHeaderContents, string httpMethod)
    {
      // see Step #5 of the Digest algorithm

      // c)
      // A2 = HTTP Method ":" digest-uri-value
      string a2 = String.Format("{0}:{1}", httpMethod, authHeaderContents["uri"]);

      // d)
      // HA2 = MD5(A2)
      string ha2 = _cryptographer.ComputeMd5HashAsHex(a2);

      // e)
      // GENRESPONSE = 
      // HA1 ":" nonce ":" nc ":" cnonce ":" qop ":" HA2
      string genresponse;

      var nonce = authHeaderContents["nonce"];

      if (authHeaderContents["qop"] != null)
      {
        genresponse = String.Format("{0}:{1}:{2}:{3}:{4}:{5}",
                                    ha1,
                                    nonce,
                                    authHeaderContents["nc"],
                                    authHeaderContents["cnonce"],
                                    authHeaderContents["qop"],
                                    ha2);
      }
      else
        genresponse = String.Format("{0}:{1}:{2}", ha1, nonce, ha2);

      return _cryptographer.ComputeMd5HashAsHex(genresponse);
    }

    static string GetDigestFor(string userName)
    {
      var user = _userAuth.GetUser(userName);

      if (user.IsNull()) return "";

      return user.AuthDigest;
    }

    public static string BuildChallengeHeader(HttpApplication app)
    {
      var realm = _config.AppSettings["HTTPDigest.Components.AuthDigest_Realm"];
      var opaque = _config.AppSettings["HTTPDigest.Components.AuthDigest_Opaque"];
      var algorithm = _config.AppSettings["HTTPDigest.Components.AuthDigest_Algorithm"];
      var qualityOfProtection = _config.AppSettings["HTTPDigest.Components.AuthDigest_Qop"];

      string nonce = GenerateNonce();

      bool isNonceStale = false;

      object staleObj = app.Context.Items["staleNonce"];

      if (staleObj != null)
        isNonceStale = (bool)staleObj;

      // Show Digest modal window
      // build WWW-Authenticate server response header
      var builder = new StringBuilder("Digest");
      builder.Append(" realm=\"");
      builder.Append((string)realm);
      builder.Append("\"");
      builder.Append(", nonce=\"");
      builder.Append(nonce);
      builder.Append("\"");
      builder.Append(", opaque=\"");
      builder.Append((string)opaque);
      builder.Append("\"");
      builder.Append(", stale=");
      builder.Append(isNonceStale ? "true" : "false");
      builder.Append(", algorithm=\"");
      builder.Append((string)algorithm);
      builder.Append("\"");
      builder.Append(", qop=\"");
      builder.Append((string)qualityOfProtection);
      builder.Append("\"");

      return builder.ToString();
    }
  }
}