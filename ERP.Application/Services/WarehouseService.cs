using AutoMapper;
using ERP.Application.DTOs;
using ERP.Application.Interfaces;
using ERP.Domain.Entities;
using ERP.Domain.Interfaces;

namespace ERP.Application.Services
{
    public class WarehouseService : IWarehouseService
    {
        private readonly IWarehouseRepository _warehouseRepository;
        private readonly IMapper _mapper;

        public WarehouseService(IWarehouseRepository warehouseRepository, IMapper mapper)
        {
            _warehouseRepository = warehouseRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<WarehouseDto>> GetAllWarehousesAsync()
        {
            var warehouses = await _warehouseRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<WarehouseDto>>(warehouses);
        }

        public async Task<WarehouseDto?> GetWarehouseByIdAsync(int id)
        {
            var warehouse = await _warehouseRepository.GetByIdAsync(id);
            return warehouse == null ? null : _mapper.Map<WarehouseDto>(warehouse);
        }

        public async Task<WarehouseDto> CreateWarehouseAsync(CreateWarehouseDto createWarehouseDto)
        {
            // 비즈니스 로직: 이름 중복 체크
            var isNameUnique = await _warehouseRepository.IsNameUniqueAsync(createWarehouseDto.Name);
            if (!isNameUnique)
            {
                throw new InvalidOperationException($"이미 존재하는 창고 이름입니다: {createWarehouseDto.Name}");
            }

            var warehouse = _mapper.Map<Warehouse>(createWarehouseDto);
            warehouse.CreatedAt = DateTime.UtcNow;

            var createdWarehouse = await _warehouseRepository.AddAsync(warehouse);
            await _warehouseRepository.SaveChangesAsync();

            return _mapper.Map<WarehouseDto>(createdWarehouse);
        }

        public async Task<bool> UpdateWarehouseAsync(int id, UpdateWarehouseDto updateWarehouseDto)
        {
            var existingWarehouse = await _warehouseRepository.GetByIdAsync(id);
            if (existingWarehouse == null)
                return false;

            // 비즈니스 로직: 이름 중복 체크
            var isNameUnique = await _warehouseRepository.IsNameUniqueAsync(updateWarehouseDto.Name, id);
            if (!isNameUnique)
            {
                throw new InvalidOperationException($"이미 다른 창고가 사용중인 이름입니다: {updateWarehouseDto.Name}");
            }

            _mapper.Map(updateWarehouseDto, existingWarehouse);

            await _warehouseRepository.UpdateAsync(existingWarehouse);
            await _warehouseRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteWarehouseAsync(int id)
        {
            var warehouse = await _warehouseRepository.GetByIdAsync(id);
            if (warehouse == null)
                return false;

            // 비즈니스 로직: 재고가 있는 창고는 삭제 불가
            if (warehouse.Inventories.Any())
            {
                throw new InvalidOperationException("재고가 있는 창고는 삭제할 수 없습니다.");
            }

            await _warehouseRepository.DeleteAsync(warehouse);
            await _warehouseRepository.SaveChangesAsync();
            return true;
        }
    }
}
