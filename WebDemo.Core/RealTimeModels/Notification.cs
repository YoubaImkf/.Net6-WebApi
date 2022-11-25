using System;

namespace WebDemo.Core.RealTimeModels
{
    //Propriété mutable source:https://www.infoq.com/fr/articles/records-c9-tugce-ozdeger/#:~:text=par%20cr%C3%A9ation%20nominale.-,Propri%C3%A9t%C3%A9%20mutable,-Les%20records%20sont
      public record Notification(string text, DateTime date);
    
}
