namespace CalculIndiceMasseCorpolle.Models
{
    public class IMCRequest
    {
        public string Nom { get; set; } = string.Empty;
        public decimal Poids { get; set; } // en kg
        public decimal Taille { get; set; } // en mètres

    }
}
