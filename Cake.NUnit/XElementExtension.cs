namespace Cake.SoftNUnit3
{
    using System.Xml.Linq;

    public static class XElementExtension
    {
        public static bool Passed(this XElement element)
        {
            return element.Attribute("result").Value == "Passed";
        }

        public static bool NotPassed(this XElement element) => !Passed(element);
    }
}
