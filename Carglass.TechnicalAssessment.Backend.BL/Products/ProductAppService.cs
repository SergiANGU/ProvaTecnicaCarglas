using AutoMapper;
using Carglass.TechnicalAssessment.Backend.DL.Repositories;
using Carglass.TechnicalAssessment.Backend.Dtos;
using Carglass.TechnicalAssessment.Backend.Entities;
using FluentValidation;

namespace Carglass.TechnicalAssessment.Backend.BL;

internal class ProductAppService : IProductAppService
{
    private readonly ICrudRepository<Product> _productRepository;
    private readonly IMapper _mapper;
    private readonly IValidator<ProductDto> _productValidator;

    public ProductAppService(
        ICrudRepository<Product> productRepository,
        IMapper mapper,
        IValidator<ProductDto> productValidator)
    {
        _productRepository = productRepository;
        _mapper = mapper;
        _productValidator = productValidator;
    }

    public IEnumerable<ProductDto> GetAll()
    {
        var products = _productRepository.GetAll();
        return _mapper.Map<IEnumerable<ProductDto>>(products);
    }

    public ProductDto GetById(params object[] keyValues)
    {
        var product = _productRepository.GetById(keyValues);
        return _mapper.Map<ProductDto>(product);
    }

    public void Create(ProductDto productDto)
    {
        if (_productRepository.GetById(productDto.Id) is not null)
        {
            throw new Exception("Ya existe un producto con este Id");
        }

        ValidateDto(productDto);

        var product = _mapper.Map<Product>(productDto);
        _productRepository.Create(product);
    }

    public void Update(ProductDto productDto)
    {
        if (_productRepository.GetById(productDto.Id) is null)
        {
            throw new Exception("No existe ningún producto con este Id");
        }

        ValidateDto(productDto);

        var product = _mapper.Map<Product>(productDto);
        _productRepository.Update(product);
    }

    public void Delete(ProductDto productDto)
    {
        if (_productRepository.GetById(productDto.Id) is null)
        {
            throw new Exception("No existe ningún producto con este Id");
        }

        var product = _mapper.Map<Product>(productDto);
        _productRepository.Delete(product);
    }

    private void ValidateDto(ProductDto productDto)
    {
        var validationResult = _productValidator.Validate(productDto);
        if (!validationResult.IsValid)
        {
            string errorMessages = string.Join("; ", validationResult.Errors.Select(error => error.ErrorMessage));
            throw new Exception($"El producto especificado no cumple los requisitos de validación. Errores: '{errorMessages}'");
        }
    }
}
