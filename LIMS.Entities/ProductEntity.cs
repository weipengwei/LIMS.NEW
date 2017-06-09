using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIMS.Entities
{
    public class ProductEntity : BaseEntity
    {
        public string Id
        {
            get; set;
        }

        public string Name
        {
            get; set;
        }

        public string FullName
        {
            get; set;
        }

        public string Description
        {
            get; set;
        }

        public string Brand
        {
            get; set;
        }

        public string ShortCode
        {
            get; set;
        }

        public string MiniPackageUnit
        {
            get; set;
        }

        public string MiniPackageSpec
        {
            get; set;
        }

        public int MiniPackageCount
        {
            get; set;
        }

        public string PackageUnit
        {
            get; set;
        }

        public string Category
        {
            get; set;
        }

        public string Barcode
        {
            get; set;
        }

        public string BarcodeUrl
        {
            get; set;
        }

        public bool IsLocal
        {
            get; set;
        }

        public string RegisterNumber
        {
            get; set;
        }

        public DateTime ValidDate
        {
            get; set;
        }=new DateTime(1900,1,1);

        public override void Init(IDataReader reader)
        {
            base.Init(reader);

            this.Id = reader["id"].ToString();
            this.Name = reader["name"].ToString();
            this.FullName = reader["full_name"].ToString();
            this.Description = reader["description"].ToString();
            this.Brand = reader["brand"].ToString();
            this.ShortCode = reader["short_code"].ToString();
            this.MiniPackageUnit = reader["mini_package_unit"].ToString();
            this.MiniPackageSpec = reader["mini_package_spec"].ToString();
            this.MiniPackageCount = reader.GetInt32(reader.GetOrdinal("mini_package_count"));
            this.PackageUnit = reader["package_unit"].ToString();
            this.Category = reader["category"].ToString();
            this.IsLocal = reader.GetBoolean(reader.GetOrdinal("is_local"));
            this.Barcode = reader["barcode"].ToString();
            this.BarcodeUrl = reader["barcode_url"].ToString();

            this.RegisterNumber = reader["register_number"].ToString();
            this.ValidDate =Convert.ToDateTime(reader["ValidDate"]);
        }
    }
}
