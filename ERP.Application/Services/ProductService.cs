using AutoMapper;
using ERP.Application.DTOs;
using ERP.Application.Interfaces;
using ERP.Domain.Entities;
using ERP.Domain.Interfaces;

namespace ERP.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public ProductService(IProductRepository productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
        {
            var products = await _productRepository.GetProductsWithInventoryAsync();
            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }

        public async Task<ProductDto?> GetProductByIdAsync(int id)
        {
            var product = await _productRepository.GetByIdWithInventoryAsync(id);
            return product == null ? null : _mapper.Map<ProductDto>(product);
        }

        public async Task<ProductDto?> GetProductBySkuAsync(string sku)
        {
            var product = await _productRepository.GetBySkuAsync(sku);
            return product == null ? null : _mapper.Map<ProductDto>(product);
        }

        public async Task<IEnumerable<ProductDto>> GetLowStockProductsAsync()
        {
            var products = await _productRepository.GetProductsWithInventoryAsync();
            var lowStockProducts = products.Where(p => p.Inventory != null && p.Inventory.Quantity <= p.Inventory.MinimumStock);
            return _mapper.Map<IEnumerable<ProductDto>>(lowStockProducts);
        }

        public async Task<ProductDto> CreateProductAsync(CreateProductDto createProductDto)
        {
            // 비즈니스 로직: SKU 중복 체크
            var isSkuUnique = await _productRepository.IsSkuUniqueAsync(createProductDto.SKU);
            if (!isSkuUnique)
            {
                throw new InvalidOperationException($"이미 존재하는 SKU입니다: {createProductDto.SKU}");
            }

            // 비즈니스 로직: 가격 유효성 체크
            if (createProductDto.UnitPrice <= 0)
            {
                throw new ArgumentException("제품 가격은 0보다 커야 합니다.");
            }

            var product = _mapper.Map<Product>(createProductDto);
            product.CreatedAt = DateTime.Now;

            var createdProduct = await _productRepository.AddAsync(product);
            await _productRepository.SaveChangesAsync();

            // 제품 생성시 자동으로 재고 레코드도 생성
            var inventory = new Inventory
            {
                ProductId = createdProduct.Id,
                Quantity = 0,
                MinimumStock = 0,
                LastUpdated = DateTime.Now
            };

            // 직접 DbContext를 통해 Inventory 추가 (Repository 패턴 완성 후 수정 예정)
            createdProduct.Inventory = inventory;
            await _productRepository.SaveChangesAsync();

            return _mapper.Map<ProductDto>(await _productRepository.GetByIdWithInventoryAsync(createdProduct.Id));
        }

        public async Task<bool> UpdateProductAsync(int id, UpdateProductDto updateProductDto)
        {
            var existingProduct = await _productRepository.GetByIdAsync(id);
            if (existingProduct == null)
                return false;

            // 비즈니스 로직: SKU 중복 체크
            var isSkuUnique = await _productRepository.IsSkuUniqueAsync(updateProductDto.SKU, id);
            if (!isSkuUnique)
            {
                throw new InvalidOperationException($"이미 다른 제품이 사용중인 SKU입니다: {updateProductDto.SKU}");
            }

            // 비즈니스 로직: 가격 유효성 체크
            if (updateProductDto.UnitPrice <= 0)
            {
                throw new ArgumentException("제품 가격은 0보다 커야 합니다.");
            }

            _mapper.Map(updateProductDto, existingProduct);

            await _productRepository.UpdateAsync(existingProduct);
            await _productRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            var product = await _productRepository.GetByIdWithInventoryAsync(id);
            if (product == null)
                return false;

            // 비즈니스 로직: 재고가 있는 제품은 삭제 불가
            if (product.Inventory != null && product.Inventory.Quantity > 0)
            {
                throw new InvalidOperationException("재고가 있는 제품은 삭제할 수 없습니다.");
            }

            await _productRepository.DeleteAsync(product);
            await _productRepository.SaveChangesAsync();
            return true;
        }
    }
}