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

        private bool IsHeroCreationPhase() {
            return (phase == Constantes.ENTITLEMENT_PHASE || phase == Constantes.STAMINA_PHASE || phase == Constantes.LUCK_PHASE);
        }

        private bool IsFightPhase() {
            return (phase == Constantes.FIGHT_PHASE);
        }

        /***** Display ******/
        private void DisplayHerosCaracteritics() {
            nextChapter.Text = hero.LastChapter.ToString();
            entitlement.Text = hero.Entitlement.ToString();
            stamina.Text = hero.Stamina.ToString();
            luck.Text = hero.Luck.ToString();
            gold.Text = hero.Gold.ToString();
        }

        private void DisplayChapter(int index) {
            try {
                chapter = Chapter.Load(index);
                hero.Update(chapter);
                textDisplayer.AppendText(chapter.Text);
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
                    hero.Entitlement = Entitlement;
                    textDisplayer.AppendText("Votre habilité sera de  " + Entitlement);
                    textDisplayer.AppendText("\n");
                    textDisplayer.AppendText("Veuillez lancer les dés pour determiner votre endurance.");
                    textDisplayer.AppendText("\n");
                    phase = Constantes.STAMINA_PHASE;
                    break;
                case Constantes.STAMINA_PHASE:
                    int Stamina = dice1.Value + dice2.Value + 12;
                    hero.Stamina = Stamina;
                    textDisplayer.AppendText("Votre endurance sera de  " + Stamina);
                    textDisplayer.AppendText("\n");
                    textDisplayer.AppendText("Veuillez lancer les dés pour determiner votre chance.");
                    textDisplayer.AppendText("\n");
                    phase = Constantes.LUCK_PHASE;
                    break;
                case Constantes.LUCK_PHASE:
                    int Luck = ((dice1.Value > 0) ? dice1.Value : dice2.Value) + 6;
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
                hero.LastChapter = DataConverter.ToInt(nextChapter.Text);
                hero.Save();
                System.Windows.MessageBox.Show("Progression Sauvegardée");
            }
        }

        private void RollDiceOneAction(object sender, RoutedEventArgs e) {
            dice1.Roll();
            dice1Image.Source = new BitmapImage(new Uri(@"DiceFaces\" + dice1.Value.ToString() + ".png", UriKind.Relative));
            if (IsHeroCreationPhase()) {
                GotoNextPhase();
            } else if (IsFightPhase()) {
                RoundFight();
            }
        }

        private void RollDiceTwoAction(object sender, RoutedEventArgs e) {
            dice2.Roll();
            dice2Image.Source = new BitmapImage(new Uri(@"DiceFaces\" + dice2.Value.ToString() + ".png", UriKind.Relative));
            if (IsHeroCreationPhase()) {
                GotoNextPhase();
            } else if (IsFightPhase()) {
                RoundFight();
            }
        }

        private void FightAction(object sender, RoutedEventArgs e) {
            GoButton.Visibility = chapter.EscapableFightPossibility ? Visibility.Visible : Visibility.Hidden;
            ClearTextDisplayer();
            phase = Constantes.FIGHT_PHASE;

            AfficherLigne("Phase de combat :\nVeuillez-lancer les deux dés");
        }

        private void RoundFight() {
            if (dice1.Value > 0 && dice2.Value > 0) {

                Enemy enemy = chapter.NextOpponent();
                if (enemy == null) {
                    AfficherLigne("Vous remportez le combat. Rendez-vous à la destination prévue");
                } else {

                    int power = dice1.Value + dice2.Value + enemy.Entitlement;
                    ResetDices();

                    AfficherLigne("Votre ennemi est un " + enemy.Name);
                    dice1.Roll();
                    AfficherLigne("Son habilité est " + enemy.Entitlement + " et son endurance est de " + enemy.Stamina);
                    dice2.Roll();
                    enemy.Power = dice1.Value + dice2.Value + enemy.Entitlement;
                    AfficherLigne("L'ennemi lance lance les dés et obtient  " + enemy.Power + " en Force");
                    
                    AfficherLigne("Vous obtenez   " + power + " en Force");
                    if (enemy.Power > power) {
                        AfficherLigne("L'ennemi gagne le round et vous inflige 2 points de dégats");
                        hero.Stamina -= 2;
                        DisplayHerosCaracteritics();
                    } else if (enemy.Power == power) {
                        AfficherLigne("Macht nul. Recommencez");
                    } else {
                        AfficherLigne("Vous gagnez le round et vous infligez 2 points de dégats  à l'ennemi");
                        enemy.Stamina -= 2;
                    }
                     ClearTextDisplayer();
                }
                ResetDices();
            }
        }

        private void AfficherLigne(string line) {
            textDisplayer.AppendText(line);
            textDisplayer.AppendText("\n");
        }

        private void ManageFightButtonVisibilty() {
            FightButton.Visibility = chapter.HasEnnemies() ? Visibility.Visible : Visibility.Hidden;
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
