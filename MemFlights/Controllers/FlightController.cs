using MemFlights.Models;
using MemFlights.Repository;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MemFlights.Controllers
{
    [ApiController]
    [Route("/")]
    public class FlightController : Controller
    {
        private readonly IFlightsRepository _flightsRepository;

        public FlightController(IFlightsRepository flightsRepository) {
            _flightsRepository = flightsRepository;
        }

        [Route("/graph")]
        [HttpGet]
        public async Task<D3Graph> FetchD3Graph([FromQuery(Name = "limit")] int limit) {
            return await _flightsRepository.FetchD3Graph(limit <= 0 ? 50 : limit);
        }
    }
}
