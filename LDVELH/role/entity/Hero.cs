using LDVELH.role.tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using LDVELH.role.entity;

namespace LDVELH {
    public class Hero {
        XmlDocument xmlDoc = new XmlDocument();
        int entitlement;
        int stamina;
        int gold;
        int luck;
        string name;
        int lastChapter;

        public int Gold { get => gold; set => gold = value; }
        public string Name { get => name; set => name = value; }
        public int Stamina { get => stamina; set => stamina = value; }
        public int Entitlement { get => entitlement; set => entitlement = value; }
        public int Luck { get => luck; set => luck = value; }
        public int LastChapter { get => lastChapter; set => lastChapter = value; }

        public Hero(string name) {
            Dice dice = new Dice();
            dice.Roll();
            this.entitlement = dice.Value + 6;
            dice.Roll();
            int firstDIce = dice.Value;
            dice.Roll();
            this.stamina = firstDIce + dice.Value + 12;
            dice.Roll();
           /* this.gold = dice.Value;
            dice.Roll();*/
            this.luck = dice.Value + 6;
            this.name = name;
        }

        public Hero() {

        }

        public void Save() {
            XmlNode heros = xmlDoc.CreateElement("heros");
            xmlDoc.AppendChild(heros);

            XmlNode hero = xmlDoc.CreateElement("hero");

            XmlAttribute attribute = xmlDoc.CreateAttribute("entitlement");
            attribute.Value = this.entitlement.ToString();
            hero.Attributes.Append(attribute);

            XmlAttribute stamina = xmlDoc.CreateAttribute("stamina");
            stamina.Value = this.stamina.ToString();
            hero.Attributes.Append(stamina);

            XmlAttribute luck = xmlDoc.CreateAttribute("luck");
            luck.Value = this.luck.ToString();
            hero.Attributes.Append(luck);

            XmlAttribute gold = xmlDoc.CreateAttribute("gold");
            gold.Value = this.gold.ToString();
            hero.Attributes.Append(gold);

            XmlAttribute lastChapter = xmlDoc.CreateAttribute("lastChapter");
            lastChapter.Value = this.lastChapter.ToString();
            hero.Attributes.Append(lastChapter);

            hero.InnerText = this.name;
            heros.AppendChild(hero);

            xmlDoc.Save(Constantes.HERO_FILE_NAME);
        }

        internal void Update(Chapter chapter) {
            if (chapter.GoldLost > 0) {
                gold -= chapter.GoldLost;
            }
        }

        public static Hero Load() {
            Hero hero = new Hero();
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(Constantes.HERO_FILE_NAME);
            XmlNodeList dataNodes = xmlDoc.SelectNodes("//heros//hero");
            XmlAttributeCollection attributes = dataNodes[0].Attributes;
            int entitlement = DataConverter.ToInt(attributes.GetNamedItem("entitlement").Value);
            hero.Entitlement = entitlement;
            int stamina = DataConverter.ToInt(attributes.GetNamedItem("stamina").Value);
            hero.Stamina = stamina;
            int luck = DataConverter.ToInt(attributes.GetNamedItem("luck").Value);
            hero.Luck = luck;
            int gold = DataConverter.ToInt(attributes.GetNamedItem("gold").Value);
            hero.Gold = gold;
            int lastChapter = DataConverter.ToInt(attributes.GetNamedItem("lastChapter").Value);
            hero.LastChapter = lastChapter;
            return hero;
        }

        public static Boolean Exist() {
            return File.Exists(Constantes.HERO_FILE_NAME);
        }
    }
}
