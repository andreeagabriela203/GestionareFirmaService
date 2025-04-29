namespace FirmaService.Models
{
    public class Piese
    {
        public int PieseId { get; set; }
        public string? TipPiesa {  get; set; } = null;
        public string? Model {  get; set; }
        public string Marca { get; set; }
        public string TipDispozitiv {  get; set; }
        public int Stoc { get; set; }
        public int Cost {  get; set; }
        
    }
}
