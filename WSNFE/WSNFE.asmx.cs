using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Script.Services;
using System.Text;
using System.ServiceModel.Web;


namespace WSNFE
{
    /// <summary>
    /// Summary description for WSNFE
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/", Name = "WSNFE")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class WSNFE : System.Web.Services.WebService
    {

       [WebMethod]
       [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
       [WebInvokeAttribute(BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json)]

       public string buscarNota(int numPedido)
        {
            this.Context.Response.ContentType = "application/json; charset=utf-8";
            StringBuilder sb = new StringBuilder();
            var nota = new Notas();
               
            sb.Append(nota.buscarNota(numPedido));
            sb.Replace("<?xml version=\"1.0\" encoding=\"utf-8\"?><string xmlns=\"http://tempuri.org/\">", "");
            return sb.ToString();
        }
       
    }
}
