using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace CSVResult.Mvc.CSV
{
    public class CSVResult : ViewResult
    {
        internal string FileName { get; private set; }

        public CSVResult(object model, string name)
            : this(model, name, "file.csv")
        {
        }

        public CSVResult()
            : this(new ViewDataDictionary(), "Csv")
        {
        }

        public CSVResult(object model)
            : this(model, "Csv")
        {
        }

        public CSVResult(object model, string name, string fileName)
        {
            ViewData = new ViewDataDictionary(model);
            ViewName = name;
            FileName = fileName;
        }

        protected override ViewEngineResult FindView(ControllerContext context)
        {
            var result = base.FindView(context);
            //return result;

            CSVView view = new CSVView(result, FileName);
            return new ViewEngineResult(view, view);
        }
    }
}
