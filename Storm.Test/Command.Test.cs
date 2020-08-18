using Storm.Execution;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Storm.Test
{
    public class CommandTest
    {
        [Fact]
        public void CreateGetCommand()
        {
            var s = new Storm();
            s.EditSchema(e => {
                return e.Add<Appointment>("Appointment", "Appointments")
                    .Add<Contact>("Contact", "Contacts")
                    .Add<User>("User", "Users")
                    .Add<Organization>("Organization", "Organizations")
                    .Add<Language>("Language", "Languages")
                    .Add("AppointmentCf", "AppointmentCustomFields", SchemaTest.AppointmentCustomFields)
                    .Connect("Contact", "Appointment", "Contact", "ContactID", "ID")
                    .Connect("AssignedUser", "Appointment", "User", "AssignedUserID", "ID");
            });

            GetCommand cmd = new GetCommand(s.schema.GetNavigator(), "Appointment");
            cmd.With("Contact").With("AssignedUser");

            Assert.Equal("Appointment", cmd.fromTree.Entity.ID);
            Assert.Equal(2, cmd.fromTree.children.Count);
            Assert.Equal("Contact", cmd.fromTree.children[0].Entity.ID);
            Assert.Equal("User", cmd.fromTree.children[1].Entity.ID);
        }

    }
}
