using LDVELH.role.tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace LDVELH.role.entity {
    class Chapter {
        int goldLost;
        string text;

        public int GoldLost { get => goldLost; set => goldLost = value; }
        public string Text { get => text; set => text = value; }

        public static Chapter Load(int index) {
            Chapter chapter = new Chapter();
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(Constantes.BOOK_FILE_NAME);
            XmlNodeList dataNodes = xmlDoc.SelectNodes("//root//chapter");


            try {
                chapter.Text = dataNodes[i: index - 1].InnerText; ;
                XmlAttributeCollection attributes = dataNodes[i: index - 1].Attributes;
                if (attributes.GetNamedItem("goldLost") != null) {
                    int goldLost = DataConverter.ToInt(attributes.GetNamedItem("goldLost").Value);
                    chapter.GoldLost = goldLost;
                }
            } catch (Exception e) {
                throw e;
            }

            return chapter;
        }
    }
}
