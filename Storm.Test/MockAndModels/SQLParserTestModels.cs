using System;
using System.Collections.Generic;
using System.Text;
using Storm;
using Storm.Schema;

namespace Storm.Test.MockAndModels
{
    public class Model_1
    {
        [StormPrimaryKey]
        public int ID;

        public string data;
    }


    public class Model_2
    {
        [StormPrimaryKey]
        public int ID;

        public int ParentID;

        public string data;
    }


    public class Model_3
    {
        [StormPrimaryKey]
        public int ID;

        public int ParentID;

        public string data;
    }

    public class Model_4
    {
        [StormPrimaryKey]
        public int ID;

        public int ParentID;

        public string data;
    }
}
