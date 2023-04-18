using FlyShoes.Common.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace FlyShoes.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AddressController : ControllerBase
    {
        public AddressController()
        {

        }

        [HttpGet("city")]
        public async Task<ServiceResponse> GetAllCity()
        {
            var result = new ServiceResponse();
            var httpClient = new HttpClient();
            var response = await httpClient.GetAsync("https://provinces.open-api.vn/api/");
            var responseString = await response.Content.ReadAsStringAsync();
            result.Data = JsonSerializer.Deserialize<List<City>>(responseString);

            return result;
        }

        [HttpGet("district-by-city/{code}")]
        public async Task<ServiceResponse> GetDictrictByCity(int code)
        {
            var result = new ServiceResponse();
            var httpClient = new HttpClient();
            var response = await httpClient.GetAsync("https://provinces.open-api.vn/api/d");
            var responseString = await response.Content.ReadAsStringAsync();
            var districts = JsonSerializer.Deserialize<List<District>>(responseString);
            result.Data = districts.Where(district => district.province_code.Equals(code)).ToList();

            return result;
        }

        [HttpGet("ward-by-district/{code}")]
        public async Task<ServiceResponse> GetWardByDistrict(int code)
        {
            var result = new ServiceResponse();
            var httpClient = new HttpClient();
            var response = await httpClient.GetAsync("https://provinces.open-api.vn/api/w");
            var responseString = await response.Content.ReadAsStringAsync();
            var wards = JsonSerializer.Deserialize<List<Ward>>(responseString);
            result.Data = wards.Where(ward => ward.district_code.Equals(code)).ToList();

            return result;
        }
    }
}
