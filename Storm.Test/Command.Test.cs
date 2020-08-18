﻿using Storm.Execution;
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
        [Fact]
        public void CreateGetCommand()
        {
            Storm s = StormDefine();

            GetCommand cmd = new GetCommand(s.schema.GetNavigator(), "Appointment");
            cmd.With("Contact").With("AssignedUser");

            Assert.Equal("Appointment", cmd.fromTree.Entity.ID);
            Assert.Equal(2, cmd.fromTree.children.Count);
            Assert.Equal("Contact", cmd.fromTree.children[0].Entity.ID);
            Assert.Equal("User", cmd.fromTree.children[1].Entity.ID);
        }

        [Fact]
        public void CreateGetCommandTLMoreDepth()
        {
            Storm s = StormDefine();

            GetCommand cmd = new GetCommand(s.schema.GetNavigator(), "Appointment");
            cmd.With("Contact").With("AssignedUser").With("Contact.OwnerUser");

            Assert.Equal("Appointment", cmd.fromTree.Entity.ID);
            Assert.Equal(2, cmd.fromTree.children.Count);
            Assert.Single(cmd.fromTree.children.First().children);
            Assert.Equal("Appointment.Contact.OwnerUser", cmd.fromTree.children.First().children.First().FullPath);

        }

        [Fact]
        public void CreateGetCommandWithWhere()
        {
            Storm s = StormDefine();

            GetCommand cmd = new GetCommand(s.schema.GetNavigator(), "Appointment");
            cmd.With("Contact")
                .With("AssignedUser")
                .Where(f => f["Contact.LastName"].EqualTo.Val("foo") * f["Contact.FirstName"].EqualTo.Val("boo"));

            Assert.Equal("Appointment", cmd.fromTree.Entity.ID);
            var _and =  Assert.IsType<AndFilter>(cmd.where);
            Assert.Equal(2, _and.filters.Count());

            var _eq1 =  Assert.IsType<EqualToFilter>( _and.filters.First());
            Assert.Equal("Contact.LastName", _eq1.Left.Path);
            Assert.Equal("foo", ((DataFilterValue)_eq1.Right).value);

            var _eq2 = Assert.IsType<EqualToFilter>(_and.filters.Last());
            Assert.Equal("Contact.FirstName", _eq2.Left.Path);
            Assert.Equal("boo", ((DataFilterValue)_eq2.Right).value);
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
                    .Add("AppointmentCf", "AppointmentCustomFields", SchemaTest.AppointmentCustomFields)
                    .Connect("Contact", "Appointment", "Contact", "ContactID", "ID")
                    .Connect("OwnerUser","Contact", "User", "OwnerUserID", "ID")
                    .Connect("AssignedUser", "Appointment", "User", "AssignedUserID", "ID")
                    .Connect("AppointmentCustomFields", "Appointment", "AppointmentCf", "ID", "ID");
            });
            return s;
        }
    }
}
