using Explorer.API.Services;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Encounters.API.Dtos;
using Explorer.Encounters.API.Public;
using Explorer.Encounters.Core.UseCases;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Explorer.Tours.API.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Globalization;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Azure;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Collections.Generic;

namespace Explorer.API.Controllers.Tourist.Encounters
{
    [Route("api/administration/touristEncounter")]
    public class TouristEncounterController : BaseApiController
    {
        private readonly IEncounterService _encounterService;
        private readonly ImageService _imageService;
        private readonly HttpClient    _httpClient ;

        public TouristEncounterController(IEncounterService encounterService)
        {
            _encounterService = encounterService;
            _imageService = new ImageService();
            _httpClient = new HttpClient();

        }

        [HttpPost]
        [Authorize(Policy = "touristPolicy")]
        public ActionResult<EncounterDto> Create([FromForm] EncounterDto encounter, [FromQuery] long checkpointId, [FromQuery] bool isSecretPrerequisite, [FromForm] List<IFormFile>? imageF = null)
        {


            if (imageF != null && imageF.Any())
            {
                var imageNames = _imageService.UploadImages(imageF);
                if (encounter.Type == "Location") ;
                    //encounter.Image = imageNames[0];
            }

            // Transformacija koordinata za longitude
            encounter.Longitude = TransformisiKoordinatu(encounter.Longitude);

            // Transformacija koordinata za latitude
            encounter.Latitude = TransformisiKoordinatu(encounter.Latitude);

            encounter.Status = "Draft";
            var result = _encounterService.CreateForTourist(encounter, checkpointId, isSecretPrerequisite, User.PersonId());
            return CreateResponse(result);
        }

        [HttpPut]
        [Authorize(Policy = "touristPolicy")]
        public ActionResult<EncounterDto> Update([FromForm] EncounterDto encounter, [FromForm] List<IFormFile>? imageF = null)
        {

            if (imageF != null && imageF.Any())
            {
                var imageNames = _imageService.UploadImages(imageF);
                if (encounter.Type == "Location") ;
                   // encounter.Image = imageNames[0];
            }

            var result = _encounterService.Update(encounter, User.PersonId());
            return CreateResponse(result);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Policy = "touristPolicy")]
        public ActionResult Delete(int id)
        {
            var result = _encounterService.Delete(id, User.PersonId());
            return CreateResponse(result);
        }

        [HttpGet]
        [Authorize(Policy = "administratorPolicy")]
        public async Task<ActionResult<List<EncounterDto>>> GetAll()
        {
            /*
            var result = _encounterService.GetPaged(0, 0);
            return CreateResponse(result);
            */
            List < EncounterDto > retrievedEncounter = new List<EncounterDto>();

            var microserviceUrl = "http://localhost:8082";
            try
            {
                // Napravite HTTP GET zahtjev ka mikroservisu za dobavljanje encounter-a po ID-u
                var response = await _httpClient.GetAsync($"{microserviceUrl}/encounters");

                // Provjerite odgovor
                if (response.IsSuccessStatusCode)
                {
                    // Ukoliko je odgovor uspješan, izvucite podatke iz odgovora
                    var jsonString = await response.Content.ReadAsStringAsync();
                    retrievedEncounter = JsonConvert.DeserializeObject<List<EncounterDto>>(jsonString);

                   
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

        [HttpGet("{id:int}")]
        [Authorize(Policy = "touristPolicy")]
        public ActionResult<EncounterDto> GetById(int id)
        {
            var result = _encounterService.Get(id);
            return CreateResponse(result);
        }

        [HttpGet("requestInfo/{encounterId:long}")]
        [Authorize(Policy = "administratorPolicy")]
        public ActionResult<EncounterDto> GetRequestInfo(long encounterId)
        {
            var result = _encounterService.GetRequestInfo(encounterId);
            return CreateResponse(result);
        }

        // Funkcija za transformaciju koordinata
        private double TransformisiKoordinatu(double koordinata)
        {
            // Pretvori broj u string kako bi se mogao indeksirati
            string koordinataString = koordinata.ToString();

            // Ako je koordinata dovoljno dugačka
            if (koordinataString.Length > 2)
            {
                // Uzmi prva dva znaka
                string prviDeo = koordinataString.Substring(0, 2);

                // Uzmi ostatak broja posle prva dva znaka
                string drugiDeo = koordinataString.Substring(2);

                // Sastavi transformisanu vrednost
                string transformisanaKoordinataString = prviDeo + '.' + drugiDeo;

                // Parsiraj rezultat nazad kao double
                if (double.TryParse(transformisanaKoordinataString, NumberStyles.Any, CultureInfo.InvariantCulture, out double transformisanaKoordinata))
                {
                    return transformisanaKoordinata;
                }
            }

            // Ako je koordinata prekratka ili neuspešno parsiranje, vrati nepromenjenu vrednost
            return koordinata;
        }
    }
}
