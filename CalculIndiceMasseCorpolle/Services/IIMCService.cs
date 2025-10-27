using CalculIndiceMasseCorpolle.Models;

namespace CalculIndiceMasseCorpolle.Services
{
    public interface IIMCService
    {
        IMCResponse CalculerImc(IMCRequest request);
        string DeterminerCategorie(decimal imc);

    }
}
