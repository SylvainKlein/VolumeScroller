<div align="center">
  <img src="VolumeScroller-icon.png" alt="Volume Scroller Logo" height="180" />
  
  <h1><font size="7">Volume Scroller</font></h1>
  
  <h3>Contrôleur de volume Windows rapide, natif et ultra-léger au bout des doigts.</h3>
  
  <p>
    Ajustez le volume sonore principal de Windows sans effort grâce à la molette de votre souris — n'importe où sur votre écran ou directement sur la barre des tâches.
  </p>

  <br/>

  <p>
    <a href="https://github.com"><img src="https://img.shields.io/badge/Plateforme-Windows%2010%20%7C%2011-0078D6?logo=windows&logoColor=white" alt="Plateforme Windows" height="20" /></a>
    <a href="https://dotnet.microsoft.com"><img src="https://img.shields.io/badge/.NET-9.0-512BD4?logo=dotnet&logoColor=white" alt=".NET 9" height="20" /></a>
    <a href="#"><img src="https://img.shields.io/badge/UI-Pastille%20OSD%20Flottante-2d3748" alt="UI" height="20" /></a>
    <a href="#"><img src="https://img.shields.io/badge/Architecture-Zéro%20Electron-success" alt="Zéro Electron" height="20" /></a>
    <a href="LICENSE"><img src="https://img.shields.io/badge/Licence-MIT-blue.svg" alt="Licence MIT" height="20" /></a>
    <a href="../../releases/latest"><img src="https://img.shields.io/badge/Version-1.0.0-blue" alt="Dernière Version" height="20" /></a>
  </p>
  
  <p>
    🇬🇧 <em><a href="README.md">English version available here</a></em>
  </p>
</div>

> [!NOTE]
> ### ☕ Un petit mot de l'auteur
> **Pour être tout à fait transparent :** Je ne suis pas développeur, et l'écosystème GitHub / open source est un univers totalement nouveau pour moi !
> 
> J'ai simplement **vibe codé** Volume Scroller par besoin personnel au quotidien : je cherchais un moyen simple, instantané et fluide de régler le son sur mon PC sans toucher aux touches médias du clavier, sans changer de fenêtre et sans installer une usine à gaz en tâche de fond.
> 
> Si vous ouvrez des Issues ou des Pull Requests, **soyez indulgents et laissez-moi un peu de temps** pour les traiter : le dev n'est pas du tout mon métier. Merci infiniment pour votre bienveillance et votre compréhension ! ❤️

---

## 💡 Pourquoi Volume Scroller ?

Gérer le son sur Windows est souvent contraignant : il faut soit viser la minuscule icône de haut-parleur dans la barre des tâches, soit atteindre des touches de fonction sur le clavier, soit installer de lourds utilitaires qui consomment beaucoup de mémoire et créent des conflits de raccourcis.

**Volume Scroller** repose sur un principe clair : **un contrôle instantané, ergonomique et zéro fioriture.**
* **Alt + Molette** n'importe où sur votre bureau ou dans vos applications.
* **Molette sur la barre des tâches** d'un simple coup de souris, à une main sans toucher au clavier.
* Une élégante pastille OSD flottante suit votre curseur en temps réel et garantit une visibilité totale sans jamais voler le focus de vos fenêtres.

---

## ⚡ Caractéristiques Principales

* **🚀 100% Natif Windows & Zéro Techno Web**
  * Développé en **C# / .NET 9 + APIs Win32 CoreAudio natives**.
  * Zéro Electron, zéro Chromium, zéro webview, aucune télémétrie en arrière-plan.
  * Exécutable autonome ultra-compact (~195 Ko), consommation CPU quasi nulle et ~15 Mo de RAM.

* **🖱️ Double Déclencheur Intuitif & Naturel**
  * **`Alt` + Molette** : Ajustez le volume instantanément où que soit votre curseur (en jeu, devant une vidéo plein écran, dans vos logiciels de création).
  * **Molette sur la barre des tâches** : Survolez simplement la barre des tâches (écran principal ou multi-écrans) et scrollez à la souris sans toucher au clavier.
  * **Interception du défilement** : Le défilement de la page ou du document sous la souris est automatiquement neutralisé lors du réglage de volume.

