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

            var person = await _unitOfWork.PersonRepository.GetByEmailAsync(model.Email);
            var customer = _mapper.Map<Customer>(model);

            if (person is null)
            {
                customer.Person = _mapper.Map<Person>(model);
            }
            else
            {
                bool isPersonAlreadyCustomer = await _unitOfWork.CustomerRepository.ExistsByPersonId(person.Id);
                if (isPersonAlreadyCustomer)
                {
                    throw new TheatreException("The customer with this email is already in Database!");
                }
                customer.PersonId = person.Id;
            }

            await _unitOfWork.CustomerRepository.AddAsync(customer);
            await _unitOfWork.SaveAsync();

            return _mapper.Map<CustomerModel>(customer);
        }

        private void Validate(CustomerModel model)
        {
            if (string.IsNullOrWhiteSpace(model.LastName))
            {
                throw new TheatreException($"{nameof(model.LastName)} cannot be blank!");
            }

            if (string.IsNullOrWhiteSpace(model.FirstName))
            {
                throw new TheatreException($"{nameof(model.FirstName)} cannot be blank!");
            }

            if (string.IsNullOrWhiteSpace(model.Email))
            {
                throw new TheatreException($"{nameof(model.Email)} cannot be blank!");
            }

            if (model.BirthDate.Year < 1900 ||
                model.BirthDate > DateTime.Now)
            {
                throw new TheatreException($"{nameof(model.BirthDate)} is out of the range (The year of 1900 to Now)!");
            }
        }

        public async Task DeleteAsync(int id)
        {
            var customer = await _unitOfWork.CustomerRepository.GetByIdAsync(id);

            if (customer  is not null)
            {
                _unitOfWork.CustomerRepository.Delete(customer);

                bool isPersonStillRegirested = await _unitOfWork.AuthorRepository.ExistsByPersonId(customer.PersonId);
                if (!isPersonStillRegirested)
                {
                    await _unitOfWork.PersonRepository.DeleteByIdAsync(customer.PersonId);
                }
            }

            await _unitOfWork.SaveAsync();
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

            bool isEmailExistsInDb = await _unitOfWork.PersonRepository.ExistsByEmailAsync(model.Email);
            if (isEmailExistsInDb)
            {
                throw new TheatreException("The this email is already in database!");
            }

            var customer = await _unitOfWork.CustomerRepository.GetByIdWithDetailsAsync(model.Id);
            if (customer is null)
            {
                throw new TheatreException($"There is no such ID as {model.Id}!");
            }

            customer.Person.FirstName = model.FirstName;
            customer.Person.LastName = model.LastName;
            customer.Person.BirthDate = model.BirthDate;
            customer.Person.Email = model.Email;

            await _unitOfWork.SaveAsync();
        }

        public async Task<IEnumerable<CustomerModel>> GetCustomersByPerformanceIdAsync(int performanceId)
        {
            var performance = await _unitOfWork.PerformanceRepository.GetByIdWithDetailsAsync(performanceId);
            if (performance is null)
            {
                throw new TheatreException($"There is no such ID as {performanceId}!");
            }

            var customers = performance.Tickets
                .Where(t => t.TicketBooking is not null)
                .Select(t => t.TicketBooking!.Customer);

            return _mapper.Map<IEnumerable<CustomerModel>>(customers);
        }
    }
}
