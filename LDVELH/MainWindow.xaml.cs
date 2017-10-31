using LDVELH.role.entity;
using LDVELH.role.tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
        Hero hero;
        int phase = Constantes.START_HERO_CREATION_PHASE;
        int LastStableChapterIndex = 1;
        Chapter chapter;
        Dice dice1 = new Dice();
        Dice dice2 = new Dice();

        public Hero Hero { get => hero; set => hero = value; }
        internal Chapter Chapter { get => chapter; set => chapter = value; }

        public MainWindow() {
            InitializeComponent();
            if (Hero.Exist()) {
                Hero = Hero.Load();
                DisplayHerosCaracteritics();
                DisplayChapter(Hero.LastChapter);
                phase = Constantes.EXPLORATION_PHASE;
            } else {
                DisplayHerosCreation();
                Hero = new Hero();
            }
        }

        private bool IsHeroCreationPhase() {
            return (phase == Constantes.ENTITLEMENT_PHASE || phase == Constantes.STAMINA_PHASE || phase == Constantes.LUCK_PHASE);
        }

        private bool IsFightPhase() {
            return (phase == Constantes.FIGHT_PHASE);
        }

        /***** Display ******/
        public void DisplayHerosCaracteritics() {
            nextChapter.Text = Hero.LastChapter.ToString();
            entitlement.Text = Hero.Entitlement.ToString();
            stamina.Text = Hero.Stamina.ToString();
            luck.Text = Hero.Luck.ToString();
            gold.Text = Hero.Gold.ToString();
        }

        private void DisplayChapter(int index) {
            try {
                Chapter = Chapter.Load(index);
                Hero.Update(Chapter);
                textDisplayer.AppendText(Chapter.Text);
                LastStableChapterIndex = index;
                DisplayHerosCaracteritics();
                ManageFightButtonVisibilty();
            } catch (Exception e) {
                DisplayChapter(LastStableChapterIndex);
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
                    int Entitlement = ((dice1.Value > 0) ? dice1.Value : dice2.Value) + 6;
                    Hero.Entitlement = Entitlement;
                    textDisplayer.AppendText("Votre habilité sera de  " + Entitlement);
                    textDisplayer.AppendText("\n");
                    textDisplayer.AppendText("Veuillez lancer les dés pour determiner votre endurance.");
                    textDisplayer.AppendText("\n");
                    phase = Constantes.STAMINA_PHASE;
                    break;
                case Constantes.STAMINA_PHASE:
                    int Stamina = dice1.Value + dice2.Value + 12;
                    Hero.Stamina = Stamina;
                    textDisplayer.AppendText("Votre endurance sera de  " + Stamina);
                    textDisplayer.AppendText("\n");
                    textDisplayer.AppendText("Veuillez lancer les dés pour determiner votre chance.");
                    textDisplayer.AppendText("\n");
                    phase = Constantes.LUCK_PHASE;
                    break;
                case Constantes.LUCK_PHASE:
                    int Luck = ((dice1.Value > 0) ? dice1.Value : dice2.Value) + 6;
                    Hero.Luck = Luck;
                    textDisplayer.AppendText("Votre chance sera de  " + Luck);
                    textDisplayer.AppendText("\n");
                    textDisplayer.AppendText("Cliquez sur Go pour commencer l'aventure");
                    phase = Constantes.EXPLORATION_PHASE;
                    DisplayChapter(1);
                    break;
                default:
                    break;
            }
            ResetDices();
        }


        /***** Actions ******/
        private void GotoNextPhase() {
            switch (phase) {
                case 1:
                    if (dice1.Value > 0 || dice2.Value > 0) {
                        DisplayHerosCreation();
                    }
                    break;
                case 2:
                    if (dice1.Value > 0 && dice2.Value > 0) {
                        DisplayHerosCreation();
                    }
                    break;
                case 3:
                    if (dice1.Value > 0 || dice2.Value > 0) {
                        DisplayHerosCreation();
                    }
                    break;
                default:
                    break;
            }
        }

        /***** Events ******/
        private void ChapterChangedAction(object sender, EventArgs e) {
            FightButton.Visibility = Visibility.Hidden;
            ClearTextDisplayer();
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
                Hero.LastChapter = DataConverter.ToInt(nextChapter.Text);
                Hero.Save();
                System.Windows.MessageBox.Show("Progression Sauvegardée");
            }
        }

        private void RollDiceOneAction(object sender, RoutedEventArgs e) {
            dice1.Roll();
            dice1Image.Source = new BitmapImage(new Uri(@"DiceFaces\" + dice1.Value.ToString() + ".png", UriKind.Relative));
            if (IsHeroCreationPhase()) {
                GotoNextPhase();
            } 
        }

        private void RollDiceTwoAction(object sender, RoutedEventArgs e) {
            dice2.Roll();
            dice2Image.Source = new BitmapImage(new Uri(@"DiceFaces\" + dice2.Value.ToString() + ".png", UriKind.Relative));
            if (IsHeroCreationPhase()) {
                GotoNextPhase();
            } 
        }

        private void FightAction(object sender, RoutedEventArgs e) {
            GoButton.Visibility = Chapter.EscapableFightPossibility ? Visibility.Visible : Visibility.Hidden;
            
            var window = new FightWindow { Owner = this };
            window.ShowDialog();

        }

       
        private void AfficherLigne(string line) {
            textDisplayer.AppendText(line);
            textDisplayer.AppendText("\n");
        }

        private void ManageFightButtonVisibilty() {
            FightButton.Visibility = Chapter.HasEnnemies() ? Visibility.Visible : Visibility.Hidden;
        }

        private void ResetDices() {
            dice1 = new Dice();
            dice2 = new Dice();
        }

        private void ClearTextDisplayer() {
            textDisplayer.Document.Blocks.Clear();
        }
    }
}
