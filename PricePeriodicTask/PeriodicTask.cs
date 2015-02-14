using AmazonProductAPIWrapper;
using EmailUtils;
using PriceUpdateWebAPIBL;
using ProductUpdateCatalogProvider.CatalogEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PricePeriodicTask
{
    public class PeriodicTask
    {
        public void CheckPriceAndSendMail()
        {
            try
            {
                var catalogProvider = PriceUpdateContext.Instance.CatalogFactory.GetProductCatalogProvider();
                var allProducts = catalogProvider.GetProducts();
                Dictionary<string, Dictionary<string, string>> emailsToSend = new Dictionary<string, Dictionary<string, string>>();
                DateTime todayDate = DateTime.UtcNow.Date;
                DayOfWeek day = todayDate.DayOfWeek;
                List<ProductCatalogEntity> entitiesToUpdate = new List<ProductCatalogEntity>();
                foreach (ProductCatalogEntity product in allProducts)
                {
                    if (product.ToEmailOnPrice && todayDate <= product.EmailOnPriceDuration)
                    {
                        //Check if last emailed on is not null
                        //GetPrice
                        //If price is lower than specifie add to email. Mark Email sent on
                        //Check if duration is passed and delete

                        var offers = AmazonProductHelper.GetOffers(product.ASIN, product.Country);
                        double itemPrice = double.Parse(offers.Items.FirstOrDefault().Item.FirstOrDefault().OfferSummary.LowestNewPrice.Amount);
                        itemPrice = itemPrice / 100;
                        double priceToEmail = double.Parse(product.PriceToEmail);

                        if (itemPrice <= priceToEmail)
                        {
                            AddItemToEmail(emailsToSend, product);
                            product.EmailSentOn = todayDate;
                            product.ToEmailOnPrice = false;
                            entitiesToUpdate.Add(product);
                        }
                        
                    }

                    if (product.ToEmailEveryWeek && todayDate <= product.EmailEveryWeekDuration)
                    {
                        DayOfWeek dayToEmail = (DayOfWeek)Enum.Parse(typeof(DayOfWeek), product.DayToEmail);
                        if (dayToEmail == day)
                        {
                            AddItemToEmail(emailsToSend, product);
                        }
                    }

                    if (product.ToEmailOnDate && todayDate == product.DateToEmail)
                    {
                        AddItemToEmail(emailsToSend, product);
                    }

                    //If the duration has passed, delete it
                    if (todayDate > product.EmailEveryWeekDuration && product.EmailEveryWeekDuration != DateTime.MinValue)
                    {
                        product.ToEmailEveryWeek = false;
                        entitiesToUpdate.Add(product);
                    }
                    if(todayDate > product.EmailOnPriceDuration && product.EmailOnPriceDuration != DateTime.MinValue)
                    {
                        product.ToEmailOnPrice = false;
                        entitiesToUpdate.Add(product);
                    }
                }

                //Send all emails
                foreach (var emailId in emailsToSend.Keys)
                {
                    //Figure out a way to fix this
                    string mailToSend = ProductEmailHelper.ProductHtmlMailContent(emailsToSend[emailId], AmazonProductAPIContext.Regions.IN);
                    EmailManagerContext.Instance.SendHtmlEmail("ragharamc@gmail.com", "Price updates for your products!", mailToSend);
                }
                
                //After all mails are sent
                entitiesToUpdate.ForEach(entity => catalogProvider.UpdateProduct(entity));
            }
            catch(Exception ex)
            {

            }
        }

        private static void AddItemToEmail(Dictionary<string, Dictionary<string, string>> emailsToSend, ProductCatalogEntity product)
        {
            var itemDetail = AmazonProductHelper.GetItemDetails(product.ASIN, product.Country);
            string itemName = itemDetail.Items.FirstOrDefault().Item.FirstOrDefault().ItemAttributes.Title;
            var cartDetail = AmazonProductHelper.GetCartDetails(product.ASIN, product.Country);
            string cartUrl = cartDetail.Cart.FirstOrDefault().PurchaseURL;
            if (!emailsToSend.ContainsKey(product.EmailId))
            {
                emailsToSend.Add(product.EmailId, new Dictionary<string, string> { { itemName, cartUrl } });
            }
            else
            {
                if (!emailsToSend[product.EmailId].ContainsKey(itemName))
                {
                    emailsToSend[product.EmailId].Add(itemName, cartUrl);
                }
            }
        }
    }
}
