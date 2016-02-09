using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestCV
{
    //Classe listant une série de méthode intéragissant avec la console windows (notamment Tesseract)
    class ConsoleFunct
    {
        System.Diagnostics.ProcessStartInfo info;

        public ConsoleFunct()
        {
            info = new System.Diagnostics.ProcessStartInfo();
            //Permet de ne pas ouvrir de fenêtre CMD à chaque commande et de rediriger la sortie sur la console du programme
            info.UseShellExecute = false;
            //Redirection des sortie en dehors de la console (mettre false si problème avec la commande pour lire la sortie d'info/erreur)
            info.RedirectStandardOutput = true;
            info.RedirectStandardError = true;
        }

        //Envoi une commande à la console (cmd = commande, param = paramètres de la commande)
        public void consoleCommande(String cmd, String param)
        {
            //Commande et paramètre
            info.FileName = cmd;
            info.Arguments = param;

            //Création d'un processus
            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.StartInfo = info;
            p.Start();
            p.WaitForExit();
            p.Dispose();
        }

        //Commande tesseract sur le fichier passé en paramètre
        public void tesseractCommande(String pathFichier)
        {
            consoleCommande("tesseract", "-l FRA \"" + pathFichier + "\" \"" + Program.cheminTemp + "tesseract - temp\"");
        }

        //Commande tesseract cherchant uniquement des nombres (ignore les autres caractères) sur le fichier passé en paramètre
        public void tesseractNombreCommande(String pathFichier)
        {
            consoleCommande("tesseract", "-l FRA \"" + pathFichier + "\" \"" + Program.cheminTemp + "tesseract - temp\" nobatch digits");
        }
    }
}
