using Storm.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Storm.Test
{
    public class SelectCommandHelperTest
    {
        [Fact]
        public void FailToParse_StringPath()
        {
            var p = "Appointment.AssignedUser. ";
            Assert.Throws<ArgumentException>(() => SelectCommandHelper.ValidatePath(p));
        }

        [Fact]
        public void Parse_OneField_StringPath()
        {
            var p = "Appointment.AssignedUser.FirstName";
            var r = SelectCommandHelper.ValidatePath(p);
            Assert.Single(r);
            var (path, field) = r.First();
            Assert.Equal("Appointment.AssignedUser", String.Join('.', path));
            Assert.Equal("FirstName", field);
        }

        [Fact]
        public void Parse_WildCard_StringPath()
        {
            var p = "Appointment.AssignedUser.*";
            var r = SelectCommandHelper.ValidatePath(p);
            Assert.Single(r);
            var (path, field) = r.First();
            Assert.Equal("Appointment.AssignedUser", String.Join('.', path));
            Assert.Equal("*", field);
        }

        [Fact]
        public void Parse_MultiField_StringPath()
        {
            var p = "Appointment.AssignedUser.{LastName, FirstName , LanguageID,ID}";
            var r = SelectCommandHelper.ValidatePath(p);
            Assert.Equal(4, r.Count());
            var r1 = r.ToArray();
            var (path0, field0) = r1[0];
            Assert.Equal("Appointment.AssignedUser", String.Join('.', path0));
            Assert.Equal("LastName", field0);
            var (path1, field1) = r1[1];
            Assert.Equal("Appointment.AssignedUser", String.Join('.', path1));
            Assert.Equal("FirstName", field1);
            var (path2, field2) = r1[2];
            Assert.Equal("Appointment.AssignedUser", String.Join('.', path2));
            Assert.Equal("LanguageID", field2);
            var (path3, field3) = r1[3];
            Assert.Equal("Appointment.AssignedUser", String.Join('.', path3));
            Assert.Equal("ID", field3);
        }
    }
}
