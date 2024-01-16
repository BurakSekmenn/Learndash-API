using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace WebApplication1.Controllers
{
    public class UserController : Controller
    {
        [HttpGet]
        public async Task<ActionResult> GetUserListet()
        {
            string username = "";
            string password = "";
            string apiUrl = "http://localhost:10011/wp-json/ldlms/v2/sfwd-courses";
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
            string username2 = "";
            string password2 = "";
            string apiUrl2 = "http://localhost:10011/wp-json/ldlms/v2/sfwd-courses";
            using (HttpClient client = new HttpClient())
            {
                var byteArray = System.Text.Encoding.UTF8.GetBytes($"{username2}:{password2}");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                HttpResponseMessage response = await client.GetAsync(apiUrl2);

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
        [HttpGet]
        public async Task<IActionResult> GetUserList()
        {
            string username2 = "";
            string password2 = "";
            string apiUrl2 = "http://localhost:10011/wp-json/ldlms/v2/sfwd-courses";

            var courseList = await GetCourses(username2, password2, apiUrl2);

            if (courseList != null)
            {
                ViewBag.CourseList = new SelectList(courseList, "Id", "Title.Rendered");
            }
            else
            {
                ViewBag.CourseList = new SelectList(new List<SelectListItem>());
            }

            return View();
        }
        [HttpPost]
        public async Task<ViewResult> GetUserList(int selectedCourseId)
        {
            string mesaj = "hata";
            string apiEndpoint = $"http://localhost:10011/wp-json/ldlms/v2/sfwd-courses/{selectedCourseId}/users?per_page={100}";
            
            using (HttpClient client = new HttpClient())
            {
                string username = "";
                string password = "";
                string base64Auth = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($"{username}:{password}"));
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", base64Auth);

                HttpResponseMessage response = await client.GetAsync(apiEndpoint);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResult = await response.Content.ReadAsStringAsync();
                    List<UserModel> userList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<UserModel>>(jsonResult);

                    foreach (var user in userList)
                    {
                        string progressApiEndpoint = $"http://localhost:10011/wp-json/ldlms/v2/users/{user.Id}/course-progress/{selectedCourseId}";
                        string usernameTo = "";
                        string passwordTo = "";
                        string base64AuthTo = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($"{usernameTo}:{passwordTo}"));

                        using (HttpClient progressClient = new HttpClient())
                        {
                            progressClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", base64AuthTo);

                            HttpResponseMessage progressResponse = await progressClient.GetAsync(progressApiEndpoint);

                            if (progressResponse.IsSuccessStatusCode)
                            {
                                string progressJsonResult = await progressResponse.Content.ReadAsStringAsync();
                                var progressDataList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<CourseProgressModel>>(progressJsonResult);

                                var currentUser = userList.FirstOrDefault(u => u.Id == user.Id);
                                if (currentUser != null && progressDataList.Count > 0)
                                {
                                    var progressData = progressDataList[0];
                                    currentUser.last_step = progressData.last_step;
                                    currentUser.steps_total = progressData.steps_total;
                                    currentUser.steps_completed = progressData.steps_completed;
                                    currentUser.date_started = progressData.date_started;
                                    currentUser.date_completed = progressData.date_completed;
                                    currentUser.progress_status = progressData.progress_status;
                                }
                                else
                                {
                                    currentUser.last_step = 0;
                                    currentUser.steps_total = 0;
                                    currentUser.steps_completed = 0;
                                    currentUser.date_started = null;
                                    currentUser.date_completed = null;
                                    currentUser.progress_status = "not-started";
                                }
                            }
                            else
                            {
                                await Console.Out.WriteLineAsync("");
                            }
                        }
                    }

                    string username2 = "";
                    string password2 = "";
                    string apiUrl2 = "http://localhost:10011/wp-json/ldlms/v2/sfwd-courses";
                    var courseList = await GetCourses(username2, password2, apiUrl2);
                    var selectList = new SelectList(courseList, "Id", "Title.Rendered");
                    ViewBag.CourseList = selectList;
                    ViewBag.Courseıd = selectedCourseId;
                    return View(userList);
                }
                else
                {
                    return View(mesaj);
                }
            }
        }


        }

        public class UserModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int course { get; set; }
        public int last_step { get; set; }
        public int steps_total { get; set; }
        public int steps_completed { get; set; }
        public string date_started { get; set; }
        public string date_completed { get; set; }

        public string progress_status { get; set; } 

    }
    public class CourseProgressModel
    {
        public int course { get; set; }
        public int last_step { get; set; }
        public int steps_total { get; set; }
        public int steps_completed { get; set; }
        public string date_started { get; set; }
        public string? date_completed { get; set; }
        public string progress_status { get; set; }

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

    public class Title
    {
        public string Rendered { get; set; }
    }

}

