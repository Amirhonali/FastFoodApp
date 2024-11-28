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
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _repository;

        private readonly IUserRepository _userRepository;

        private readonly IValidator<Product> _validator;

        public ProductController(IProductRepository repository, IValidator<Product> validator,IUserRepository userRepository)
        {
            _repository = repository;
            _validator = validator;
            _userRepository = userRepository;
        }

        [HttpGet("[action]")]
        [Authorize(Roles = "GetAllProducts")]
        public async Task<IActionResult> GetAllProductsAsync()
        {
            var products = _repository.GetAsync(x => true);

            return Ok(products);
        }

        [HttpGet("[action]")]
        [Authorize(Roles = "GetProductById")]
        public async Task<IActionResult> GetProductById([FromQuery] int Id)
        {
            Product product = await _repository.GetByIdAsync(Id);
            if (product == null)
            {
                return NotFound("Product from this id not found!!!");
            }
            return Ok(product);
        }


        [HttpPost("[action]")]
        [Authorize(Roles = "CreateProduct")]
        public async Task<IActionResult> CreateProduct([FromForm] ProductCreateDTO productCreateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (productCreateDto.Image == null || productCreateDto.Image.Length == 0)
            {
                return BadRequest("Image is required.");
            }

            byte[] imageData;
            using (var memoryStream = new MemoryStream())
            {
                await productCreateDto.Image.CopyToAsync(memoryStream);
                imageData = memoryStream.ToArray();
            }

            var product = new Product
            {
                Name = productCreateDto.Name,
                Description = productCreateDto.Description,
                Price = productCreateDto.Price,
                Category = productCreateDto.Category,
                ImageData = imageData 
            };

            var validationResult = _validator.Validate(product);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var createdProduct = await _repository.AddAsync(product);
            return Ok(createdProduct);
        }



        [HttpPut("[action]")]
        [Authorize(Roles = "UpdateProduct")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] ProductUpdateDTO productUpdateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var hasProd = await _repository.GetByIdAsync(id);
            if(hasProd == null)
            {
                return NotFound("Product not found!!!");
            }

            hasProd.Name = productUpdateDto.Name;
            hasProd.Description = productUpdateDto.Description;
            hasProd.Price = productUpdateDto.Price;
            hasProd.Category = productUpdateDto.Category;
           

            var validationResult = _validator.Validate(hasProd);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var updatedProduct = await _repository.UpdateAsync(hasProd);
            return Ok(updatedProduct);
        }

        [HttpDelete("[action]")]
        [Authorize(Roles = "DeleteProduct")]
        public async Task<IActionResult> DeleteProduct([FromQuery] int Id)
        {
            var existingProduct = await _repository.GetByIdAsync(Id);
            if (existingProduct == null)
            {
                return NotFound("Product not found.");
            }

            await _repository.DeleteAsync(Id);
            return Ok($"Product with ID {Id} has been deleted successfully.");
        }

        [HttpGet("[action]")]
        [Authorize(Roles = "SearchingProducts")]
        public IActionResult SearchingProducts(string text)
        {
            return Ok(_repository.SearchingProducts(text));
        }
    }
}

