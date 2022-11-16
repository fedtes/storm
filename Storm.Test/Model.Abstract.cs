using System;
using System.Collections.Generic;
using System.Text;
using Storm.Schema;

namespace Storm.Test
{
    public class Model_0
    {
        [StormPrimaryKey]
        public int ID;

        public int Model1ID;

        public string Field1;

        public string Field2;

        public string Field3;

        public string Field4;

        public string Field5;
    }

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

    public class Model_5
    {
        [StormPrimaryKey]
        public int ID;

        public int ParentID;

        public string ParentData;
    }

}
