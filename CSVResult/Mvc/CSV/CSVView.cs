using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Linq;

namespace CSVResult.Mvc.CSV
{
    public class CSVView : IView, IViewEngine
    {
        private readonly ViewEngineResult _result = null;
        private readonly string _filename = null;

        public CSVView(ViewEngineResult result, string fileName)
        {
            if (result == null) throw new ArgumentNullException("result");
            _result = result;
            this._filename = fileName;
        }

        public void Render(ViewContext viewContext, System.IO.TextWriter writer)
        {
            // generate view into string
            var sb = new System.Text.StringBuilder();
            TextWriter tw = new System.IO.StringWriter(sb);
            _result.View.Render(viewContext, tw);
            var resultCache = sb.ToString();

            XElement root = XElement.Load(GetXmlReader(resultCache));

            List<string> csv_rows = new List<string>();

            IEnumerable<XElement> headers =
                from trs in root.Descendants("thead")
                from th in trs.Descendants("th")
                select th;

            string csv_headers = string.Join(";", headers.Select(h => h.Value));
            csv_rows.Add(csv_headers);

            IEnumerable<XElement> data =
                from trs in root.Descendants("tbody")
                select trs;

            foreach (var tr in data.Descendants("tr"))
            {
                IEnumerable<XElement> tds =
                    from td in tr.Descendants("td")
                    select td;

                string csv_data_row = string.Join(";", tds.Select(d => d.Value));
                csv_rows.Add(csv_data_row);
            }

            string csv = string.Join(System.Environment.NewLine, csv_rows);

            viewContext.Writer.Write(csv);
            viewContext.HttpContext.Response.ContentType = "text/csv; charset=utf-8";
            viewContext.HttpContext.Response.AddHeader(
                "Content-disposition",
                string.Format("attachment;filename={0}", this._filename));
        }

        public ViewEngineResult FindPartialView(ControllerContext controllerContext, string partialViewName, bool useCache)
        {
            throw new NotImplementedException();
        }

        public ViewEngineResult FindView(ControllerContext controllerContext, string viewName, string masterName, bool useCache)
        {
            throw new NotImplementedException();
        }

        public void ReleaseView(ControllerContext controllerContext, IView view)
        {
            _result.ViewEngine.ReleaseView(controllerContext, _result.View);
        }

        private static XmlTextReader GetXmlReader(string source)
        {
            byte[] byteArray = Encoding.UTF8.GetBytes(source);
            MemoryStream stream = new MemoryStream(byteArray);

            var xtr = new XmlTextReader(stream);
            xtr.WhitespaceHandling = WhitespaceHandling.None;
            return xtr;
        }
    }
}
