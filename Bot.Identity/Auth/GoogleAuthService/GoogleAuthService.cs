using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Core.External.Contracts;
using Core.Models.Contracts;
using Microsoft.Extensions.Configuration;
using Core.Models.Contracts.Google;
using System;

namespace Bot.Identity.Services
{
  public class GoogleAuthService : IGoogleAuthService
  {
    private const string getAccessTokenUrl = "https://oauth2.googleapis.com/token?code={0}&client_id={1}&client_secret={2}&redirect_uri={3}&grant_type=authorization_code";
    private const string tokenValidationUrl = "https://oauth2.googleapis.com/tokeninfo?access_token={0}";
    private const string UserInfoUrl = "https://openidconnect.googleapis.com/v1/userinfo?access_token={0}";

    private readonly string _appId;
    private readonly string _appSecret;
    private readonly string _redirectUri;


    private readonly IHttpClientFactory _httpClientFactory;

    public GoogleAuthService(IConfiguration config, IHttpClientFactory httpClientFactory)
    {
      _appId = config.GetSection("GoogleAuthSettings:AppId").Value;
      _appSecret = config.GetSection("GoogleAuthSettings:AppSecret").Value;
      _redirectUri = config.GetSection("GoogleAuthSettings:RedirectUri").Value;
      _httpClientFactory = httpClientFactory;
    }

    public async Task<GoogleTokenValidationResult> ValidateAccessTokenAsync(string accessToken)
    {
      var formattedUrl = string.Format(tokenValidationUrl, accessToken);
      var result = await _httpClientFactory.CreateClient().GetAsync(formattedUrl);
      result.EnsureSuccessStatusCode();
      var responseAsString = await result.Content.ReadAsStringAsync();
      return JsonConvert.DeserializeObject<GoogleTokenValidationResult>(responseAsString);
    }

    public async Task<GoogleUserInfoResult> GetUserInfoAsync(string accessToken)
    {
      var formattedUrl = string.Format(UserInfoUrl, accessToken);
      var result = await _httpClientFactory.CreateClient().GetAsync(formattedUrl);
      result.EnsureSuccessStatusCode();
      var responseAsString = await result.Content.ReadAsStringAsync();
      return JsonConvert.DeserializeObject<GoogleUserInfoResult>(responseAsString);
    }

    public async Task<AccesssTokenResponse> GetAccessTokenAsync(string code)
    {

      // TODO: refactor AccesssTokenResponse object for google fields
      var formattedUrl = string.Format(getAccessTokenUrl, code, _appId, _appSecret, _redirectUri);
      var client = new HttpClient();
      var response = await client.PostAsync(formattedUrl, null);

      response.EnsureSuccessStatusCode();
      var responseAsString = await response.Content.ReadAsStringAsync();
      Console.WriteLine(responseAsString);

      return JsonConvert.DeserializeObject<AccesssTokenResponse>(responseAsString);


    }
  }
}