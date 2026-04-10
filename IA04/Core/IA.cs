using IACore.Model;

namespace IACore
{
    /// <summary>
    /// IA : Intelligence Artificielle, il s'agit de la structure principale qui permet de traiter une input et rendre une réponse complète
    /// </summary>
    public class IA
    {
        /// <summary>
        /// Network : liste des couches (layers) par lesquelles passe l'input pour générer une réponse
        /// </summary>
        private List<Layer> _network;
        public List<Layer> Network
        {
            get { return _network; }
            set { _network = value; }
        }

        /// <summary>
        /// IAUsage : role de l'IA, pour l'instant comme ça mais utile si je veux ajouter par exemple génération de texte
        /// </summary>
        public enum IAUsage
        {
            imageRecognisation
        }

        /// <summary>
        /// Role : definit ce que réalise l'IA
        /// </summary>
        private IAUsage _role;
        public IAUsage Role
        {
            get { return _role; }
            set { _role = value; }
        }

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="network"></param>
        /// <param name="role"></param>
        public IA(List<Layer> network, IAUsage role)
        {
            this._network = network;
            this._role = role;
        }

        //méthodes
        //charger les data
        //générer une réponse
        //corriger le réseau
    }
}
