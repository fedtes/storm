using Storm.Schema;
using System;
using System.Collections.Generic;
using System.Text;

namespace Storm.Test
{
    public abstract class BaseObj
    {
        [StormPrimaryKey]
        [StormColumnAccess(ColumnAccess.ReadOnly)]
        public int ID { get; set; }
    }

    public class Appointment : BaseObj
    {
        public string Summary { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public string Subject { get; set; }
        [StormColumnType(System.Data.DbType.String, 255)]
        public string Description { get; set; }
        public bool Private { get; set; }
        [StormColumnName("UserID")]
        public int AssignedUserID { get; set; }
        [StormDefaultIfNull(-1)]
        public int ContactID { get; set; }
        public Object AppointmentInfo { get; set; }
        [StormIgnore]
        public int IgnoreThisProp { get; set; }
    }


    public class Contact : BaseObj
    {
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string ZipCode { get; set; }
        public string City { get; set; }
        public string Phone { get; set; }
        public string Phone2 { get; set; }
        public string FAX { get; set; }
        public string Mobile { get; set; }
        public string EMail { get; set; }
        public string FiscalCode { get; set; }
        public string ContactCode { get; set; }
        public bool Marketing { get; set; }
        public string Gender { get; set; }
        public DateTime BirthDate { get; set; }
        public int OwnerUserID { get; set; }
        public int OrganizationID { get; set; }
        public int LanguageID { get; set; }
    }

    public class User : BaseObj
    {
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public int LanguageID { get; set; }
    }

    public class Organization : BaseObj
    {
        public string AccountCode { get; set; }
        public string CompanyName { get; set; }
        public string Address1 { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }
        public string Phone { get; set; }
        public string Mobile { get; set; }
        public string EMail { get; set; }
        public string FiscalCode { get; set; }
        public string VATCode { get; set; }
    }


    public class Language : BaseObj
    {
        public string Code { get; set; }
        public string Description { get; set; }
    }


}
