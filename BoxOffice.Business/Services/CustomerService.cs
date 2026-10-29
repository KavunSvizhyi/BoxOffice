using AutoMapper;
using BoxOffice.Business.Interfaces;
using BoxOffice.Business.Models;
using BoxOffice.Business.Validation;
using BoxOffice.Data.Entities;
using BoxOffice.Data.Interfaces;

namespace BoxOffice.Business.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public CustomerService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<CustomerModel> AddAsync(CustomerModel model)
        {
            Validate(model);

            var person = _mapper.Map<Person>(model);
            await _unitOfWork.PersonRepository.AddAsync(person);

            var customer = _mapper.Map<Customer>(model);
            await _unitOfWork.CustomerRepository.AddAsync(customer);

            await _unitOfWork.SaveAsync();

            return _mapper.Map<CustomerModel>(customer);
        }

        public async Task DeleteAsync(int id)
        {
            var customer = await _unitOfWork.CustomerRepository.GetByIdAsync(id);
            if (customer is not null)
            {
                await _unitOfWork.CustomerRepository.DeleteByIdAsync(id);
                await _unitOfWork.PersonRepository.DeleteByIdAsync(customer.PersonId);
                await _unitOfWork.SaveAsync();
            }
        }

        public async Task<IEnumerable<CustomerModel>> GetAllAsync()
        {
            var customers = await _unitOfWork.CustomerRepository.GetAllWithDetailsAsync();

            return _mapper.Map<IEnumerable<CustomerModel>>(customers);
        }

        public async Task<CustomerModel?> GetByIdAsync(int id)
        {
            var customer = await _unitOfWork.CustomerRepository.GetByIdWithDetailsAsync(id);

            return _mapper.Map<CustomerModel>(customer);
        }

        public async Task UpdateAsync(CustomerModel model)
        {
            Validate(model);

            var customer = await _unitOfWork.CustomerRepository.GetByIdWithDetailsAsync(model.Id);
            if (customer is null)
            {
                throw new TheatreException($"There is no such ID as {model.Id}!");
            }

            customer.Person.FirstName = model.FirstName;
            customer.Person.LastName = model.LastName;
            customer.Person.BirthDate = model.BirthDate;

            _unitOfWork.PersonRepository.Update(customer.Person);
            await _unitOfWork.SaveAsync();
        }

        private void Validate(CustomerModel model)
        {
            if (string.IsNullOrEmpty(model.LastName) ||
                string.IsNullOrWhiteSpace(model.LastName))
            {
                throw new TheatreException($"{nameof(model.LastName)} cannot be blank!");
            }

            if (string.IsNullOrEmpty(model.FirstName) ||
                string.IsNullOrWhiteSpace(model.FirstName))
            {
                throw new TheatreException($"{nameof(model.FirstName)} cannot be blank!");
            }

            if (model.BirthDate.Year < 1900 ||
                model.BirthDate > DateTime.Now)
            {
                throw new TheatreException($"{nameof(model.BirthDate)} is out of the range (The year of 1900 to Now)!");
            }
        }

        public async Task<IEnumerable<CustomerModel>> GetCustomersByPerfomanceIdAsync(int perfomanceId)
        {
            var perfomance = await _unitOfWork.PerfomanceRepository.GetByIdWithDetailsAsync(perfomanceId);
            if (perfomance is null)
            {
                throw new TheatreException($"There is no such ID as {perfomanceId}!");
            }

            var customers = perfomance.Tickets
                .Where(t => t.TicketBooking is not null)
                .Select(t => t.TicketBooking!.Customer);

            return _mapper.Map<IEnumerable<CustomerModel>>(customers);
        }
    }
}
