using Microsoft.AspNetCore.Mvc;
using ERP.Application.Interfaces;
using ERP.Application.DTOs;

namespace ERP.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        /// <summary>
        /// 모든 제품 조회 (재고 정보 포함)
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts()
        {
            var products = await _productService.GetAllProductsAsync();
            return Ok(products);
        }

        /// <summary>
        /// 제품 상세 조회
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> GetProduct(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound($"ID {id}에 해당하는 제품을 찾을 수 없습니다.");
            }
            return Ok(product);
        }

        /// <summary>
        /// SKU로 제품 조회
        /// </summary>
        [HttpGet("by-sku/{sku}")]
        public async Task<ActionResult<ProductDto>> GetProductBySku(string sku)
        {
            var product = await _productService.GetProductBySkuAsync(sku);
            if (product == null)
            {
                return NotFound($"SKU {sku}에 해당하는 제품을 찾을 수 없습니다.");
            }
            return Ok(product);
        }

        /// <summary>
        /// 재고 부족 제품 조회
        /// </summary>
        [HttpGet("low-stock")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetLowStockProducts()
        {
            var products = await _productService.GetLowStockProductsAsync();
            return Ok(products);
        }

        /// <summary>
        /// 새 제품 생성
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ProductDto>> CreateProduct(CreateProductDto createProductDto)
        {
            try
            {
                var product = await _productService.CreateProductAsync(createProductDto);
                return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// 제품 정보 수정
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, UpdateProductDto updateProductDto)
        {
            try
            {
                var result = await _productService.UpdateProductAsync(id, updateProductDto);
                if (!result)
                {
                    return NotFound($"ID {id}에 해당하는 제품을 찾을 수 없습니다.");
                }
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// 제품 삭제
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                var result = await _productService.DeleteProductAsync(id);
                if (!result)
                {
                    return NotFound($"ID {id}에 해당하는 제품을 찾을 수 없습니다.");
                }
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}