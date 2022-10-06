using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebDemo.Api.Options
{
    public class SecondPositionOption
    {
        public const string Month = "Month";
        public const string Year = "Year";
/*
       "Month": {
      "Name": "Green Widget",
      "Model": "GW46"
     }                         */
        public string Name { get; set; }  = String.Empty;
        public string Model { get; set; } = String.Empty;
    }
}
