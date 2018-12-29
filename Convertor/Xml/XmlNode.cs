using System;

namespace Convertor.Xml
{
    public abstract class XmlNode : Stringifiable
    {
        public override string ToString()
        {
            return "Xml(" + this.Stringify() + ")";
        }
    }
}
