using AutoMapper;
using Carglass.TechnicalAssessment.Backend.DL.Repositories;
using Carglass.TechnicalAssessment.Backend.Dtos;
using Carglass.TechnicalAssessment.Backend.Entities;
using FluentValidation;

namespace Carglass.TechnicalAssessment.Backend.BL;

internal class ClientAppService : IClientAppService
{
    private readonly IClientRepository _clientRepository;
    private readonly IMapper _mapper;
    private readonly IValidator<ClientDto> _clientValidator;

    public ClientAppService(
    IClientRepository clientRepository,
    IMapper mapper,
    IValidator<ClientDto> clientValidator)
    {
        _clientRepository = clientRepository;
        _mapper = mapper;
        _clientValidator = clientValidator;
    }

    public IEnumerable<ClientDto> GetAll()
    {
        var clients = _clientRepository.GetAll();
        return _mapper.Map<IEnumerable<ClientDto>>(clients);
    }

    public ClientDto GetById(params object[] keyValues)
    {
        var client = _clientRepository.GetById(keyValues);
        return _mapper.Map<ClientDto>(client);
    }

    public void Create(ClientDto clientDto)
    {
        if (null != _clientRepository.GetById(clientDto.Id))
        {
            throw new Exception("Ya existe un cliente con este Id");
        }

        if (null != _clientRepository.GetByDocNum(clientDto.DocNum))
        {
            throw new Exception("Ya existe un cliente con este número de documento");
        }

        ValidateDto(clientDto);

        var client = _mapper.Map<Client>(clientDto);
        _clientRepository.Create(client);
    }

    public void Update(ClientDto clientDto)
    {
        if (null == _clientRepository.GetById(clientDto.Id))
        {
            throw new Exception("No existe ningún cliente con este Id");
        }

        ValidateDto(clientDto);

        var client = _mapper.Map<Client>(clientDto);
        _clientRepository.Update(client);
    }

    public void Delete(ClientDto clientDto)
    {
        if (null == _clientRepository.GetById(clientDto.Id))
        {
            throw new Exception("No existe ningún cliente con este Id");
        }

        var client = _mapper.Map<Client>(clientDto);
        _clientRepository.Delete(client);

    }

    private void ValidateDto(ClientDto clientDto)
    {
        var validationResult = _clientValidator.Validate(clientDto);
        if (validationResult.Errors.Any())
        {
            string toShowErrors = string.Join("; ", validationResult.Errors.Select(s => s.ErrorMessage));
            throw new Exception($"El cliente especificado no cumple los requisitos de validación. Errores: '{toShowErrors}'");
        }
    }
}