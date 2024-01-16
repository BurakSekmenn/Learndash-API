using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace WebApplication1.Controllers
{
    public class CourseController : Controller
    {
        

        public class Title
        {
            public string Rendered { get; set; }
        }
        public async Task<ActionResult> Index()
        {
            string apiUrl = "http://localhost:10011/wp-json/ldlms/v2/sfwd-courses";
            string username = "";
            string password = "";

            var courses = await GetCourses(apiUrl, username, password);

            if (courses != null && courses.Length > 0)
            {
                return View(courses);
            }
            else
            {
                return View("Error");
            }
        }
        private async Task<Course[]> GetCourses(string apiUrl, string username, string password)
        {
            using (HttpClient client = new HttpClient())
            {
                // Temel kimlik doğrulama bilgilerini ekleyin
                var byteArray = System.Text.Encoding.UTF8.GetBytes($"{username}:{password}");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                HttpResponseMessage response = await client.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    var courses = JsonConvert.DeserializeObject<Course[]>(data);

                    return courses;
                }
                else
                {
                    Console.WriteLine($"Hata: {response.StatusCode}, {await response.Content.ReadAsStringAsync()}");
                    return null;
                }
            }
        }





        public class User
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        public class Course
        {
            public int Id { get; set; }
            public Title Title { get; set; }
        }
    }
}

