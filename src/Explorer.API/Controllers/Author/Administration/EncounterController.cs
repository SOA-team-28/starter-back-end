using Explorer.API.Services;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Encounters.API.Dtos;
using Explorer.Encounters.API.Public;
using Explorer.Encounters.Core.Domain.Encounters;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Explorer.Tours.API.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using System.Net.Http;
using System.Text;

using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Azure;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace Explorer.API.Controllers.Author.Administration
{
    [Route("api/administration/encounter")]
    [Authorize(Policy = "authorPolicy")]


    public class EncounterController : BaseApiController
    {
        private readonly IEncounterService _encounterService;
        private readonly ImageService _imageService;
        private readonly HttpClient _httpClient;


        public EncounterController(IEncounterService encounterService)
        {
            _encounterService = encounterService;
            _imageService = new ImageService();
            _httpClient = new HttpClient();

        }
        

        [HttpPost]
        public async Task<ActionResult<Boolean>> Create([FromForm] EncounterDto encounter, [FromQuery] int checkpointId, [FromQuery] bool isSecretPrerequisite, [FromForm] List<IFormFile>? imageF = null)
        {
            /*

            if (imageF != null && imageF.Any())
            {
                var imageNames = _imageService.UploadImages(imageF);
                if (encounter.Type =="Location")
                    encounter.Image = imageNames[0];
            }

            var result = _encounterService.Create(encounter, checkpointId, isSecretPrerequisite,User.PersonId());
            return CreateResponse(result);
            */

            Console.WriteLine($"Objekat sa fronta {encounter}");

            encounter.CheckPointId = checkpointId;

            var microserviceUrl = "http://localhost:8082";

            try
            {
                // Serijalizujte EncounterDto objekat u JSON format
                var jsonContent = JsonConvert.SerializeObject(encounter);

                // Kreirajte HTTP zahtjev sa JSON sadržajem
                var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                // Pošaljite HTTP POST zahtjev ka Go mikroservisu
                var response = await _httpClient.PostAsync($"{microserviceUrl}/encounters", httpContent);

                // Provjerite odgovor
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"HTTP status successfull {response.StatusCode}");
                    return true;
                }
                else
                {
                    // Ukoliko je odgovor neuspješan, obradite grešku na odgovarajući način
                    Console.WriteLine($"HTTP request failed with status code {response.StatusCode}");
                    return false;

                }
            }
            catch (Exception ex)
            {
                // Uhvatite i obradite izuzetak ako se desi
                Console.WriteLine($"An error occurred: {ex.Message}");
                return false;

            }
        }

    
     

        [HttpPut]
        public async Task< ActionResult<EncounterDto>> Update([FromForm] EncounterDto encounter, [FromForm] List<IFormFile>? imageF = null)
        {
            /*

            if (imageF != null && imageF.Any())
            {
                var imageNames = _imageService.UploadImages(imageF);
                if (encounter.Type == "Location") ;
                   // encounter.Image = imageNames[0];
            }

            var result = _encounterService.Update(encounter,User.PersonId());
            return CreateResponse(result);
            */

            var microserviceUrl = "http://localhost:8082";

            try
            {
                // Serijalizujte EncounterDto objekat u JSON format
                var jsonContent = JsonConvert.SerializeObject(encounter);

                // Kreirajte HTTP zahtjev sa JSON sadržajem
                var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                // Pošaljite HTTP POST zahtjev ka Go mikroservisu
                var response = await _httpClient.PutAsync($"{microserviceUrl}/encounters/update", httpContent);

                // Provjerite odgovor
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"HTTP status successfull {response.StatusCode}");
                    return encounter;
                }
                else
                {
                    // Ukoliko je odgovor neuspješan, obradite grešku na odgovarajući način
                    Console.WriteLine($"HTTP request failed with status code {response.StatusCode}");
                    return encounter;

                }
            }
            catch (Exception ex)
            {
                // Uhvatite i obradite izuzetak ako se desi
                Console.WriteLine($"An error occurred: {ex.Message}");
                return encounter;

            }
        }

        [HttpDelete("{id:int}")]
        public ActionResult Delete(int id)
        {
            var result = _encounterService.Delete(id, User.PersonId());
            return CreateResponse(result);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<EncounterDto>> GetById(int id)
        {
            /*
            var result = _encounterService.Get(id);
            return CreateResponse(result);

            */
            var microserviceUrl = "http://localhost:8082";
            try
            {
                // Napravite HTTP GET zahtjev ka mikroservisu za dobavljanje encounter-a po ID-u
                var response = await _httpClient.GetAsync($"{microserviceUrl}/encounters/getById/{id}");

                // Provjerite odgovor
                if (response.IsSuccessStatusCode)
                {
                    // Ukoliko je odgovor uspješan, izvucite podatke iz odgovora
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var retrievedEncounter = JsonConvert.DeserializeObject<EncounterDto>(jsonString);

                    Console.WriteLine($"Encounter retrieved successfully. ID: {retrievedEncounter.Id}");
                    return retrievedEncounter;
                }
                else
                {
                    // Ukoliko je odgovor neuspješan, obradite grešku na odgovarajući način
                    Console.WriteLine($"HTTP request failed with status code {response.StatusCode}");
                    return null; // Vratite null ili neku drugu indikaciju da je dobavljanje encounter-a neuspješno
                }
            }
            catch (Exception ex)
            {
                // Uhvatite i obradite izuzetak ako se desi
                Console.WriteLine($"An error occurred: {ex.Message}");
                return null; // Vratite null ili neku drugu indikaciju o grešci
            }


        }
        [HttpGet("getByCheckPoint/{chId:int}")]
        public async Task<ActionResult<EncounterDto>> GetByCheckPoint([FromRoute]int chId)
        {
            /*
            var result = _encounterService.Get(id);
            return CreateResponse(result);

            */
            EncounterDto retrievedEncounter = new EncounterDto();
            var microserviceUrl = "http://localhost:8082";
            try
            {
                // Napravite HTTP GET zahtjev ka mikroservisu za dobavljanje encounter-a po ID-u
                var response = await _httpClient.GetAsync($"{microserviceUrl}/encounters/getByCheckPoint/{chId}");

                // Provjerite odgovor
                if (response.IsSuccessStatusCode)
                {
                    // Ukoliko je odgovor uspješan, izvucite podatke iz odgovora
                    var jsonString = await response.Content.ReadAsStringAsync();
                     retrievedEncounter = JsonConvert.DeserializeObject<EncounterDto>(jsonString);

                    Console.WriteLine($"Encounter retrieved successfully. ID: {retrievedEncounter.Id}");
                    return retrievedEncounter;
                }
                else
                {
                    // Ukoliko je odgovor neuspješan, obradite grešku na odgovarajući način
                    Console.WriteLine($"HTTP request failed with status code {response.StatusCode}");
                    return retrievedEncounter; // Vratite null ili neku drugu indikaciju da je dobavljanje encounter-a neuspješno
                }
            }
            catch (Exception ex)
            {
                // Uhvatite i obradite izuzetak ako se desi
                Console.WriteLine($"An error occurred: {ex.Message}");
                return retrievedEncounter; // Vratite null ili neku drugu indikaciju o grešci
            }


        }
    }
}
