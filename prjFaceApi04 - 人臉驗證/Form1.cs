using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;

namespace prjFaceApi04人臉驗證
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        string apiUrl, apiKey;

        private void Form1_Load(object sender, EventArgs e)
        {
            //放入api網址
            apiKey = "eca7a5f3a8804bb4b3ecbfce05b0e619";
            apiUrl = "https://kevinfaceapi0507.cognitiveservices.azure.com/";
        }

        private async Task<string> GetFaceId
           (string _imgPath, string _apiUrl, string _apiKey)
        {
            FileStream fileStream = File.OpenRead(_imgPath);
            FaceClient faceClient = new FaceClient
                (new ApiKeyServiceClientCredentials(_apiKey),
                new System.Net.Http.DelegatingHandler[] { });
            faceClient.Endpoint = _apiUrl;

            IList<DetectedFace> detectedFaces = await
                faceClient.Face.DetectWithStreamAsync(fileStream,
                   detectionModel: DetectionModel.Detection03,
                   recognitionModel: RecognitionModel.Recognition04);

            if (detectedFaces.Count > 0)
            {
                return detectedFaces[0].FaceId.ToString();
            }
            else
            {
                return "無法取得Face Id，請重新選一張照片";
            }
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                pictureBox2.Image = new Bitmap(openFileDialog1.FileName);
                richTextBox2.Text = await GetFaceId(openFileDialog1.FileName, apiUrl, apiKey);
            };
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void richTextBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private async void button3_Click(object sender, EventArgs e)
        {
            if (richTextBox1.Text != "" || richTextBox2.Text != "")
            {
                FaceClient faceClient = new FaceClient
                    (new ApiKeyServiceClientCredentials(apiKey),
                    new System.Net.Http.DelegatingHandler[] { });
                faceClient.Endpoint = apiUrl;
                //比較Face Id
                VerifyResult result = await faceClient.Face.VerifyFaceToFaceAsync
                    (
                      System.Guid.Parse(richTextBox1.Text),
                      System.Guid.Parse(richTextBox2.Text)
                    );
                string msg = $"是否為同一人：{result.IsIdentical}\n" +
                    $"相似度：{result.Confidence}";
                MessageBox.Show(msg);
            }
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Image = new Bitmap(openFileDialog1.FileName);
                richTextBox1.Text = await GetFaceId(openFileDialog1.FileName, apiUrl, apiKey);
            };
        }
    }
}