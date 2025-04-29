using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System.ComponentModel;

namespace FirmaService.Models
{
    public class Dispozitive
    {
        public int DispozitiveID { get; set; }
        public int ClientID { get; set; }
        public string TipDispozitiv {  get; set; }
        public string? Model { get; set; }
        public string? Marca {  get; set; }
        public string Problema {  get; set; }
        public string? Status {  get; set; }
      
    }
}
