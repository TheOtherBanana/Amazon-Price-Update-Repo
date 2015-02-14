using AWSProjects.CommonUtils;
using PriceUpdateWebAPIBL;
using ProductUpdateCatalogProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace AmazonPriceUpdateWebAPI.Controllers
{
    public class ProductAPIController : ApiController
    {
        WebAPIManager apiManager;
        public ProductAPIController(WebAPIManager _apiManager)
        {
            this.apiManager = _apiManager;
        }


        // GET Api to get Heartbeat of service
        public HttpResponseMessage Get(bool heartBeat)
        {
            return Request.CreateResponse(HttpStatusCode.OK,heartBeat);
        }

        //POST to get args and return the product details
        public HttpResponseMessage Post([FromBody]GetProductAPIArgs args)
        {
            HttpResponseMessage httpResponse;
            try
            {
                if (args == null)
                {
                    httpResponse = Request.CreateResponse(HttpStatusCode.BadRequest,"Input arguments are null. Please check your request");
                }
                else
                {
                    HttpRequest httpRequest = HttpContext.Current.Request;
                    var response = this.apiManager.GetProductDetails(args, Request);
                    httpResponse = Request.CreateResponse(HttpStatusCode.OK, response);
                }
            }
            catch
            {
                httpResponse = Request.CreateResponse(HttpStatusCode.InternalServerError, "Something went wrong. Please check your inputs and retry after sometime");
            }

            return httpResponse;

        }

        //PUT Will update the product with the rule given
        public HttpResponseMessage Put([FromBody]UpdateProductAPIArgs args)
        {
            HttpResponseMessage httpResponse;
            try
            {
                if (args == null)
                {
                    httpResponse = Request.CreateResponse(HttpStatusCode.BadRequest, "Input arguments are null. Please check your request");
                }
                else
                {
                    HttpRequest httpRequest = HttpContext.Current.Request;
                    this.apiManager.UpdateProductDetails(args, Request);
                    httpResponse = Request.CreateResponse(HttpStatusCode.OK, "Updated successfully");
                }
            }
            catch
            {
                httpResponse = Request.CreateResponse(HttpStatusCode.InternalServerError, "Something went wrong. Please check your inputs and retry after sometime");
            }

            return httpResponse;
        }

        // PUT will confirm, Confirmation through email
        public HttpResponseMessage Put([FromBody]ProductIdArgs args, bool confirmed)
        {
            HttpResponseMessage httpResponse;
            if (args == null)
            {
                httpResponse = Request.CreateResponse(HttpStatusCode.BadRequest, "Input arguments are null. Please check your request");
            }
            else
            {
                HttpRequest httpRequest = HttpContext.Current.Request;
                httpResponse = this.apiManager.ConfirmProductDetails(args, confirmed, Request);
            }

            return httpResponse;
        }

        //DELETE Delete's the product rule and marks the product as deleted
        public HttpResponseMessage Delete([FromBody]ProductIdArgs args)
        {
            return null;
        }

    }
}
