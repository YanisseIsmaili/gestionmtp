namespace Yprotect.Utils
{
    public static class Messages
    {
        // Messages pour BDMotDictionnaire
        public const string General_Validation_IDMotDictionnaire_Required = "L'identifiant du mot de dictionnaire est requis.";
        public const string General_Validation_MotDictionnaire_Required = "Le mot de dictionnaire est requis.";
        public const string General_Validation_MotDictionnaire_StringLength = "Le mot de dictionnaire ne peut pas dépasser 50 caractères.";

        // Messages pour BDUtilisateur
        public const string General_Validation_IDUtilisateur_Required = "L'identifiant utilisateur est requis.";
        public const string General_Validation_Nom_Required = "Le nom est requis.";
        public const string General_Validation_Nom_StringLength = "Le nom ne peut pas dépasser 50 caractères.";
        public const string General_Validation_Prenom_Required = "Le prénom est requis.";
        public const string General_Validation_Prenom_StringLength = "Le prénom ne peut pas dépasser 50 caractères.";
        public const string General_Validation_Email_Required = "L'email est requis.";
        public const string General_Validation_Email_Format = "Le format de l'email n'est pas valide.";
        public const string General_Validation_MotDePasse_Required = "Le mot de passe est requis.";
        
        // Messages pour Admin
        public const string Default_Admin_Password = "AdminPassword123!";
    }
}