* **🎯 Pastille OSD Flottante Intelligente (au Curseur)**
  * Affiche une pilule sombre semi-transparente moderne directement au niveau de votre curseur de souris, avec pourcentage en temps réel (`XX%`) et icône adaptative.
  * **Garde-fous anti-débordement** : Si votre curseur approche des bords ou des coins de l'écran, la pastille est automatiquement retenue dans l'écran visible et ne dépasse jamais. Si vous êtes tout en haut, elle bascule automatiquement sous le curseur.
  * **Premier Plan Absolu (`HWND_TOPMOST`)** : Reste toujours visible au premier plan, y compris par-dessus les vidéos ou fenêtres maximisées.
  * **Non-intrusive** : Transparente aux clics (`IsHitTestVisible="False"`) et ne vole jamais le focus de vos fenêtres ou jeux en cours (`SWP_NOACTIVATE`).

* **🔊 Intégration Directe Windows CoreAudio**
  * Dialogue directement avec l'interface COM native Windows (`IAudioEndpointVolume`).
  * Réponse immédiate sans aucune latence.
  * **Unmute automatique** : Réactive automatiquement le son si vous montez le volume alors qu'il était coupé (*Mute*).

* **⚙️ Gestion Simplifiée du Démarrage avec Windows**
  * **À l'installation** : Cochez l'option pendant l'installeur pour activer le lancement automatique au démarrage de Windows.
  * **Au quotidien** : Clic droit sur l'icône de la barre des tâches pour activer ou désactiver l'option **"Démarrer avec Windows"** à tout moment, sans fouiller dans les paramètres du système.
  * Clé de registre propre et isolée dans `HKCU\Software\Microsoft\Windows\CurrentVersion\Run`.

---

## 📦 Téléchargement & Installation

Rendez-vous sur la page **[GitHub Releases](../../releases/latest)** pour télécharger la version de votre choix :

| Format | Description | Utilisation |
|---|---|---|
| **`VolumeScroller-Setup.exe`** | Installeur officiel Windows (Inno Setup) | Installe l'application avec raccourcis, choix du démarrage auto et désinstalleur propre. |
| **`VolumeScroller.exe`** | Exécutable autonome portable (~195 Ko) | Double-cliquez pour lancer immédiatement sans installation. |
| **`VolumeScroller-Portable.zip`** | Archive ZIP de la version portable | Décompressez où vous voulez et lancez. |

---

## 🎮 Raccourcis & Contrôles

| Action | Effet |
|---|---|
| <kbd>Alt</kbd> + <kbd>Molette Haut</kbd> | Augmente le volume (+2%) |
| <kbd>Alt</kbd> + <kbd>Molette Bas</kbd> | Diminue le volume (-2%) |
| <kbd>Molette</kbd> sur la barre des tâches | Ajuste le son à la souris seule |
| **Double-clic sur l'icône systray** | Affiche / teste la pastille OSD flottante |
| **Clic droit sur l'icône systray** | Ouvre le menu contextuel (Démarrer avec Windows, Tester l'OSD, Quitter) |

---

## 📁 Organisation du Projet

```text
VolumeScroller/
├── installer/
│   └── VolumeScroller.iss           # Script de compilation Inno Setup
├── publish/
│   ├── Installer/
│   │   └── VolumeScroller-Setup.exe # Installeur officiel complet
│   ├── VolumeScroller-Portable/
│   │   ├── VolumeScroller.exe       # Exécutable autonome portable
│   │   └── icon.ico
│   └── VolumeScroller-Portable.zip  # Archive portable compressée
├── src/
│   └── VolumeScroller/              # Code source C# / .NET 9
│       ├── Audio/                   # Interfaces COM Windows CoreAudio
│       ├── Input/                   # Hook souris bas niveau & détection barre des tâches
│       ├── Startup/                 # Gestionnaire de démarrage registre Windows
│       ├── Tray/                    # Icône zone de notification & menu contextuel
│       └── UI/                      # Fenêtre OSD flottante (WPF)
├── build.ps1                        # Script 1-clic de compilation et packaging
├── icon.ico                         # Icône Windows de l'application
├── VolumeScroller-icon.png          # Logo haute résolution pour la documentation
├── README.md                        # Documentation en anglais (GitHub)
└── README-fr.md                     # Documentation en français
```

---

## 🛠️ Compilation depuis les sources

### Prérequis
* [.NET 9.0 SDK](https://dotnet.microsoft.com/download)
* *(Optionnel, pour générer l'installeur)* [Inno Setup 6](https://jrsoftware.org/isdl.php)

### Compilation 1-Clic (PowerShell)
```powershell
.\build.ps1
```
Ce script :
1. Compile l'exécutable portable Release en fichier unique dans `publish/VolumeScroller-Portable/`.
2. Génère l'archive ZIP portable.
3. Déclenche Inno Setup pour produire automatiquement l'installeur `publish/Installer/VolumeScroller-Setup.exe`.

---

## 📄 Licence

Ce projet est open source et distribué sous licence [MIT](LICENSE).
