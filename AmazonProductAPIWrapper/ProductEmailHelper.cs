using AmazonProductAPIWrapper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;

namespace AmazonProductAPIWrapper
{
    public class ProductEmailHelper
    {
        const string amazonURLWithTagFormat = "{0}/exec/obidos/redirect-home?tag={1}&placement=home_multi.gif&site=amazon";
        const string imageURL = "http://media.corporate-ir.net/media_files/IROL/97/97664/images/amazon_logo_RGB.jpg";
        public static string GetAmazonLogoAndLink(AmazonProductAPIContext.Regions region)
        {
            StringWriter stringWriter = new StringWriter();
            using (HtmlTextWriter writer = new HtmlTextWriter(stringWriter))
            {
                string amazonEndpoint = AmazonProductAPIConstants.RegionPublicURLMapping[region];
                string associateTag = AmazonProductAPIContext.RegionAssociateIdMapping[region];
                string amazonURL = string.Format(amazonURLWithTagFormat, amazonEndpoint, associateTag);

                writer.AddAttribute(HtmlTextWriterAttribute.Href, amazonURL);
                writer.RenderBeginTag(HtmlTextWriterTag.A);//Open A tag

                writer.AddAttribute(HtmlTextWriterAttribute.Src, imageURL);
                writer.AddAttribute(HtmlTextWriterAttribute.Width, "200");
                writer.AddAttribute(HtmlTextWriterAttribute.Height, "200");
                writer.RenderBeginTag(HtmlTextWriterTag.Img); //Img open
                writer.RenderEndTag();//Img Close
                writer.RenderEndTag();//A Close

            }
            return stringWriter.ToString();
        }

        private static string GetLinkForProduct(string product, string link)
        {
            StringWriter stringWriter = new StringWriter();
            using (HtmlTextWriter writer = new HtmlTextWriter(stringWriter))
            {
                writer.RenderBeginTag(HtmlTextWriterTag.Li);//Open Li tag

                writer.AddAttribute(HtmlTextWriterAttribute.Href, link);
                writer.RenderBeginTag(HtmlTextWriterTag.A);//Open A tag
                writer.Write(product);
                
                writer.RenderEndTag();//Close A tag
                writer.RenderEndTag();//Close LI tag

            }

            return stringWriter.ToString();
        }
        public static string ProductHtmlMailContent(Dictionary<string, string> productsToMail, AmazonProductAPIContext.Regions region)
        {
            StringWriter stringWriter = new StringWriter();
            using (HtmlTextWriter writer = new HtmlTextWriter(stringWriter))
            {
                writer.RenderBeginTag(HtmlTextWriterTag.Html);//Html open
                writer.RenderBeginTag(HtmlTextWriterTag.Body);//Body open

                writer.Write(GetAmazonLogoAndLink(region));//Insert logo and link
                
                writer.RenderBeginTag(HtmlTextWriterTag.Ul);//Unordered list open

                foreach(var key in productsToMail.Keys)
                {
                    string product = key;
                    string link = productsToMail[key];
                    var tempWriter = GetLinkForProduct(product, link);
                    writer.Write(tempWriter);
                }
                writer.RenderEndTag();//Unorder List close
                writer.RenderEndTag();//Body Close
                writer.RenderEndTag();//Html Close
            }

            return stringWriter.ToString();
        }
    }
}
