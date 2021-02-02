using System;
using System.Collections.Generic;
using System.Text;
using Storm.Schema;

namespace Storm.Test.PublicAPI.MockAndModels
{
    public class User
    {
        [StormPrimaryKey]
        public int ID;
        public string FirstName;
        public string LastName;
        public string Email;
    }


    public class Task
    {
        [StormPrimaryKey]
        public int ID;
        public int UserID;
        public string TaskType;
        public bool Completed;
        [StormColumnName("ShortDesc")]
        public string Subject;
        public DateTime DateStart;
        public DateTime DateEnd;
    }
}
