using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LIMS.Util;
using LIMS.Models;
using LIMS.Entities;
using LIMS.Repositories;

namespace LIMS.Services
{
    public class ProductService
    {
        private static object ms_lock = new object();
        private static IDictionary<string, ProductEntity> ms_Products = new Dictionary<string, ProductEntity>();

        public IList<ProductEntity> Query()
        {
            return ProductRepository.Query();
        }

        public IList<ProductEntity> Query(string condition, PagerInfo pager)
        {
            return ProductRepository.Query(condition, pager);
        }

        public ProductEntity Get(string id)
        {
            ProductEntity product;
            if(ms_Products.TryGetValue(id, out product))
            {
                return product;
            }

            lock(ms_lock)
            {
                if (ms_Products.TryGetValue(id, out product))
                {
                    return product;
                }

                product = ProductRepository.Get(id);
                if (product == null)
                {
                    return null;
                }

                //SetBarcode(product);

                ms_Products[id] = product;

                return product;
            }
        }

        public IList<ProductEntity> Get(IList<string> ids)
        {
            return ProductRepository.Get(ids);
        }

        public void Save(ProductEntity product)
        {
            if (string.IsNullOrEmpty(product.Id))
            {
                product.Id = Guid.NewGuid().ToString();

                //SetBarcode(product, false);

                ProductRepository.Add(product);

                ms_Products[product.Id] = product;
            }
            else
            {
                ProductRepository.Update(product);

                if (ms_Products.ContainsKey(product.Id))
                {
                    lock (ms_lock)
                    {
                        if (ms_Products.ContainsKey(product.Id))
                        {
                            ms_Products[product.Id] = product;
                        }
                    }
                }
            }
        }

        public IList<ProductEntity> GetByVendor(string vendorId)
        {
            return ProductRepository.GetByVendor(vendorId);
        }
        
        public IList<ProductEntity> GetByHospital(string hospitalId)
        {
            var products = new List<ProductEntity>();

            var filter = new Dictionary<string, string>();
            var list = HospitalProductRepository.GetByHospital(hospitalId);
            foreach (var item in list)
            {
                if (!filter.Keys.Contains(item.ProductId))
                {
                    var product = this.Get(item.ProductId);
                    if (product != null)
                    {
                        products.Add(product);
                    }
                    filter.Add(item.ProductId,"Y");
                }
            }

            return products;
        }

        public ProductEntity GetByBarcode(string barcode)
        {
            return ProductRepository.GetByBarcode(barcode);
        }

        private void SetBarcode(ProductEntity entity, bool needUpdate = true)
        {
            if(string.IsNullOrEmpty(entity.Barcode) || string.IsNullOrEmpty(entity.BarcodeUrl))
            {
                var key = IdentityCreatorService.New(IdentityKey.PRODUCT_BARCODE);
                var barcode = key.ToString().PadLeft(13, '0');
                var barcodeUrl = BarcodeHelper.CreateImg(barcode);

                entity.Barcode = barcode;
                entity.BarcodeUrl = barcodeUrl;

                if (needUpdate)
                {
                    ProductRepository.UpdateBarcode(entity.Id, barcode, barcodeUrl);
                }
            }
        }
    }
}
