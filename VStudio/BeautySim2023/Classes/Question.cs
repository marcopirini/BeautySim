using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautySim2023
{
    public class Question
    {
        public string QuestionText { get; set; }
        public List<string> Options { get; set; }
        public List<string> CorrectAnswers { get; set; }
        public string Explanation { get; set; }
        public float Weight { get; set; }
    }
}
