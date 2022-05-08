using Microsoft.AspNetCore.Mvc;

using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;

namespace prjWebCv01.Controllers
{
    public class HomeController : Controller
    {

        string _path = "";
        public HomeController(IWebHostEnvironment hostEnvironment)
        {
            _path = $@"{hostEnvironment.WebRootPath}\Images";
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(IFormFile formFile)
        {
            if(formFile != null)
            {
                if(formFile.Length > 0)
                {
                    string fileName = formFile.FileName;
                    string savePath = $@"{_path}\{fileName}";
                    using (var stream = new FileStream(savePath, FileMode.Create))
                    {
                        await formFile.CopyToAsync(stream);
                    }


                    //分析影像
                    try
                    {
                        string cvApiUrl, cvApiKey;
                        cvApiKey = "";
                        cvApiUrl = "";

                        FileStream fs = System.IO.File.Open(savePath, FileMode.Open);    //開檔
                        ComputerVisionClient computerVisionClient =
                            new ComputerVisionClient
                            (new ApiKeyServiceClientCredentials(cvApiKey),
                            new System.Net.Http.DelegatingHandler[] { });
                        computerVisionClient.Endpoint = cvApiUrl;

                        //指定視覺特徵
                        VisualFeatureTypes?[] visualFeatureTypes = new VisualFeatureTypes?[]
                        {
                        VisualFeatureTypes.ImageType,
                        VisualFeatureTypes.Color,
                        VisualFeatureTypes.Faces,
                        VisualFeatureTypes.Adult,
                        VisualFeatureTypes.Categories,
                        VisualFeatureTypes.Tags,
                        VisualFeatureTypes.Description,
                        VisualFeatureTypes.Objects
                        };

                        ImageAnalysis result = await
                            computerVisionClient.AnalyzeImageInStreamAsync(fs, visualFeatureTypes);


                        if (result == null)
                        {
                            ViewBag.Show = "無法分析";
                            return View();
                        }


                        string str = $"<p><img src='Images/{fileName}' width='240'><p>";
                        for (int i = 0; i < result.Description.Captions.Count; i++)
                        {
                            str += $"影像描述：{result.Description.Captions[i].Text}\n" +
                                $"描述信度：{result.Description.Captions[i].Confidence}\n";
                        }
                        str += $"<hr>臉部資訊\n";
                        for (int i = 0; i < result.Faces.Count; i++)
                        {
                            str += $"\t{result.Faces[i].Gender}\t{result.Faces[i].Age}\n";
                        }
                        str += $"<hr>標籤資訊\n";
                        for (int i = 0; i < result.Tags.Count; i++)
                        {
                            str += $"\t{result.Tags[i].Name}\t{result.Tags[i].Confidence}\n";
                        }

                        str += $"<hr>成人資訊：{result.Adult.IsAdultContent}\t{result.Adult.AdultScore}\n";
                        str += $"兒童不宜：{result.Adult.IsRacyContent}\t{result.Adult.RacyScore}\n";
                        str += $"血腥資訊：{result.Adult.IsGoryContent}\t{result.Adult.GoreScore}\n";

                        if (result.Categories[0].Detail != null)
                        {
                            //印出名人
                            if (result.Categories[0].Detail.Celebrities != null)
                            {
                                str += $"<hr>名人資訊如下：\n";
                                for (int i = 0; i < result.Categories[0].Detail.Celebrities.Count; i++)
                                {
                                    str += $"\t{result.Categories[0].Detail.Celebrities[i].Name}" +
                                        $"\t{result.Categories[0].Detail.Celebrities[i].Confidence}\n";
                                }
                            }

                            //印出地標
                            if (result.Categories[0].Detail.Landmarks != null)
                            {
                                str += $"<hr>地標資訊如下：\n";
                                for (int i = 0; i < result.Categories[0].Detail.Landmarks.Count; i++)
                                {
                                    str += $"\t{result.Categories[0].Detail.Landmarks[i].Name}" +
                                        $"\t{result.Categories[0].Detail.Landmarks[i].Confidence}\n";
                                }
                            }
                        }

                        ViewBag.Show = str.Replace("\n", "<br>").Replace("\t", "　　");

                        fs.Close();
                        fs.Dispose();
                    }
                    catch (Exception ex)
                    {
                        ViewBag.Show = ex.Message;
                    }






                }
            }





            return View();
        }


    }
}
