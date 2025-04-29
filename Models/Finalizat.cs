using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

namespace FirmaService.Models
{
    public class Finalizat
    {
            public string ClientNume { get; set; }
            public string ClientPrenume { get; set; }
            public string TipDispozitiv { get; set; }
 
            public string Status { get; set; }
            public decimal Cost { get; set; }

      

    }
}
