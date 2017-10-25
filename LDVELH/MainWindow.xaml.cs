using LDVELH.role.entity;
using LDVELH.role.tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LDVELH {
    public partial class MainWindow : Window {
        int lastStableChapterIndex = 1;
        Hero hero;
        int phase = Constantes.START_HERO_CREATION_PHASE;
        int diceOneValue = 0;
        int diceTwoValue = 0;

        public int LastStableChapterIndex { get => lastStableChapterIndex; set => lastStableChapterIndex = value; }


        public MainWindow() {
            InitializeComponent();
            if (Hero.Exist()) {
                hero = Hero.Load();
                DisplayHerosCaracteritics();
                DisplayChapter(hero.LastChapter);
                phase = Constantes.EXPLORATION_PHASE;
            } else {
                DisplayHerosCreation();
                hero = new Hero();
            }
        }

        private void DisplayHerosCreation() {
            switch (phase) {
                case Constantes.START_HERO_CREATION_PHASE:
                    textDisplayer.AppendText("Veuillez lancer un dé pour determiner votre habilité.");
                    textDisplayer.AppendText("\n");
                    phase = Constantes.ENTITLEMENT_PHASE;
                    break;
                case Constantes.ENTITLEMENT_PHASE:
                    int Entitlement = ((diceOneValue > 0) ? diceOneValue : diceTwoValue) + 6;
                    hero.Entitlement = Entitlement;
                    textDisplayer.AppendText("Votre habilité sera de  " + Entitlement);
                    textDisplayer.AppendText("\n");
                    textDisplayer.AppendText("Veuillez lancer les dés pour determiner votre endurance.");
                    textDisplayer.AppendText("\n");
                    phase = Constantes.STAMINA_PHASE;
                    break;
                case Constantes.STAMINA_PHASE:
                    int Stamina = diceOneValue + diceTwoValue + 12;
                    hero.Stamina = Stamina;
                    textDisplayer.AppendText("Votre endurance sera de  " + Stamina);
                    textDisplayer.AppendText("\n");
                    textDisplayer.AppendText("Veuillez lancer les dés pour determiner votre chance.");
                    textDisplayer.AppendText("\n");
                    phase = Constantes.LUCK_PHASE;
                    break;
                case Constantes.LUCK_PHASE:
                    int Luck = ((diceOneValue > 0) ? diceOneValue : diceTwoValue) + 6;
                    hero.Luck = Luck;
                    textDisplayer.AppendText("Votre chance sera de  " + Luck);
                    textDisplayer.AppendText("\n");
                    textDisplayer.AppendText("Cliquez sur Go pour commencer l'aventure");
                    phase = Constantes.EXPLORATION_PHASE;
                    DisplayChapter(1);
                    break;
                default:
                    break;
            }
            diceOneValue = 0;
            diceTwoValue = 0;
        }

        private void DisplayHerosCaracteritics() {
            nextChapter.Text = hero.LastChapter.ToString();
            entitlement.Text = hero.Entitlement.ToString();
            stamina.Text = hero.Stamina.ToString();
            luck.Text = hero.Luck.ToString();
            gold.Text = hero.Gold.ToString();

        }

        private int GetLastStableChapterIndex() {
            return LastStableChapterIndex;
        }

        private void DisplayChapter(int index) {
            try {
                Chapter chapter = Chapter.Load(index);
                hero.Update(chapter);
                textDisplayer.AppendText(chapter.Text);
                LastStableChapterIndex = index;
                DisplayHerosCaracteritics();
            } catch (Exception e) {
                DisplayChapter(LastStableChapterIndex);
            }
        }

        private void ChapterChangedEventHandler(object sender, EventArgs e) {
            textDisplayer.Document.Blocks.Clear();
            int index = LastStableChapterIndex;
            if (nextChapter != null) {
                string value = nextChapter.Text;
                index = DataConverter.ToInt(value);
            }
            DisplayChapter(index);
            nextChapter.Text = LastStableChapterIndex.ToString();
        }

        private void SaveAction(object sender, RoutedEventArgs e) {
            if (phase == Constantes.EXPLORATION_PHASE) {
                hero.LastChapter = lastStableChapterIndex;
                hero.Save();
                System.Windows.MessageBox.Show("Progression Sauvegardée");
            }
        }

        private void RollDiceOneAction(object sender, RoutedEventArgs e) {
            if (IsHeroCreationPhase()) {
                Random num = new Random();
                diceOneValue = num.Next(1, 7);
                dice1.Source = new BitmapImage(new Uri(@"DiceFaces\" + diceOneValue.ToString() + ".png", UriKind.Relative));
                GotoNextPhase();
            }
        }


        private void RollDiceTwoAction(object sender, RoutedEventArgs e) {
            if (IsHeroCreationPhase()) {
                Random num = new Random();
                diceTwoValue = num.Next(1, 7);
                dice2.Source = new BitmapImage(new Uri(@"DiceFaces\" + diceTwoValue.ToString() + ".png", UriKind.Relative));
                GotoNextPhase();
            }
        }

        private void GotoNextPhase() {
            switch (phase) {
                case 1:
                    if (diceOneValue > 0 || diceTwoValue > 0) {
                        DisplayHerosCreation();
                    }
                    break;
                case 2:
                    if (diceOneValue > 0 && diceTwoValue > 0) {
                        DisplayHerosCreation();
                    }
                    break;
                case 3:
                    if (diceOneValue > 0 || diceTwoValue > 0) {
                        DisplayHerosCreation();
                    }
                    break;
                default:
                    break;
            }
        }


        private bool IsHeroCreationPhase() {
            return (phase == Constantes.ENTITLEMENT_PHASE || phase == Constantes.STAMINA_PHASE || phase == Constantes.LUCK_PHASE);
        }
    }
}
