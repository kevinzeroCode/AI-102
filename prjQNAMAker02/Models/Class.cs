namespace prjQNAMAker01.Modles
{
    public class Answer

    {

        public string answer { get; set; }

        public List<string> questions { get; set; }

        public decimal score { get; set; }

    }
    public class QnAresponse

    {

        public List<Answer> answers { get; set; }

    }
}
