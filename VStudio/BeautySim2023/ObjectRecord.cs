
using BeautySim2023.DataModel;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace BeautySim2023
{
    /// <summary>
    /// 
    /// </summary>
    public class ObjectRecord
    {
        /// <summary>
        /// 
        /// </summary>
        /// 
        public Results Result;

        public Users Teacher;

        public Users Student;

        public string NameCase;

        public Events EventOrg;

        public DateTime DateTimeResult;

        public ObjectRecord(Results result)
        {
            this.Result = result;
            Student = DBConnector.Instance.FindRowById<Users>(new BsonValue((int)result.IdStudent));
            Teacher = DBConnector.Instance.FindRowById<Users>(new BsonValue((int)result.IdTeacher));
            EventOrg = DBConnector.Instance.FindRowById<Events>(new BsonValue((int)result.IdEvent));

            NameCase = result.CaseName;
            DateTimeResult = DateTime.Parse(result.Date, new CultureInfo(16));

        }
    }
}
