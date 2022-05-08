using Microsoft.AspNetCore.Mvc;

using System.Net;

namespace prjQNAMAker01.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(string question)
        {
            WebClient webClient = new WebClient();
            webClient.Headers.Clear();
            webClient.Headers.Add("Content-Type", "application/json");
            webClient.Headers.Add("Authorization", "EndpointKey f01a6ad0-3c6f-44e1-a3b6-ece3793adb25");

            string jsonQuestion = "{'question':'" + question + "'}";
            byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(jsonQuestion);

            byte[] result = webClient.UploadData
                ("https://qnakevin20220508.azurewebsites.net/qnamaker/knowledgebases/23b979f6-17e3-46b0-926e-67c8c08ca784/generateAnswer", byteArray);

            string answer = System.Text.Encoding.UTF8.GetString(result);

            ViewBag.Question = $"詢問的問題：{question}";
            ViewBag.Answer = $"我們的回答：{answer}";

            return View();
        }
    }
}
