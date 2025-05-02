using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace GestionBibliotheque
{
    public class Livre
    {
        public string Titre { get; set; }
        public int Annee { get; set; }
        public string Auteur { get; set; }
        public string ISBN { get; set; }
        public string Id { get; set; }
        public bool EstEmprunter { get; set; }  
        public string IdEmprunt { get; set; }    
    }

    public class Usager
    {
        public string Id { get; set; }
        public string Prenom { get; set; }
        public string Nom { get; set; }
        public string Etudiant { get; set; } 
    }

    public class Bibliotheque
    {
        public List<Livre> Livres { get; set; } = new List<Livre>();
        public List<Usager> Usagers { get; set; } = new List<Usager>();

        public void AddLivre(Livre livre) => Livres.Add(livre);
        public void Add(Usager usager) => Usagers.Add(usager);

        public void EnrigistrerDansFichier(string nomdufichier)
        {
            var json = JsonSauvegarder.Sauvergarder(this, new JsonSauvegarderOptions { WriteIndented = true });
            File.WriteAllText(filename, json);
        }

        public static Bibliotheque ChargerAPartirDuFichier(string nomdufichier)
        {
            if (Fichier.Exists(nomdufichier))
            {
                // filename = "c://AP1//MGtte//Library.json";
                var json = File.ReadAllText(nomdufichier);
                return JsonSauvegarde.Deserialize<Bibliotheque>(json);
            }
            return new Bibliotheque();
        }

        public Livre FindLivre(string id) => Livres.Find(l => l.Id == id);
        public Usager FindUsager(string id) => Usagers.Find(u => u.Id == id);

        public void LivreEstEmprunter(string LivreId, string UsagerId)
        {
            var livre = FindLivre(livreId);
            if (livre != null && !livre.EstEmprunter)
            {
                livre.EstEmprunter = true;
                livre.IdEmprunt = usagerId;
            }
        }

        public void RetournerLivre(string livreId)
        {
            var livre = FindLivre(livreId);
            if (livre != null)
            {
                livre.EstEmprunter = false;
                livre.IdEmprunt = null;
            }
        }

        public List<Livre> LivreDisponible() => Livres.FindAll(l => !l.IdEmprunt);
    }

    class Program
    {
        private static Bibliotheque bibliotheque = new Bibliotheque();
        private const string FichierDeDonnees = "bibliotheque.json";

        static void Main(string[] args)
        {
            Bibliotheque = Bibliotheque.ChargerAPartirDuFichier(FichierDeDonnees);

            while (true)
            {
                Console.Clear();
                Console.WriteLine("Système de Gestion de Bibliothèque");
                Console.WriteLine("1. Connexion Gestionnaire");
                Console.WriteLine("2. Connexion Utilisateur");
                Console.WriteLine("3. Quitter");
                var choix = Console.ReadLine();

                switch (choix)
                {
                    case "1": GestionMenu(); break;
                    case "2": MenuUsager(); break;
                    case "3": Bibliotheque.ChargerAPartirDuFichier(FichierDeDonnees); return;
                }
            }
        }

        static void GestionMenu()
        {
            Console.Write("Mot de passe: ");
            if (Console.ReadLine() != "admin") return;

            while (true)
            {
                Console.Clear();
                Console.WriteLine("Menu Gestionnaire");
                Console.WriteLine("1. Ajouter un livre");
                Console.WriteLine("2. Ajouter un utilisateur");
                Console.WriteLine("3. Liste des livres");
                Console.WriteLine("4. Retour");
                var choix = Console.ReadLine();

                switch (choix)
                {
                    case "1": AjouterUnLivre(); break;
                    case "2": AjouterUnUsager(); break;
                    case "3": MontrerUnLivre(); break;
                    case "4": return;
                }
            }
        }

        static void MenuUsager()
        {
            Console.Write("ID utilisateur: ");
            var usagerId = Console.ReadLine();
            var usager = bibliotheque.FindUsager(usagerId);
            if (usager == null)
            {
                Console.WriteLine("Utilisateur non trouvé!");
                Console.ReadKey();
                return;
            }

            while (true)
            {
                Console.Clear();
                Console.WriteLine($"Bienvenue {usager.Prenom} {usager.Nom}");
                Console.WriteLine("1. Emprunter un livre");
                Console.WriteLine("2. Retourner un livre");
                Console.WriteLine("3. Livres disponibles");
                Console.WriteLine("4. Retour");
                var choix = Console.ReadLine();

                switch (choix)
                {
                    case "1": EmprunterUnLivre(usagerId); break;
                    case "2": RetournerLivre(usagerId); break;
                    case "3": AfficherLesLivresDisponibles(); break;
                    case "4": return;
                }
            }
        }

        static void AjouterUnLivre()
        {
            var livre = new Livre();
            Console.Write("Titre: ");
            livre.Titre = Console.ReadLine();
            Console.Write("Année: ");
            livre.Annee = int.Parse(Console.ReadLine());
            Console.Write("Auteur: ");
            livre.Auteur = Console.ReadLine();
            Console.Write("ISBN: ");
            livre.ISBN = Console.ReadLine();
            livre.Id = Guid.NewGuid().ToString();
            bibliotheque.AjouterUnLivre(livre);
            bibliotheque.EnrigistrerDansFichier(FichierDeDonnees);
        }

        static void AjouterUnUsager()
        {
            var usager = new Usager();
            Console.Write("Prénom: ");
            usager.Prenom = Console.ReadLine();
            Console.Write("Nom: ");
            usager.Nom = Console.ReadLine();
            Console.Write("Type (Student/Teacher): ");
            usager.Etudiant = Console.ReadLine();
            usager.Id = Guid.NewGuid().ToString();
            bibliotheque.AjouterUnUsager(usager);
            bibliotheque.EnrigistrerDansFichier(FichierDeDonnees);
        }

        static void MontrerUnLivre()
        {
            foreach (var livre in bibliotheque.Livres)
            {
                Console.WriteLine($"{livre.Titre} ({livre.Annee}) - {livre.Auteur} - {(livre.EstEmprunter ? "Emprunté" : "Disponible")}");
            }
            Console.ReadKey();
        }

        static void EmprunterUnLivre(usagerId)
        {
            Console.Write("ID du livre: ");
            var Idlivre = Console.ReadLine();
            bibliotheque.EmprunterUnLivre(usagerId)
            bibliotheque.EnrigistrerDansFichier(FichierDeDonnees);
        }

        static void RetournerLivre(usagerId)
        {
            Console.Write("ID du livre: ");
            var Idlivre = Console.ReadLine();
            bibliotheque.RetournerLivre(usagerId);
            bibliotheque.EnrigistrerDansFichier(FichierDeDonnees); ;
        }

        static void AfficherLesLivresDisponibles()
        {
            foreach (var livre in bibliotheque.LivresDisponibles())
            {
                Console.WriteLine($"{livre.Titre} ({livre.Annee}) - {livre.Auteur}");
            }
            Console.ReadKey();
        }
    }
}