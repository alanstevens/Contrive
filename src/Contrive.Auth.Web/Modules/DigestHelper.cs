using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;
using System.Web;
using Contrive.Auth.Membership;
using Contrive.Common;
using Contrive.Common.Extensions;
using Microsoft.Practices.ServiceLocation;

namespace Contrive.Auth.Web.Modules
{
  public class DigestHelper
  {
    static DigestHelper()
    {
      _config = ServiceLocator.Current.GetInstance<IConfigurationProvider>();
      _userService = ServiceLocator.Current.GetInstance<IUserServiceExtended>();
      _cryptographer = ServiceLocator.Current.GetInstance<ICryptographer>();
    }

    const string AUTHENTICATION_METHOD_NAME = "HTTPDigest.Components.AuthDigest";
    static readonly IConfigurationProvider _config;
    static readonly IUserServiceExtended _userService;
    static readonly ICryptographer _cryptographer;

    static string GenerateNonce()
    {
      // Now + 3 minutes, encoded base64
      // The nonce validity check will be performed also against the time
      // More strong example of nonce - 
      // use additionally ETag and unique key, which is
      // known by the server
      var expiration = DateTime.UtcNow + TimeSpan.FromMinutes(3);

      var buffer = new ASCIIEncoding().GetBytes(expiration.ToString("G"));

      var nonce = Convert.ToBase64String(buffer);

      // Strip "=" characters added by Base64 encoding 
      // which are forbidden by the server
      nonce = nonce.TrimEnd(new[] {'='});
      return nonce;
    }

    static bool IsValidNonce(string encodedNonce)
    {
      DateTime expiration;
      var padCharCount = encodedNonce.Length%4;

      if (padCharCount > 0) padCharCount = 4 - padCharCount;

      encodedNonce = encodedNonce.PadRight(encodedNonce.Length + padCharCount, '=');

      try
      {
        var buffer = Convert.FromBase64String(encodedNonce);
        var nonce = new ASCIIEncoding().GetString(buffer);
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
      var parts = authHeader.Substring(7).Split(new[] {','});
      var results = new Dictionary<string, string>();
      foreach (var part in parts)
      {
        var subParts = part.Split(new[] {'='}, 2);
        var key = subParts[0].Trim(new[] {' ', '\"'});
        var val = subParts[1].Trim(new[] {' ', '\"'});
        results.Add(key, val);
      }
      return results;
    }

    public static bool Authenticate(HttpApplication app)
    {
      var request = app.Request;
      var context = app.Context;
      var authHeader = request.Headers[AuthenticationModuleBase.RESPONSE_HEADER_NAME].Trim();
      var httpMethod = request.HttpMethod;

      if (authHeader.IsEmpty() || authHeader.IndexOf("Digest", 0) != 0) return false;

      var authHeaderContents = ParseAuthHeader(authHeader);

      var username = authHeaderContents["username"];

      var ha1 = GetDigestFor(username);

      if (ha1.IsEmpty()) return false;

      var digest = GenerateDigest(ha1, authHeaderContents, httpMethod);

      var isNonceStale = !IsValidNonce(authHeaderContents["nonce"]);

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
      var a2 = String.Format("{0}:{1}", httpMethod, authHeaderContents["uri"]);

      // d)
      // HA2 = MD5(A2)
      var ha2 = a2.CalculateMd5Hash().Base64ToHex();

      // e)
      // GENRESPONSE = 
      // HA1 ":" nonce ":" nc ":" cnonce ":" qop ":" HA2
      string genresponse;

      var nonce = authHeaderContents["nonce"];

      if (authHeaderContents["qop"] != null)
      {
        genresponse = String.Format("{0}:{1}:{2}:{3}:{4}:{5}", ha1, nonce, authHeaderContents["nc"],
                                    authHeaderContents["cnonce"], authHeaderContents["qop"], ha2);
      }
      else genresponse = String.Format("{0}:{1}:{2}", ha1, nonce, ha2);

      return genresponse.CalculateMd5Hash().Base64ToHex();
    }

    static string GetDigestFor(string userName)
    {
      var user = _userService.GetUserByUserName(userName);

      if (user.IsNull()) return "";

      return user.AuthDigest;
    }

    public static string BuildChallengeHeader(HttpApplication app)
    {
      var realm = _config.AppSettings["HTTPDigest.Components.AuthDigest_Realm"];
      var opaque = _config.AppSettings["HTTPDigest.Components.AuthDigest_Opaque"];
      var algorithm = _config.AppSettings["HTTPDigest.Components.AuthDigest_Algorithm"];
      var qualityOfProtection = _config.AppSettings["HTTPDigest.Components.AuthDigest_Qop"];

      var nonce = GenerateNonce();

      var isNonceStale = false;

      var staleObj = app.Context.Items["staleNonce"];

      if (staleObj != null) isNonceStale = (bool) staleObj;

      // Show Digest modal window
      // build WWW-Authenticate server response header
      var builder = new StringBuilder("Digest");
      builder.Append(" realm=\"");
      builder.Append(realm);
      builder.Append("\"");
      builder.Append(", nonce=\"");
      builder.Append(nonce);
      builder.Append("\"");
      builder.Append(", opaque=\"");
      builder.Append(opaque);
      builder.Append("\"");
      builder.Append(", stale=");
      builder.Append(isNonceStale ? "true" : "false");
      builder.Append(", algorithm=\"");
      builder.Append(algorithm);
      builder.Append("\"");
      builder.Append(", qop=\"");
      builder.Append(qualityOfProtection);
      builder.Append("\"");

      return builder.ToString();
    }
  }
}