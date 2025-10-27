namespace CalculIndiceMasseCorpolle.Models
{
    public class CalculIMC
    {
        public int Id { get; set; }
        public string Nom { get; set; } = string.Empty;
        public decimal Poids { get; set; }
        public decimal Taille { get; set; }
        public decimal IMC { get; set; }
        public string Categorie { get; set; } = string.Empty;
        public DateTime DateCalcul { get; set; }
    }
}
