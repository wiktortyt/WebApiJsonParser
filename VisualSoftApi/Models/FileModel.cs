using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace VisualSoftApi.Models
{
    public class FileModel
    {
        public List<Header> Headers { get; set; } = new List<Header>();
        public List<Cargo> Cargos { get; set; } = new List<Cargo>();
        public List<string> Comments { get; set; } = new List<string>();


        public long CharCount { get; set; }
        public int LineCount { get; set; }
        public int CargosCount { get; set; }
        public int DocsWithCargosGreaterThanX { get; set; }
        public string ProductsWithMaxNetValue { get; set; }


    }
    public class Header
    {
        public string KodBA { get; set; }
        public string Typ { get; set; }
        public string NrDokumentu { get; set; }
        public DateTime DataOperacji { get; set; }
        public int NumerDniaDokumentu { get; set; }
        public string KodKontrahenta { get; set; }
        public string NazwaKontrahenta { get; set; }
        public int NrDokZewnetrznego { get; set; }
        public DateTime DataDokZewnetrznego { get; set; }
        public decimal Netto { get; set; }
        public decimal Vat { get; set; }
        public decimal Brutto { get; set; }
        public string F1 { get; set; }
        public string F2 { get; set; }
        public string F3 { get; set; }
        
        [JsonIgnore]
        public int CargosCount { get; set; }

        [JsonIgnore]
        public List<Cargo> Cargos { get; set; } = new List<Cargo>();

    }

    public class Cargo
    {
        public string KodProduktu { get; set; }
        public string NazwaProduktu { get; set; }
        public decimal Ilosc { get; set; }
        public decimal CenaNetto { get; set; }
        public decimal WartoscNetto { get; set; }
        public decimal Vat { get; set; }
        public decimal IloscPrzed { get; set; }
        public decimal AvgPrzed { get; set; }
        public decimal IloscPo { get; set; }
        public decimal AvgPo { get; set; }
        public string Grupa { get; set; }

    }
}
