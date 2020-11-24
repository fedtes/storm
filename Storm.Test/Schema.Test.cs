using System;
using System.Data;
using System.Linq;
using Storm.Schema;
using Xunit;

namespace Storm.Test
{
    public class SchemaTest
    {

        public static EntityBuilder AppointmentCustomFields(EntityBuilder e)
        {
            return e.Add(new FieldConfig() { CodeName = "ID", CodeType = typeof(String), IsPrimary=true, DBName = "AppointmentID", DBType = DbType.Int32 })
                .Add(new FieldConfig() { CodeName = "CF1", CodeType = typeof(String), DBName = "CF1_COL", DBType = DbType.String })
                .Add(new FieldConfig() { CodeName = "CF2", CodeType = typeof(String), DBName = "CF2_COL", DBType = DbType.String })
                .Add(new FieldConfig() { CodeName = "CF3", CodeType = typeof(int), DBName = "CF3_COL", DBType = DbType.Int32 })
                .Add(new FieldConfig() { CodeName = "CF4", CodeType = typeof(DateTime), DBName = "CF4_COL", DBType = DbType.DateTime});

        }

        [Fact]
        public void CreateSchemaWithEditor()
        {
            var s = new Storm();
            s.EditSchema(e => {
                return e.Add<Appointment>("Appointment", "Appointments")
                    .Add<Contact>("Contact", "Contacts")
                    .Add<User>("User", "Users")
                    .Add<Organization>("Organization", "Organizations")
                    .Add<Language>("Language", "Languages")
                    .Add("AppointmentCf", "AppointmentCustomFields", AppointmentCustomFields);

            });

            var n = s.schema.GetNavigator();

            //-------------------- APPOINTMENT ---------------------
            var appointment = n.GetEntity("Appointment");
            Assert.Equal("Appointments", appointment.DBName);
            Assert.Equal(typeof(Appointment), appointment.TModel);
            Assert.Equal("Appointment", appointment.ID);
            Assert.Equal(9, appointment.entityFields.Count());


            //------------------- APPOINTMENT CF -------------------
            var appointmentCf = n.GetEntity("AppointmentCf");
            Assert.Equal("AppointmentCustomFields", appointmentCf.DBName);
            Assert.Null(appointmentCf.TModel);
            Assert.Equal("AppointmentCf", appointmentCf.ID);
            Assert.Equal(5, appointmentCf.entityFields.Count());

        }
    }
}
