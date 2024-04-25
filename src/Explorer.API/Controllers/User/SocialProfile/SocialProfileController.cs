using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using FluentResults;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace Explorer.API.Controllers.User.SocialProfile
{
    [Authorize(Policy = "userPolicy")]
    [Route("api/social-profile")]
    public class SocialProfileController : BaseApiController
    {
        private readonly ISocialProfileService _userProfileService;
        private readonly HttpClient _httpClient;
        private readonly IUserService _userService;


        public SocialProfileController(ISocialProfileService userProfileService, IUserService userService)
        {
            _userProfileService = userProfileService;
            _httpClient = new HttpClient();
            _userService = userService;
        }

        [HttpPost("follow/{followerId:int}/{followedId:int}")]
        public async  Task<ActionResult<SocialProfileDto>> Follow(int followerId, int followedId)
        {
            /*

            var result = _userProfileService.Follow(followerId, followedId);

            return CreateResponse(result);
            */
            var microserviceUrl = "http://localhost:8082";
            var requestUrl = $"{microserviceUrl}/update/{followerId}/{followedId}";
            FollowerDto dto = new FollowerDto();
            dto.Id=followerId;
            dto.Followed = new List<int>();
            dto.Followed.Add(followedId);
            SocialProfileDto socialDto = new SocialProfileDto();

            try
            {
                // Serijalizujte DTO objekat u JSON
                var json = JsonConvert.SerializeObject(dto);
                var data = new StringContent(json, Encoding.UTF8, "application/json");

                // Pošaljite HTTP POST zahtjev ka mikroservisu
                var response = await _httpClient.PutAsync(requestUrl,null);

                // Provjerite odgovor
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"HTTP status successful: {response.StatusCode}");
                    // Dobijanje JSON odgovora
                    var jsonResponse = await response.Content.ReadAsStringAsync();

                    // Deserijalizacija JSON odgovora u odgovarajući DTO objekat
                    var followerDto = JsonConvert.DeserializeObject<FollowerDto>(jsonResponse);
                    return Ok(); // Vratite status kod 200 OK ako je brisanje uspješno
                }
                else
                {
                    // Ukoliko je odgovor neuspješan, obradite grešku na odgovarajući način
                    Console.WriteLine($"HTTP request failed with status code: {response.StatusCode}");
                    return StatusCode((int)response.StatusCode); // Vratite odgovarajući status kod u zavisnosti od odgovora mikroservisa
                }
            }
            catch (Exception ex)
            {
                // Uhvatite i obradite izuzetak ako se desi
                Console.WriteLine($"An error occurred: {ex.Message}");
                return StatusCode(500); // Vratite status kod 500 Internal Server Error ako se desi izuzetak
            }
        }


    

        [HttpPost("un-follow/{followerId:int}/{unFollowedId:int}")]
        public ActionResult<SocialProfileDto> UnFollow(int followerId, int unFollowedId)
        {
            var result = _userProfileService.UnFollow(followerId, unFollowedId);

            return CreateResponse(result);
        }

        [HttpGet("get/{userId:int}")]
        public async  Task<ActionResult<SocialProfileDto>> GetSocialProfile(int userId)
        {
            /*
            var socialProfile = _userProfileService.Get(userId);

            return CreateResponse(socialProfile);
            */
            var microserviceUrl = "http://localhost:8082";
            var requestUrl = $"{microserviceUrl}/getById/{userId}";
            SocialProfileDto socialDto = new SocialProfileDto();
            List<UserDto> allUsers = new List<UserDto>();
            allUsers = _userService.GetPaged(0, 0).Value.Results;
            var currentLogged = _userService.GetUserById(userId).Value;

            try
            {
                // Pošaljite HTTP GET zahtjev ka mikroservisu
                var response = await _httpClient.GetAsync(requestUrl);

                // Provjerite odgovor
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"HTTP status successful: {response.StatusCode}");
                    var json = await response.Content.ReadAsStringAsync();
                    var followerdto = JsonConvert.DeserializeObject<FollowerDto>(json);
                    socialDto.Id = followerdto.Id;
                    socialDto.Followed = new List<UserDto>();
                    socialDto.Followers = new List<UserDto>();
                    socialDto.Followable = new List<UserDto>();

                    if (followerdto.Followed != null) {
                        foreach (int obj in followerdto.Followed)
                        {
                            var userdto = _userService.GetUserById(obj).Value;
                            socialDto.Followed.Add(userdto);
                        }
                    }
                    

                    if (followerdto.Followers!=null) {
                        foreach (int obj in followerdto.Followers)
                        {
                            var userdto = _userService.GetUserById(obj).Value;
                            socialDto.Followers.Add(userdto);
                        }
                    }

                    if (followerdto.Followable != null)
                    {
                        foreach (int obj in followerdto.Followable)
                        {
                            var userdto = _userService.GetUserById(obj).Value;
                            socialDto.Followable.Add(userdto);
                        }
                    }


                    return socialDto;
                    
                }
                else
                {
                    // Ukoliko je odgovor neuspješan, obradite grešku na odgovarajući način
                    Console.WriteLine($"HTTP request failed with status code: {response.StatusCode}");
                    return null; // Vratite odgovarajući status kod u zavisnosti od odgovora mikroservisa
                }
            }
            catch (Exception ex)
            {
                // Uhvatite i obradite izuzetak ako se desi
                Console.WriteLine($"An error occurred: {ex.Message}");
                return null; ; // Vratite status kod 500 Internal Server Error ako se desi izuzetak
            }
        }
    }
}