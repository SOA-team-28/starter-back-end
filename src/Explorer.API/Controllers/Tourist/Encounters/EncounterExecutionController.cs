using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Encounters.API.Dtos;
using Explorer.Encounters.API.Public;
using Explorer.Encounters.Core.Domain.Encounters;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;
using System.Net.Http;
using System.Text;

using System.Net.Http.Json;
using System.Threading.Tasks;
using Azure;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace Explorer.API.Controllers.Tourist.Encounters
{
    [Route("api/tourist/encounter-execution")]
    [Authorize(Policy = "touristPolicy")]
    public class EncounterExecutionController : BaseApiController
    {
        private readonly IEncounterExecutionService _encounterExecutionService;
        private readonly IEncounterService _encounterService;
        private readonly HttpClient _httpClient;

        public EncounterExecutionController(IEncounterExecutionService encounterExecutionService, IEncounterService encounterService)
        {
            _encounterExecutionService = encounterExecutionService;
            _encounterService = encounterService;
            _httpClient = new HttpClient();

        }

        [HttpGet("{id:int}")]
        public ActionResult<EncounterDto> GetById([FromRoute] int id)
        {
            
            var result = _encounterExecutionService.Get(id);
            return CreateResponse(result);
            
        }

        [HttpPut]
        public ActionResult<EncounterExecutionDto> Update([FromForm] EncounterExecutionDto encounterExecution)
        {
            var result = _encounterExecutionService.Update(encounterExecution, User.PersonId());
            return CreateResponse(result);
        }

        [HttpPut("activate/{chId:int}")]
        public async Task< ActionResult<EncounterExecutionDto>> Activate([FromRoute] int chId, [FromBody] int touristId)
        {
            /*
           // var result = _encounterExecutionService.Activate(User.PersonId(), touristLatitude, touristLongitude, id);
            return CreateResponse(result);
            */
            //DOBAVI ENCOUNTER PREKO CHID
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






            EncounterExecutionDto encExecutionDto = new EncounterExecutionDto();
       
            encExecutionDto.StartTime=DateTime.Now;
            encExecutionDto.TouristId = touristId;
          encExecutionDto.Status = "Active";
            encExecutionDto.EncounterId = retrievedEncounter.Id;
           // encExecutionDto.EncounterDto.CheckPointId = chId;
           // var microserviceUrl = "http://localhost:8082";


            try
            {
                // Serijalizujte EncounterDto objekat u JSON format
                var jsonContent = JsonConvert.SerializeObject(encExecutionDto);

                // Kreirajte HTTP zahtjev sa JSON sadržajem
                var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                // Kreirajte StringContent objekat sa potrebnim enkodingom i MIME tipom

                // Pošaljite HTTP POST zahtjev ka Go mikroservisu
                var response = await _httpClient.PostAsync($"{microserviceUrl}/executions/activate", httpContent);

                // Provjerite odgovor
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"HTTP status successfull {response.StatusCode}");
                    return encExecutionDto;
                }
                else
                {
                    // Ukoliko je odgovor neuspješan, obradite grešku na odgovarajući način
                    Console.WriteLine($"HTTP request failed with status code {response.StatusCode}");
                    return encExecutionDto;

                }
            }
            catch (Exception ex)
            {
                // Uhvatite i obradite izuzetak ako se desi
                Console.WriteLine($"An error occurred: {ex.Message}");
                return encExecutionDto;

            }
        }

        [HttpPut("completed/{id:int}")]
        public ActionResult<EncounterExecutionDto> CompleteExecution([FromRoute] int id, [FromForm] double touristLatitude, [FromForm] double touristLongitude)
        {
            var result = _encounterExecutionService.CompleteExecution(id, User.PersonId(), touristLatitude, touristLongitude);
            if (result.IsSuccess)
                result = _encounterService.AddEncounter(result.Value);
            return CreateResponse(result);
        }

        [HttpDelete("{id:int}")]
        public ActionResult Delete(int id)
        {
            var result = _encounterExecutionService.Delete(id, User.PersonId());
            return CreateResponse(result);
        }

        [HttpGet("get-all/{id:int}")]
        public ActionResult<PagedResult<EncounterExecutionDto>> GetAllByTourist(int id, [FromQuery] int page, [FromQuery] int pageSize)
        {
            if (id != User.PersonId())
            {
                return Unauthorized();
            }
            var result = _encounterExecutionService.GetAllByTourist(id, page, pageSize);
            return CreateResponse(result);
        }

        [HttpGet("get-all-completed")]
        public async Task<ActionResult<List<EncounterExecutionDto>>> GetAllCompletedByTourist([FromQuery] int page, [FromQuery] int pageSize)
        {
            var microserviceUrl = "http://localhost:8082"; // Adresa vašeg mikroservisa

            try
            {
                // Napravite HTTP GET zahtjev ka mikroservisu
                var response = await _httpClient.GetAsync($"{microserviceUrl}/executions/get-all-completed/2");

                // Provjerite odgovor
                if (response.IsSuccessStatusCode)
                {
                    // Ako je zahtjev uspješan, parsirajte odgovor i vratite rezultat
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<List<EncounterExecutionDto>>(jsonString);
                    Console.WriteLine(result);
                    return result;
                }
                else
                {
                    // Ukoliko je odgovor neuspješan, obradite grešku na odgovarajući način
                    Console.WriteLine($"HTTP request failed with status code {response.StatusCode}");
                    return StatusCode((int)response.StatusCode); // Vratite odgovarajući status kod
                }
            }
            catch (Exception ex)
            {
                // Uhvatite i obradite izuzetak ako se desi
                Console.WriteLine($"An error occurred: {ex.Message}");
                return StatusCode(500); // Internal Server Error
            }
        }


        /*
        [HttpGet("get-all-completed")]
        public ActionResult<PagedResult<EncounterExecutionDto>> GetAllCompletedByTourist([FromQuery] int page, [FromQuery] int pageSize)
        {
            var result = _encounterExecutionService.GetAllCompletedByTourist(User.PersonId(), page, pageSize);
            return CreateResponse(result);
        }

        
        [HttpGet("get-by-tour/{chId:int}")]
        public async Task<ActionResult<EncounterExecutionDto>> GetByTour([FromRoute] int chId)
        */

        [HttpGet("get-by-tour/{chId:int}")]
        public async Task<ActionResult<EncounterExecutionDto>> GetByTour([FromRoute] int chId)
        {
            /*
            var result = _encounterExecutionService.GetVisibleByTour(id, touristLongitude, touristLatitude, User.PersonId());
            if(result.IsSuccess)
                result = _encounterService.AddEncounter(result.Value);
            return CreateResponse(result);
            */

            var microserviceUrl = "http://localhost:8082";

            try
            {
                // Napravite HTTP GET zahtjev ka mikroservisu za dobavljanje encounter-a po ID-u
                var response = await _httpClient.GetAsync($"{microserviceUrl}/executions/getByCheckPoint/{chId}");

                // Provjerite odgovor
                if (response.IsSuccessStatusCode)
                {
                    // Ukoliko je odgovor uspješan, izvucite podatke iz odgovora
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var retrievedEncounter = JsonConvert.DeserializeObject<EncounterExecutionDto>(jsonString);

                    Console.WriteLine($"EncounterExecution retrieved successfully. ID: {retrievedEncounter}");
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

        [HttpGet("social/checkRange/{id:int}/{tourId:int}")]
        public ActionResult<EncounterExecutionDto> CheckPosition([FromRoute] int tourId, [FromRoute] int id, [FromQuery] double touristLatitude, [FromQuery] double touristLongitude)
        {
            var result = _encounterExecutionService.GetWithUpdatedLocation(tourId, id, touristLongitude, touristLatitude, User.PersonId());
            if (result.IsSuccess)
                result = _encounterService.AddEncounter(result.Value);
            return CreateResponse(result);
        }

        [HttpGet("location/checkRange/{id:int}/{tourId:int}")]
        public ActionResult<EncounterExecutionDto> CheckPositionLocationEncounter([FromRoute] int tourId, [FromRoute] int id, [FromQuery] double touristLatitude, [FromQuery] double touristLongitude)
        {
            var result = _encounterExecutionService.GetHiddenLocationEncounterWithUpdatedLocation(tourId, id, touristLongitude, touristLatitude, User.PersonId());
            if (result.IsSuccess)
                result = _encounterService.AddEncounter(result.Value);
            return CreateResponse(result);
        }

        [HttpGet("active/by-tour/{id:int}")]
        public ActionResult<List<EncounterExecutionDto>> GetActiveByTour([FromRoute] int id)
        {
            var result = _encounterExecutionService.GetActiveByTour(User.PersonId(), id);
            if (result.IsSuccess)
                result = _encounterService.AddEncounters(result.Value);
            return CreateResponse(result);
        }
    }
}
