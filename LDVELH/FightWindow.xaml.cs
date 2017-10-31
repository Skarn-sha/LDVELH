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
using System.Windows.Shapes;

namespace LDVELH {
    public partial class FightWindow : Window {
        Dice dice1 = new Dice();
        Dice dice2 = new Dice();
        Chapter chapter;
        Hero hero;
        Enemy currentEnemy;

        public FightWindow() {
            InitializeComponent();
            dice1Image.Source = new BitmapImage(new Uri(@"DiceFaces\0.png", UriKind.Relative));
            dice2Image.Source = new BitmapImage(new Uri(@"DiceFaces\0.png", UriKind.Relative));
            var main = App.Current.MainWindow as MainWindow;
            hero = main.Hero;
            chapter = main.Chapter;
            DisplayHerosCaracteritics();
            currentEnemy = Chapter.NextOpponent();
            AfficherLigne("Phase de combat :\nVeuillez-lancer les deux dés");
        }

        public Hero Hero { get => hero; set => hero = value; }
        internal Chapter Chapter { get => chapter; set => chapter = value; }

        private void RollDiceOneAction(object sender, RoutedEventArgs e) {
            dice1.Roll();
            dice1Image.Source = new BitmapImage(new Uri(@"DiceFaces\" + dice1.Value.ToString() + ".png", UriKind.Relative));
            RoundFight();
        }

        private void RollDiceTwoAction(object sender, RoutedEventArgs e) {
            dice2.Roll();
            dice2Image.Source = new BitmapImage(new Uri(@"DiceFaces\" + dice2.Value.ToString() + ".png", UriKind.Relative));
            RoundFight();
        }

        private void DisplayHerosCaracteritics() {
            entitlement.Text = hero.Entitlement.ToString();
            stamina.Text = hero.Stamina.ToString();
            luck.Text = hero.Luck.ToString();
        }

        private void AfficherLigne(string line) {
            textDisplayer.AppendText(line);
            textDisplayer.AppendText("\n");
        }

        private void RoundFight() {
            if (dice1.Value > 0 && dice2.Value > 0) {
                if (currentEnemy == null) {
                    System.Windows.MessageBox.Show("Fin du combat");
                    var main = App.Current.MainWindow as MainWindow;
                    main.Hero = hero;
                    main.DisplayHerosCaracteritics();
                    this.Close();
                } else {
                    ClearTextDisplayer();

                    int power = dice1.Value + dice2.Value + currentEnemy.Entitlement;
                    ResetDices();

                    AfficherLigne("Votre ennemi est un " + currentEnemy.Name);
                    dice1.Roll();
                    AfficherLigne("Son habilité est " + currentEnemy.Entitlement + " et son endurance est de " + currentEnemy.Stamina);
                    dice2.Roll();
                    currentEnemy.Power = dice1.Value + dice2.Value + currentEnemy.Entitlement;
                    AfficherLigne("L'ennemi lance lance les dés et obtient  " + currentEnemy.Power + " en Force");

                    AfficherLigne("Vous obtenez   " + power + " en Force");
                    if (currentEnemy.Power > power) {
                        AfficherLigne("L'ennemi gagne le round et vous inflige 2 points de dégats");
                        Hero.Stamina -= 2;
                        DisplayHerosCaracteritics();
                    } else if (currentEnemy.Power == power) {
                        AfficherLigne("Macht nul. Recommencez");
                    } else {
                        AfficherLigne("Vous gagnez le round et vous infligez 2 points de dégats  à l'ennemi");
                        currentEnemy.Stamina -= 2;
                        if (currentEnemy.Stamina <= 0) {
                            AfficherLigne("Votre ennemi est vaincu");
                            currentEnemy = Chapter.NextOpponent();
                        }
                    }
                    ResetDices();
                }
            }
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
