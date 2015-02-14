using AmazonProductAPIWrapper;
using AWSProjects.CommonUtils;
using ProductUpdateCatalogProvider;
using ProductUpdateCatalogProvider.CatalogEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace PriceUpdateWebAPIBL
{
    public class WebAPIManager
    {
        public GetProductAPIContract GetProductDetails(GetProductAPIArgs args, HttpRequestMessage request)
        {

            DynamoDBTracer.Tracer.Write
            this.CheckAndUpdateDoSLimits(request,"GetProductCall");
            ItemLookupResponse itemResponse = AmazonProductHelper.GetItemDetails(args.ProductASIN, args.ProductRegion.ToString());
            
            var item = itemResponse.Items.FirstOrDefault().Item.FirstOrDefault();

            GetProductAPIContract apiContract = new GetProductAPIContract
            {
                ProductASIN = item.ASIN,
                ProductName = item.ItemAttributes.Title,
                ProductRegion = args.ProductRegion
            };

            return apiContract;
        }

        public void UpdateProductDetails(UpdateProductAPIArgs args, HttpRequestMessage request)
        {
            this.CheckAndUpdateDoSLimits(request,args.EmailId);
            ProductCatalogEntity catalogEntity = ValidateAndGetEntityFromArgs(args);
            var offers = AmazonProductHelper.GetOffers(args.ProductASIN, args.ProductRegion.ToString());
            var offersSummary = offers.Items.FirstOrDefault().Item.FirstOrDefault().OfferSummary;
            string price= offersSummary.LowestNewPrice.Amount;
            catalogEntity.InitialPrice = (double.Parse(price) / 100).ToString();
            catalogEntity.CurrencyCode = offersSummary.LowestNewPrice.CurrencyCode;
            
            

            var catalogProvider = PriceUpdateContext.Instance.CatalogFactory.GetProductCatalogProvider();
            catalogProvider.UpdateProduct(catalogEntity);
            //Trace success
        }

        public HttpResponseMessage ConfirmProductDetails(ProductIdArgs args, bool confirm, HttpRequestMessage request)
        {
            this.CheckAndUpdateDoSLimits(request,args.EmailId);
            HttpResponseMessage response;
            try
            {
                var catalogProvider = PriceUpdateContext.Instance.CatalogFactory.GetProductCatalogProvider();
                ProductCatalogEntity catalogEntity = catalogProvider.GetProduct(args);
                if (string.IsNullOrEmpty(catalogEntity.EmailId) || string.IsNullOrEmpty(catalogEntity.ASIN))
                {
                    response = new HttpResponseMessage(HttpStatusCode.NotFound);
                    return response;
                }
                catalogEntity.IsConfirmed = confirm;
                catalogProvider.UpdateProduct(catalogEntity);
                response = new HttpResponseMessage(HttpStatusCode.OK);
            }
            catch
            {
                response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }

            return response;
        }
        private static ProductCatalogEntity ValidateAndGetEntityFromArgs(UpdateProductAPIArgs args)
        {
            ProductCatalogEntity catalogEntity = new ProductCatalogEntity
            {
                ASIN = args.ProductASIN,
                Country = args.ProductRegion.ToString(),
                EmailId = args.EmailId,
                ToEmailEveryWeek = args.ToEmailEveryWeek,
                ToEmailOnDate = args.ToEmailOnDate,
                ToEmailOnPrice = args.ToEmailOnPrice,
                IsConfirmed = false,
                IsDeleted = false,
                ProductPriceType = args.ProductPriceType,
                DateLastUpdated = DateTime.UtcNow.Date
            };

            if (catalogEntity.ToEmailEveryWeek)
            {
                if (Enum.IsDefined(typeof(DayOfWeek), args.DayToEmail) && args.EmailEveryWeekDuration != null)
                {
                    catalogEntity.DayToEmail = args.DayToEmail;
                    catalogEntity.EmailEveryWeekDuration = args.EmailEveryWeekDuration;
                }
                else
                {
                    throw new ArgumentException("Day of week is not a valid day or the duration is invalid");
                }
            }

            if (catalogEntity.ToEmailOnDate)
            {
                if (args.DateToEmail != null)
                {
                    catalogEntity.DateToEmail = args.DateToEmail;
                }
                else
                {
                    throw new ArgumentException("The date to email is invalid");
                }
            }

            if (catalogEntity.ToEmailOnPrice)
            {
                double value;
                if (double.TryParse(args.PriceToEmail, out value) && args.EmailOnPriceDuration != null)
                {
                    catalogEntity.PriceToEmail = args.PriceToEmail;
                    catalogEntity.EmailOnPriceDuration = args.EmailOnPriceDuration;
                }
                else
                {
                    throw new ArgumentException("Price is not a valid double or the duration is invalid");
                }
            }
            return catalogEntity;
        }

        private void CheckAndUpdateDoSLimits(HttpRequestMessage request, string emailId)
        {
            var dosLimitCatalogProvider = PriceUpdateContext.Instance.CatalogFactory.GetDoSLimitCatalogProvider();
            string userIP = GetClientIp(request);
            
            DoSLimitArgs args = new DoSLimitArgs
            {
                UserIP = userIP,
                EmailId = emailId
            };
            
            DoSLimitEntity dosEntity = dosLimitCatalogProvider.GetLimit(args);
            if(dosEntity == null)
            {
                dosEntity = new DoSLimitEntity
                {
                    UserIPAddress = args.UserIP,
                    EmailId = args.EmailId
                };
            }

            //TODO: Should come from config
            if (dosEntity.Count <= 500)
            {
                dosEntity.Count += 1;
                dosLimitCatalogProvider.UpdateLimit(dosEntity);
            }
            else
            {
                throw new Exception("Request limits reached for this IP");
            }

        }

        private string GetClientIp(HttpRequestMessage request)
        {
            if (request.Properties.ContainsKey("MS_HttpContext"))
            {
                return ((HttpContextWrapper)request.Properties["MS_HttpContext"]).Request.UserHostAddress;
            }
            else if (request.Properties.ContainsKey(RemoteEndpointMessageProperty.Name))
            {
                RemoteEndpointMessageProperty prop = (RemoteEndpointMessageProperty)request.Properties[RemoteEndpointMessageProperty.Name];
                return prop.Address;
            }
            else if (HttpContext.Current != null)
            {
                return HttpContext.Current.Request.UserHostAddress;
            }
            else
            {
                return null;
            }
        }
    }
}
