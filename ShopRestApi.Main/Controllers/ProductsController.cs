using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopRestApi.Application.Common.Constants;
using ShopRestApi.Application.Common.Models;
using ShopRestApi.Application.DTOs.ProductsDtos;
using ShopRestApi.Application.Interfaces;
using ShopRestApi.Domain.Entities;

namespace ShopRestApi.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IMapper _mapper;
        public ProductsController(IProductService productService, IMapper mapper)
        {
            _productService = productService;
            _mapper = mapper;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _productService.GetAllAsync();
            var result = _mapper.Map<List<ProductDto>>(products);
            return Ok(new ApiResponse<List<ProductDto>>
            {
                Success = true,
                Message = "Products retrieved successfully",
                Data = products
            });
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Product not found",
                    Errors = new List<string> { $"Id {id} does not exist" }
                });
            }

            var result = _mapper.Map<ProductDto>(product);
            return Ok(result);
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProductDto dto)
        {
            var result = await _productService.AddAsync(dto);

            return CreatedAtAction(
                    nameof(GetById),
                    new { id = result.Id },
                    new ApiResponse<ProductDto>
                    {
                        Success = true,
                        Message = "Product created successfully",
                        Data = result
                    });
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateProductDto dto)
        {
            var updated = await _productService.UpdateAsync(id, dto);

            if (!updated)
                return NotFound();

            return NoContent();
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _productService.DeleteAsync(id);

            if (!deleted)
                return NotFound();

            return NoContent();
        }

        [Authorize]
        [HttpGet("paged")]
        public async Task<IActionResult> GetPaged([FromQuery] ProductQueryParameters parameters)
        {
            var result = await _productService.GetPagedAsync(parameters);

            return Ok(new ApiResponse<PagedResult<ProductDto>>
            {
                Success = true,
                Data = result
            });
        }
    }
}