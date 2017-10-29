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
        List<Enemy> enemies = new List<Enemy>();
        Boolean escapableFightPossibility = false;
        Boolean mandatoryFight = false;
        int index;

        public int GoldLost { get => goldLost; set => goldLost = value; }
        public string Text { get => text; set => text = value; }
        public bool EscapableFightPossibility { get => escapableFightPossibility; set => escapableFightPossibility = value; }
        public bool MandatoryFight { get => mandatoryFight; set => mandatoryFight = value; }
        public int Index { get => index; set => index = value; }
        internal List<Enemy> Enemies { get => enemies; set => enemies = value; }

        public static Chapter Load(int index) {
            Chapter chapter = new Chapter();
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(Constantes.BOOK_FILE_NAME);
            XmlNodeList dataNodes = xmlDoc.SelectNodes("//root//chapter");


            try {
                XmlNode node = dataNodes[i: index - 1];
                chapter.index = index;
                foreach (XmlNode enemy in node.SelectNodes("enemy")) {
                    chapter.Enemies.Add(new Enemy(enemy["name"].InnerText, DataConverter.ToInt(enemy["entitlement"].InnerText), DataConverter.ToInt(enemy["stamina"].InnerText)));
                }
                chapter.Text = node.SelectNodes("text")[0].InnerText; ;
                XmlAttributeCollection attributes = node.Attributes;

                if (attributes.GetNamedItem("goldLost") != null) {
                    int goldLost = DataConverter.ToInt(attributes.GetNamedItem("goldLost").Value);
                    chapter.GoldLost = goldLost;
                }
                if (attributes.GetNamedItem("escapableFightPossibility") != null) {
                    chapter.EscapableFightPossibility = DataConverter.ToBoolean(attributes.GetNamedItem("escapableFightPossibility").Value); ;
                }
                if (attributes.GetNamedItem("mandatoryFight") != null) {
                    chapter.EscapableFightPossibility = DataConverter.ToBoolean(attributes.GetNamedItem("mandatoryFight").Value); ;
                }
            } catch (Exception e) {
                throw e;
            }

            return chapter;
        }

        public Boolean HasEnnemies() {
            return this.enemies.Count() > 0;
        }

        public Enemy NextOpponent() {
            Enemy enemy = null;
            foreach (Enemy e in enemies) {
                if (!e.IsKO()) {
                    enemy = e;
                    break;
                }
            }
            return enemy;
        }
    }
}
