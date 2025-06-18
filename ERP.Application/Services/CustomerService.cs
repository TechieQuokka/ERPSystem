using AutoMapper;
using ERP.Application.DTOs;
using ERP.Application.Interfaces;
using ERP.Domain.Entities;
using ERP.Domain.Interfaces;

namespace ERP.Application.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IMapper _mapper;

        public CustomerService(ICustomerRepository customerRepository, IMapper mapper)
        {
            _customerRepository = customerRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CustomerDto>> GetAllCustomersAsync()
        {
            var customers = await _customerRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<CustomerDto>>(customers);
        }

        public async Task<CustomerDto?> GetCustomerByIdAsync(int id)
        {
            var customer = await _customerRepository.GetByIdAsync(id);
            return customer == null ? null : _mapper.Map<CustomerDto>(customer);
        }

        public async Task<CustomerDto> CreateCustomerAsync(CreateCustomerDto createCustomerDto)
        {
            // 비즈니스 로직: 이메일 중복 체크
            if (!string.IsNullOrEmpty(createCustomerDto.Email))
            {
                var isEmailUnique = await _customerRepository.IsEmailUniqueAsync(createCustomerDto.Email);
                if (!isEmailUnique)
                {
                    throw new InvalidOperationException($"이미 존재하는 이메일입니다: {createCustomerDto.Email}");
                }
            }

            var customer = _mapper.Map<Customer>(createCustomerDto);
            customer.CreatedAt = DateTime.Now;

            var createdCustomer = await _customerRepository.AddAsync(customer);
            await _customerRepository.SaveChangesAsync();

            return _mapper.Map<CustomerDto>(createdCustomer);
        }

        public async Task<bool> UpdateCustomerAsync(int id, UpdateCustomerDto updateCustomerDto)
        {
            var existingCustomer = await _customerRepository.GetByIdAsync(id);
            if (existingCustomer == null)
                return false;

            // 비즈니스 로직: 이메일 중복 체크
            if (!string.IsNullOrEmpty(updateCustomerDto.Email))
            {
                var isEmailUnique = await _customerRepository.IsEmailUniqueAsync(updateCustomerDto.Email, id);
                if (!isEmailUnique)
                {
                    throw new InvalidOperationException($"이미 다른 고객이 사용중인 이메일입니다: {updateCustomerDto.Email}");
                }
            }

            // AutoMapper로 업데이트할 속성들만 매핑
            _mapper.Map(updateCustomerDto, existingCustomer);

            await _customerRepository.UpdateAsync(existingCustomer);
            await _customerRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteCustomerAsync(int id)
        {
            var customer = await _customerRepository.GetByIdAsync(id);
            if (customer == null)
                return false;

            await _customerRepository.DeleteAsync(customer);
            await _customerRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CustomerExistsAsync(int id)
        {
            return await _customerRepository.ExistsAsync(id);
        }
    }
}