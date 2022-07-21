using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Dynamic;
using System.Reflection;
using System.IO;
using System.Threading;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController : Controller
    {
        private readonly IConfiguration _config;
        public TestController(IConfiguration config)
        {
            _config = config;
        }

        [Route("getAllBooks")]
        [HttpGet]
        public IActionResult GetAllBooks()
        {
            var bookList = GenerateBookList();
            return Ok(JsonConvert.SerializeObject(bookList));
        }

        [Route("addBook")]
        [HttpPost]
        public IActionResult AddBook(BookModel book)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid data.");
            var count = GenerateBookList().Count;
            book.Title = "api-" + book.Title;
            AddtoJson(book);
            Thread.Sleep(600);
            var count1 = GenerateBookList().Count;
            if (count1 - count == 1)
            {
                return Ok();
            }
            else
                return StatusCode(500);


        }

        [Route("getAllCompany")]
        [HttpGet]
        public IActionResult GetAllCompany()
        {
            try
            {
                var companyList = GenerateCompanyList();
                return Ok(JsonConvert.SerializeObject(companyList));
            }
            catch (Exception ex)
            {
                return StatusCode(503, new
                {
                    Message = "Failed to get the List",
                    Exception = new
                    {
                        Message = ex.Message,
                        StackTrace = ex.StackTrace
                    }
                });                
            }
        }

        private List<BookModel> GenerateBookList()
        {
            var bookList = new List<BookModel>();
            for (var i = 0; i <= 100; i++)
            {
                if (_config.GetValue<int>($"Book{i + 1}:Id") > 0)
                {
                    BookModel book = new BookModel();
                    book.Id = _config.GetValue<int>($"Book{i + 1}:Id");
                    book.Title = _config.GetValue<string>($"Book{i + 1}:Title");
                    book.Genre = _config.GetValue<string>($"Book{i + 1}:Genre");
                    book.Price = _config.GetValue<decimal>($"Book{i + 1}:Price");
                    book.Company = _config.GetValue<string>($"Book{i + 1}:Company");
                    bookList.Add(book);
                }
                else
                    break;
            }
            return bookList;
        }

        private List<CompanyModel> GenerateCompanyList()
        {
            var companyList = new List<CompanyModel>();
            for (var i = 0; i <= 100; i++)
            {
                if (_config.GetValue<int>($"Company{i + 1}:Id") > 0)
                {
                    CompanyModel Company = new CompanyModel();
                    Company.Id = _config.GetValue<int>($"Company{i + 1}:Id");
                    Company.Name = _config.GetValue<string>($"Company{i + 1}:Name");
                    Company.City = _config.GetValue<string>($"Company{i + 1}:City");
                    companyList.Add(Company);
                }
                else
                    break;
            }
            return companyList;
        }

        private void AddtoJson<Object>(Object T)
        {
            var appSettingsPath = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "appsettings.json");
            var json = System.IO.File.ReadAllText(appSettingsPath);

            var jsonSettings = new JsonSerializerSettings();
            jsonSettings.Converters.Add(new ExpandoObjectConverter());
            jsonSettings.Converters.Add(new StringEnumConverter());

            dynamic config = JsonConvert.DeserializeObject<ExpandoObject>(json, jsonSettings);

            dynamic newinput = JsonConvert.DeserializeObject<ExpandoObject>(JsonConvert.SerializeObject(T), jsonSettings);

            Type t = T.GetType();
            PropertyInfo[] props = t.GetProperties();

            var expando = config as IDictionary<string, object>;
            expando.Add($"{t.Name.Remove(t.Name.Length - 5)}{props[0].GetValue(T)}", newinput);

            var newJson = JsonConvert.SerializeObject(config, Formatting.Indented, jsonSettings);

            System.IO.File.WriteAllText(appSettingsPath, newJson);
        }
    }
}


// https://www.pragimtech.com/blog/blazor/call-rest-api-from-blazor/
// https://www.tutorialsteacher.com/webapi/implement-post-method-in-web-api
