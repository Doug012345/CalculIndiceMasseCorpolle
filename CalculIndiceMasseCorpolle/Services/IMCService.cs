using CalculIndiceMasseCorpolle.Models;

namespace CalculIndiceMasseCorpolle.Services
{
    public class IMCService : IIMCService
    {
        public IMCResponse CalculerImc(IMCRequest request)
        {
            if (request.Taille <= 0)
                throw new ArgumentException("La taille doit être supérieure à 0");

            decimal imc = request.Poids / (request.Taille * request.Taille);
            string categorie = DeterminerCategorie(imc);

            return new IMCResponse
            {
                Nom = request.Nom,
                Poids = request.Poids,
                Taille = request.Taille,
                IMC = Math.Round(imc, 2),
                Categorie = categorie,
                DateCalcul = DateTime.Now
            };
        }

        public string DeterminerCategorie(decimal imc)
        {
            return imc switch
            {
                < 16.5m => "Dénutrition",
                >= 16.5m and < 18.5m => "Maigreur",
                >= 18.5m and < 25m => "Corpulence normale",
                >= 25m and < 30m => "Surpoids",
                >= 30m and < 35m => "Obésité modérée",
                >= 35m and < 40m => "Obésité sévère",
                >= 40m => "Obésité morbide",

            };
        }
    
}
}
