using System;
using System.ComponentModel;

namespace AnnuaireApp.Helpers
{
    public static class AdminManager
    {
        public static bool IsAdmin { get;  set; } = false;


        public static event Action? AdminStateChanged; // Pour notifier l'UI

        public static void LoginAsAdmin()
        {
            IsAdmin = true;
            AdminStateChanged?.Invoke(); // Notifie que l'admin est connecté
        }

        public static void Logout()
        {
            IsAdmin = false;
            AdminStateChanged?.Invoke(); // Notifie que l'admin est déconnecté
        }
    }
}

