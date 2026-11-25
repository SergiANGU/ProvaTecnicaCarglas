using Carglass.TechnicalAssessment.Backend.Entities;

namespace Carglass.TechnicalAssessment.Backend.DL.Repositories;

public class ClientIMRepository : IClientRepository
{
    private ICollection<Client> _clients;

    public ClientIMRepository()
    {
        _clients = new HashSet<Client>()
        {
            new Client()
            {
                Id = 1,
                DocType = "nif",
                DocNum = "11223344E",
                Email = "eromani@sample.com",
                GivenName = "Enriqueta",
                FamilyName1 = "Romani",
                Phone = "668668668"
            }
        };
    }

    public IEnumerable<Client> GetAll()
    {
        return _clients.ToList();
    }

    public Client? GetById(params object[] keyValues)
    {
        return _clients.SingleOrDefault(x => x.Id.Equals(keyValues[0]));
    }

    public void Create(Client item)
    {
        _clients.Add(item);
    }

    public void Update(Client item)
    {
        var existing = _clients.Single(x => x.Id.Equals(item.Id));
        existing.DocType = item.DocType;
        existing.DocNum = item.DocNum;
        existing.Email = item.Email;
        existing.GivenName = item.GivenName;
        existing.FamilyName1 = item.FamilyName1;
        existing.Phone = item.Phone;
    }

    public void Delete(Client item)
    {
        var toDeleteItem = _clients.Single(x => x.Id.Equals(item.Id));

        _clients.Remove(toDeleteItem);
    }

    public Client? GetByDocNum(string docNum)
    {
        return _clients.FirstOrDefault(x =>
            string.Equals(x.DocNum, docNum, StringComparison.OrdinalIgnoreCase));
    }
}
