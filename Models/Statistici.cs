using Microsoft.AspNetCore.Mvc;

namespace FirmaService.Models
{
    public class Statistici
    {
        public string Nume { get; set; }
        public string Prenume { get; set; }

        public int NrDispozitive { get; set; }
        public int Cost { get; set; }
        public decimal VenitperAngajat { get; set; }

        public string PiesaUtilizata { get; set; }
        public int Decateori { get; set; }
        public int Stoc { get; set; }

        public List<string> TipPiesa { get; set; }
        public List<string> Marca {  get; set; }
        public List<string> Model {  get; set; }
        public List<int> StocPiesaNeutilizata {  get; set; }
        public List<int> CostPiesaNeutilizata { get; set; }

        public List<String> Disp {  get; set; }
        public List <decimal> CostMediu { get; set; }

    }
    public class NuAuLucrat
    {
        public string NumeAngajat { get; set; }
        public string PrenumeAngajat { get; set; }
        public int Salariu { get; set; }
    }

}
