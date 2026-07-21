# Explorateur .NET HA-Magiline

Cette solution expérimentale permet de tester l'API locale du boîtier iMAGI-X depuis un PC Windows.

## Structure

- `Magiline.Protocol` : bibliothèque .NET 8 indépendante de WPF contenant le protocole HTTP local.
- `Magiline.Wpf` : interface graphique d'exploration pour Windows.

## Fonctions disponibles

- lecture de `GET /api/v1/pool/local` ;
- projecteur ON (`wanted=2`) et OFF (`wanted=1`) ;
- filtration avec six boutons envoyant `wanted=0` à `wanted=5` ;
- confirmation avant les valeurs encore inconnues `4` et `5` ;
- relecture automatique de l'état après une commande ;
- journal local dans `logs/` à côté de l'exécutable.

## État des valeurs de filtration

| Valeur | Interprétation actuelle | Niveau de confiance |
|---:|---|---|
| 0 | Auto | Confirmé expérimentalement |
| 1 | Marche continue | Confirmé expérimentalement |
| 2 | Arrêt | Confirmé expérimentalement |
| 3 | Auto global / variante automatique | Observé, à confirmer |
| 4 | Inconnu | Expérimental |
| 5 | Inconnu | Expérimental |

## Lancement

Prérequis : Windows et SDK .NET 8.

```powershell
cd experiments/dotnet
dotnet build Magiline.sln
dotnet run --project Magiline.Wpf/Magiline.Wpf.csproj
```

Renseigner l'adresse IPv4 locale du boîtier et conserver le port `11000`, sauf découverte contraire.

## Sécurité

Les boutons `4` et `5` envoient des valeurs dont la fonction est inconnue. Ils demandent une confirmation, mais doivent être testés uniquement sous surveillance directe de l'installation.

Ne pas committer les journaux contenant des adresses privées ou des données propres à la piscine.

## Portabilité future

Toute la logique réseau se trouve dans `Magiline.Protocol`. La bibliothèque pourra être réutilisée ultérieurement par une application console, un Worker Service Windows/Linux ou un service hébergé sur Raspberry Pi sans dépendance à WPF.
