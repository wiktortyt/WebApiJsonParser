using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using VisualSoftApi.Models;
using VisualSoftApi.Services.Authentication;

namespace VisualSoftApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class testController : ControllerBase
    {
        [HttpPost]
        [BasicAuthorization]
        [Route("{x:int}")]
        public IActionResult Post(int x, IFormFile file)
        {
            try
            {

                Stream stream = file.OpenReadStream();
                StreamReader reader = new StreamReader(stream);
                if (reader.BaseStream.Length == 0)
                    return BadRequest("Nie podano danych w pliku");

                FileModel model = CreateModel(reader, x);



                string modelSerialized = JsonSerializer.Serialize(model, typeof(FileModel));
                return new JsonResult(modelSerialized);
            }
            catch (IndexOutOfRangeException ex)
            {
                return BadRequest("Błąd w formacie" + ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest("Błąd przy procesowaniu: " + ex.Message);

            }
        }

        private FileModel CreateModel(StreamReader reader, int x)
        {
            try
            {
                string line;
                FileModel model = new FileModel();

                //because counting headers from zero
                int headerCounter = -1;
                int lineCount = 0;
                int cargosCount = 0;

                while ((line = reader.ReadLine()) != null)
                {
                    char c = line[0];
                    lineCount++;

                    switch (c)
                    {
                        case 'H':
                            Header header = parseHeader(line);
                            model.Headers.Add(header);
                            headerCounter++;
                            break;
                        case 'B':
                            Cargo cargo = parseCargo(line);
                            model.Cargos.Add(cargo);
                            model.Headers[headerCounter].Cargos.Add(cargo);
                            cargosCount++;
                            break;
                        case 'C':
                            string comment = line;
                            model.Comments.Add(comment);
                            break;
                        default:
                            break;
                    }
                }

                model.CharCount = reader.BaseStream.Length;
                reader.BaseStream.Seek(0, SeekOrigin.Begin);
                model.LineCount = lineCount;
                model.CargosCount = cargosCount;
                model.DocsWithCargosGreaterThanX = model.Headers.Where(c=>c.Cargos.Count > x).Count();

                //creating products with maxNetValue
                decimal maxNet = model.Cargos.Max(x => x.WartoscNetto);
                IEnumerable<string> cargosWithMaxNetList = model.Cargos.Where(x => x.WartoscNetto == maxNet).Select(x => x.NazwaProduktu);
                string cargosWithMaxNet = string.Join(',', cargosWithMaxNetList);
                model.ProductsWithMaxNetValue = cargosWithMaxNet;

                return model;
            }
            catch (Exception ex)
            {
                throw new Exception("Błąd przy tworzeniu modelu: " + ex.Message);
            }
        }

        private Cargo parseCargo(string line)
        {
            try
            {
                string[] parts = line.Split(',');
                Cargo cargo = new Cargo()
                {
                    KodProduktu = parts[1],
                    NazwaProduktu = parts[2],
                    Ilosc = decimal.Parse(parts[3], CultureInfo.InvariantCulture),
                    CenaNetto = decimal.Parse(parts[4], CultureInfo.InvariantCulture),
                    WartoscNetto = decimal.Parse(parts[5], CultureInfo.InvariantCulture),
                    Vat = decimal.Parse(parts[6], CultureInfo.InvariantCulture),
                    IloscPrzed = decimal.Parse(parts[7], CultureInfo.InvariantCulture),
                    AvgPrzed = decimal.Parse(parts[8], CultureInfo.InvariantCulture),
                    IloscPo = decimal.Parse(parts[9], CultureInfo.InvariantCulture),
                    AvgPo = decimal.Parse(parts[10], CultureInfo.InvariantCulture),
                    Grupa = parts[11]
                };
                return cargo;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw new Exception("Błąd przy procesowaniu Towaru");
            }
        }

        private Header parseHeader(string line)
        {
            try
            {
                string[] parts = line.Split(',');
                Header header = new Header()
                {
                    KodBA = parts[1],
                    Typ = parts[2],
                    NrDokumentu = parts[3],
                    DataOperacji = DateTime.ParseExact(parts[4], "dd-MM-yyyy", CultureInfo.InvariantCulture),
                    NumerDniaDokumentu = int.Parse(parts[5], CultureInfo.InvariantCulture),
                    KodKontrahenta = parts[6],
                    NazwaKontrahenta = parts[7],
                    //NrDokZewnetrznego = int.Parse(parts[8], CultureInfo.InvariantCulture),
                    DataDokZewnetrznego = DateTime.ParseExact(parts[9], "dd-MM-yyyy", CultureInfo.InvariantCulture),
                    Netto = decimal.Parse(parts[10], CultureInfo.InvariantCulture),
                    Vat = decimal.Parse(parts[11], CultureInfo.InvariantCulture),
                    Brutto = decimal.Parse(parts[12], CultureInfo.InvariantCulture),
                    F1 = parts[13],
                    F2 = parts[14],
                    F3 = parts[15]
                };
                return header;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw new Exception("Błąd przy procesowaniu Headera");
            }
        }
    }
}
