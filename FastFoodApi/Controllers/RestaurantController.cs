using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DTO.ProductDTO;
using Application.DTO.RestaurantDTO;
using Application.Repositories;
using Domain.Entities;
using FluentValidation;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace FastFoodApi.Controllers
{
    [ApiController]
    [Route("/")]
    [Authorize]
    public class RestaurantController : ControllerBase
    {
        private readonly IRestaurantRepository _repository;

        private readonly IValidator<Restaurant> _validator;

        private readonly IUserRepository _userRepository;

        public RestaurantController(IRestaurantRepository repository, IValidator<Restaurant> validator, IUserRepository userRepository)
        {
            _repository = repository;
            _validator = validator;
            _userRepository = userRepository;
        }

        [HttpGet("[action]")]
        [Authorize(Roles = "GetAllRestaurant")]
        public async Task<IActionResult> GetAllRestaurantAsync()
        {
            var restaurants = _repository.GetAsync(x => true);

            return Ok(restaurants);
        }

        [HttpGet("[action]")]
        [Authorize(Roles = "GetRestaurantById")]
        public async Task<IActionResult> GetRestaurantById([FromQuery] int Id)
        {
            Restaurant restaurant = await _repository.GetByIdAsync(Id);
            if(restaurant == null)
            {
                return NotFound("Restaurant from this Id not found");
            }
            return Ok(restaurant);
        }

        [HttpPost("[action]")]
        [Authorize(Roles = "CreateRestaurnt")]
        public async Task<IActionResult> CreateRestaurnt([FromForm] RestaurantCreateDTO restCreateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (restCreateDto.Image == null || restCreateDto.Image.Length == 0)
            {
                return BadRequest("Image is required.");
            }

            byte[] imageData;
            using (var memoryStream = new MemoryStream())
            {
                await restCreateDto.Image.CopyToAsync(memoryStream);
                imageData = memoryStream.ToArray();
            }

            var restaurant = new Restaurant
            {
                Name = restCreateDto.Name,
                Description = restCreateDto.Description,
                ImageData = imageData
            };

            var validationResult = _validator.Validate(restaurant);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var createdRestaurant = await _repository.AddAsync(restaurant);
            return Ok(createdRestaurant);
        }

        [HttpPut("[action]")]
        [Authorize(Roles = "UpdateRestaurant")]
        public async Task<IActionResult> UpdateRestaurant(int id,[FromBody] RestaurantUpdateDTO restUpdate)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var hasRest = await _repository.GetByIdAsync(id);

            hasRest.Name = restUpdate.Name;
            hasRest.Description = restUpdate.Description;
            

            var validationResult = _validator.Validate(hasRest);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var updatedRestaurant = await _repository.UpdateAsync(hasRest);
            return Ok(updatedRestaurant);
        }

        [HttpDelete("[action]")]
        [Authorize(Roles = "DeleteRestaurant")]
        public async Task<IActionResult> DeleteRestaurant([FromQuery] int Id)
        {
            var existingProduct = await _repository.GetByIdAsync(Id);
            if (existingProduct == null)
            {
                return NotFound("Restaurant not found.");
            }

            await _repository.DeleteAsync(Id);
            return Ok($"Restaurant with ID {Id} has been deleted successfully.");
        }

        [HttpGet("[action]")]
        [Authorize(Roles = "SearchingRestaurants")]
        public IActionResult SearchingRestaurants(string text)
        {
            return Ok(_repository.SearchingRestaurants(text));
        }

        [HttpGet("[action]")]
        [Authorize(Roles = "GetRestaurantImage")]
        public async Task<IActionResult> GetRestaurantImage(int id)
        {
            Restaurant restaurant = await _repository.GetByIdAsync(id);
            if (restaurant == null || restaurant.ImageData == null)
            {
                return NotFound("Image not found for the restaurant.");
            }

            return File(restaurant.ImageData, "image/png");
        }

    }
}

