using AutoMapper;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Application.DTOs.Customers;
using WebApplication1.Application.Interfaces;
using WebApplication1.Domain.Entities;

using WebApplication1.Shared.Results;
using WebApplication1.Shared.Extensions;
using WebApplication1.Infrastructure.Data;

namespace WebApplication1.Application.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly IBaseRepository<Customer> _customerRepository;
        
        private readonly IMapper _mapper;

        public CustomerService(IBaseRepository<Customer> customerRepository, IMapper mapper)
        {
            _customerRepository = customerRepository;
            
            _mapper = mapper;
        }

        public async Task<PaginatedResult<CustomerDto>> GetCustomersAsync(FilterParams filterParams)
        {
            var query = _customerRepository.Query();

            if (!string.IsNullOrWhiteSpace(filterParams.SearchTerm))
            {
                query = query.Where(c => c.FirstName.Contains(filterParams.SearchTerm) || c.LastName.Contains(filterParams.SearchTerm) || c.Email.Contains(filterParams.SearchTerm));
            }

            query = query.ApplyFilterParams(filterParams);

            var dtoQuery = _mapper.ProjectTo<CustomerDto>(query);

            return await dtoQuery.ToPaginatedListAsync(filterParams.PageNumber, filterParams.PageSize);
        }

        public async Task<CustomerDto> GetCustomerByIdAsync(Guid id)
        {
            var customer = await _customerRepository.GetByIdAsync(id);
            if (customer == null)
            {
                return null;
            }

            return new CustomerDto
            {
                Id = customer.Id,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Email = customer.Email,
                Phone = customer.Phone,
                Address = customer.Address
            };
        }

        public async Task<CustomerDto> CreateCustomerAsync(CreateCustomerDto createCustomerDto)
        {
            var customer = _mapper.Map<Customer>(createCustomerDto);

            await _customerRepository.AddAsync(customer);
            await _customerRepository.SaveChangesAsync();

            return _mapper.Map<CustomerDto>(customer);
        }

        public async Task<bool> UpdateCustomerAsync(Guid id, UpdateCustomerDto updateCustomerDto)
        {
            var customer = await _customerRepository.GetByIdAsync(id);
            if (customer == null)
            {
                return false;
            }

            _mapper.Map(updateCustomerDto, customer);

            _customerRepository.Update(customer);
            await _customerRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteCustomerAsync(Guid id)
        {
            var customer = await _customerRepository.GetByIdAsync(id);
            if (customer == null)
            {
                return false;
            }

            _customerRepository.Remove(customer);
            await _customerRepository.SaveChangesAsync();

            return true;
        }

   

        

    }
}