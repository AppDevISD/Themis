using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace WebUI
{
    /// <summary>
    /// Used to Get Data for Charts from C# Code
    /// </summary>
    [System.Web.Script.Services.ScriptService]
    public class ChartHandler : System.Web.Services.WebService
    {
        [WebMethod]
        public string GetRandomCoords(int count, int min, int max)
        {
            Random r = new Random();
            List<float> randomVals = new List<float>();
            List<double> rDouble = new List<double>();
            float rFloat = 0;
            double rDoub = 0;
            for (int i = 0; i < count; i++)
            {
                rDoub = r.NextDouble();
                rFloat = r.Next(min, max);
                rFloat = rFloat + (float)rDoub;
                randomVals.Add(rFloat);
            }
            string randomValsJson = JsonConvert.SerializeObject(randomVals);
            return randomValsJson;
        }
    }
}
