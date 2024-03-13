using SqlKata;
using SqlKata.Compilers;
using Storm.Execution;
using Storm.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Storm.Test
{
    public class CommandTest
    {
        private string Checksum(string s)
        {
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                return BitConverter
                    .ToString(md5.ComputeHash(Encoding.UTF8.GetBytes(s)))
                    .Replace("-", String.Empty);
            }
        }

        [Fact]
        public void CreateGetCommand()
        {
            Storm s = StormDefine();

            GetCommand cmd = new GetCommand(s.CreateContext(), "Appointment");
            cmd.With("Contact").With("AssignedUser");

            Assert.Equal("Appointment", cmd.from.root.Entity.Id);
            Assert.Equal(2, cmd.from.root.children.Count);
            Assert.Equal("Contact", cmd.from.root.children[0].Entity.Id);
            Assert.Equal("User", cmd.from.root.children[1].Entity.Id);
        }

        [Fact]
        public void CreateGetCommandWith_Depth_3()
        {
            Storm s = StormDefine();

            GetCommand cmd = new GetCommand(s.CreateContext(), "Appointment");
            cmd.With("Contact").With("AssignedUser").With("Contact.OwnerUser");

            Assert.Equal("Appointment", cmd.from.root.Entity.Id);
            Assert.Equal(2, cmd.from.root.children.Count);
            Assert.Single(cmd.from.root.children.First().children);
            Assert.Equal("Appointment.Contact.OwnerUser", cmd.from.root.children.First().children.First().FullPath.Path);

        }

        [Fact]
        public void Parse_GetCommandWith_Depth_3()
        {
            Storm s = StormDefine();

            GetCommand cmd = new GetCommand(s.CreateContext(), "Appointment");
            cmd.With("Contact").With("AssignedUser").With("Contact.OwnerUser");
            cmd.ParseSQL();

            var compiler = new SqlServerCompiler();
            SqlResult result = compiler.Compile(cmd.query);
            string sql = result.Sql;
            // Previusly Calculated check sum integrity
            Assert.Equal("0B13F4B63EE5535DD7E2E3AC63EDB7FF", Checksum(sql));
        }

        [Fact]
        public void CreateGetCommandWithWhere()
        {
            Storm s = StormDefine();

            GetCommand cmd = new GetCommand(s.CreateContext(), "Appointment");
            cmd.With("Contact")
                .With("AssignedUser")
                .Where(f => f["Contact.LastName"].EqualTo.Val("foo") * f["Contact.FirstName"].EqualTo.Val("boo"));

            Assert.Equal("Appointment", cmd.from.root.Entity.Id);
            var _and =  Assert.IsType<AndFilter>(cmd.where);
            Assert.Equal(2, _and.filters.Count());

            var _eq1 =  Assert.IsType<EqualToFilter>( _and.filters.First());
            Assert.Equal("Contact.LastName", _eq1.Left.Path);
            Assert.Equal("foo", ((DataFilterValue)_eq1.Right).value);

            var _eq2 = Assert.IsType<EqualToFilter>(_and.filters.Last());
            Assert.Equal("Contact.FirstName", _eq2.Left.Path);
            Assert.Equal("boo", ((DataFilterValue)_eq2.Right).value);
        }

        [Fact]
        public void Parse_GetCommandWithWhere()
        {
            Storm s = StormDefine();

            GetCommand cmd = new GetCommand(s.CreateContext(), "Appointment");
            cmd.With("Contact")
                .With("AssignedUser")
                .Where(f => f["Contact.LastName"].EqualTo.Val("foo") * f["Contact.FirstName"].EqualTo.Val("boo"));

            cmd.ParseSQL();

            var compiler = new SqlServerCompiler();
            SqlResult result = compiler.Compile(cmd.query);
            string sql = result.Sql;

            // Previusly Calculated check sum integrity
            Assert.Equal("A024F3D087A2DD1487AFFA4D59BCA657", Checksum(sql));
            Assert.Equal("foo", result.Bindings.First());
            Assert.Equal("boo", result.Bindings.Last());
        }


        [Fact]
        public void Parse_GetCommandWithWhereCol()
        {
            Storm s = StormDefine();

            GetCommand cmd = new GetCommand(s.CreateContext(), "Appointment");
            cmd.With("Contact")
                .Where(f => f["Contact.LastName"].EqualTo.Ref("AssignedUser.LastName"));

            cmd.ParseSQL();

            var compiler = new SqlServerCompiler();
            SqlResult result = compiler.Compile(cmd.query);
            string sql = result.Sql;

            // Previusly Calculated check sum integrity
            Assert.Equal("D9E6E2A3C2A1DE3AE862FB609B8819F8", Checksum(sql));
        }


        [Fact]
        public void Parse_GetCommandWithWhere_ComplexFilter()
        {
            Storm s = StormDefine();

            GetCommand cmd = new GetCommand(s.CreateContext(), "Appointment");
            cmd.Where(f => {
                    return f["Appointment.ID"].GreaterTo.Val(10) * (( f["Contact.LastName"].EqualTo.Ref("AssignedUser.LastName") * f["Contact.LastName"].IsNotNull ) + f["Contact.LastName"].IsNull);
                });

            cmd.ParseSQL();

            var compiler = new SqlServerCompiler();
            SqlResult result = compiler.Compile(cmd.query);
            string sql = result.Sql;

            // Previusly Calculated check sum integrity
            Assert.Equal("398E282B662A45718E2B023406B243BC", Checksum(sql));
            Assert.Equal(10, result.Bindings.First());
        }


        [Fact]
        public void Parse_SelectCommand()
        {
            Storm s = StormDefine();

            SelectCommand cmd = new SelectCommand(s.CreateContext(), "Appointment");
            cmd.Select("Appointment.{ID, Summary}")
                .Select("Contact.*")
                .Select("AssignedUser.ID");


            cmd.ParseSQL();

            var compiler = new SqlServerCompiler();
            SqlResult result = compiler.Compile(cmd.query);
            string sql = result.Sql;

            // Previusly Calculated check sum integrity
            Assert.Equal("0436112BE4650A8FAD71A4CF432A8CA5", Checksum(sql));
        }

        private static Storm StormDefine()
        {
            var s = new Storm();
            s.EditSchema(e =>
            {
                return e.Add<Appointment>("Appointment", "Appointments")
                    .Add<Contact>("Contact", "Contacts")
                    .Add<User>("User", "Users")
                    .Add<Organization>("Organization", "Organizations")
                    .Add<Language>("Language", "Languages")
                    .Add("AppointmentCf", "AppointmentCustomFields", TestingSchema.AppointmentCustomFields)
                    .Connect("Contact", "Appointment", "Contact", "ContactID", "ID")
                    .Connect("OwnerUser","Contact", "User", "OwnerUserID", "ID")
                    .Connect("AssignedUser", "Appointment", "User", "AssignedUserID", "ID")
                    .Connect("AppointmentCustomFields", "Appointment", "AppointmentCf", "ID", "ID");
            });
            return s;
        }
    }
}
