using System;

namespace Storm.Test
{
    public class TwoEntityMockDataSources
    {
        public MockReader GetReader() => new MockReader(source, names);

        public String[] names = new string[]
        {
            "A1$ID",
            "A1$FirstName",
            "A1$LastName",
            "A1$Email",
            "A1$Mobile",
            "A1$Phone",
            "A1$Address",
            "A2$ID",
            "A2$Info1",
            "A2$Info2",
            "A2$Info3"
        };

        public object[][] source = new object[][]
        {
            new object[] {1, "Mario", "Bianchi", "asd@mail.it", "3331313131", "04512364", "street 1", 1, "Pet", "Food", "Sport" },
            new object[] {2, "Maria", "Rima", "fgh@mail.it", "66565565", "04512364", "street 2", 2, "Movies", "Food", "Games"  },
            new object[] {3, "Andre", "Est", "ajkl@mail.it", "5545458786", "04512364", "street 12", 3, "Games", "Pet", "Sport"  },
            new object[] {4, "Luca", "Verdi", "jkl@mail.it", "54555656", "04512364", "street 13", 4, "Travels", "Food", "Games"  },
            new object[] {5, "Gino", "Russo", "zxc@mail.it", "7898787987", "04512364", "street 11", 5, "Travels", "Food", "Sport"  },
            new object[] {6, "Mario", "Amer", "acvb@mail.it", "788798998", "04512364", "street 2", 6, "", "", ""  },
            new object[] {7, "Paride", "Paris", "bnm@mail.it", "54566569665", "04512364", "street 31", 7, "", "", ""  },
            new object[] {8, "Fede", "Rico", "qwe@mail.it", "124554878", "04512364", "street 51", 8, "", "", ""  },
            new object[] {9, "Luca", "Serio", "ert@mail.it", "21385438384", "04512364", "street 671", 9, "", "", ""  },
            new object[] {10, "Lucia", "Merlo", "tyu@mail.it", "21245683531", "04512364", "street 231", 10, "", "", ""  },
            new object[] {11, "Doria", "Galli", "yui@mail.it", "2135465926", "04512364", "street 41", 11, "", "", ""  },
            new object[] {12, "Mattia", "Polo", "uio@mail.it", "1213843483", "04512364", "street 2341", 12, "", "", ""  },
            new object[] {13, "Mattia", "Denim", "aoip@mail.it", "2323456684", "04512364", "street 41", 13, "", "", ""  },
            new object[] {14, "Mara", "Ciano", "dfg@mail.it", "23235648468", "04512364", "street 451", 14, "", "", ""  },
            new object[] {15, "Iole", "Amer", "ghj@mail.it", "35454684", "04512364", "street 156", 15, "", "", ""  },
            new object[] {16, "Laura", "Giallo", "kjl@mail.it", "5435135", "04512364", "street 123", 16, "", "", ""  },
            new object[] {17, "Lucio", "Verdi", "zxc@mail.it", "65989896565", "04512364", "street 14", 17, "", "", ""  },
            new object[] {18, "Mario", "Rossi", "cvb@mail.it", "33333133331", "04512364", "street 12", 18, "Pet", "Food", "Travels" },
            new object[] {18, "Mario", "Rossi", "cvb@mail.it", "33333133331", "04512364", "street 12", 18, "Game", "Sleep", "Running" },
            new object[] {18, "Mario", "Rossi", "cvb@mail.it", "33333133331", "04512364", "street 12", 18, "Food", "Drinks", "Movie" },
            new object[] {18, "Mario", "Rossi", "cvb@mail.it", "33333133331", "04512364", "street 12", 18, "Music", "Game", "Coding" },
            new object[] {18, "Mario", "Rossi", "cvb@mail.it", "33333133331", "04512364", "street 12", 18, "Cat", "Dog", "Snake" }
        };

    }

    public class TwoEntityMockDataSources2
    {
        public MockReader GetReader() => new MockReader(source, names);

        public String[] names = new string[]
        {
            "A1$ID",
            "A1$FirstName",
            "A1$LastName",
            "A1$Email",
            "A1$Mobile",
            "A1$Phone",
            "A1$Address",
            "A2$ID",
            "A2$ParentID",
            "A2$Info1",
            "A2$Info2",
            "A2$Info3"
        };

        public object[][] source = new object[][]
        {
            new object[] {18, "Mario", "Rossi", "cvb@mail.it", "33333133331", "04512364", "street 12", 1, 18, "Pet", "Food", "Travels" },
            new object[] {18, "Mario", "Rossi", "cvb@mail.it", "33333133331", "04512364", "street 12", 2, 18, "Game", "Sleep", "Running" },
            new object[] {18, "Mario", "Rossi", "cvb@mail.it", "33333133331", "04512364", "street 12", 3, 18, "Food", "Drinks", "Movie" },
            new object[] {18, "Mario", "Rossi", "cvb@mail.it", "33333133331", "04512364", "street 12", 4, 18, "Music", "Game", "Coding" },
            new object[] {18, "Mario", "Rossi", "cvb@mail.it", "33333133331", "04512364", "street 12", 5, 18, "Cat", "Dog", "Snake" },
            new object[] {17, "Lucio", "Verdi", "zxc@mail.it", "65989896565", "04512364", "street 14", 6, 17, "", "", ""  }
        };

    }
}
