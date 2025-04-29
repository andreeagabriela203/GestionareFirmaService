using Microsoft.AspNetCore.Mvc;

namespace FirmaService.Models
{
    public class InLucru 
    {
        public string Status { get; set; }

       public string TipDispozitiv { get; set; }
        public string Marca { get; set; }
        public string Model { get; set; }
        public string piesa_lipsa { get; set; }
    }
}
