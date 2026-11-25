using Carglass.TechnicalAssessment.Backend.Entities;

namespace Carglass.TechnicalAssessment.Backend.DL.Repositories
{
    public interface IClientRepository : ICrudRepository<Client>
    {
        Client? GetByDocNum(string docNum);
    }
}
