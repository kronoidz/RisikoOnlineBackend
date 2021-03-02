using System.Collections.Generic;

namespace RisikoOnline.Data
{
    public static class Continents
    {
        public static readonly Dictionary<Continent, List<Territory>> Map = new()
        {
            {
                Continent.NorthAmerica,
                new()
                {
                    Territory.Alaska, Territory.Alberta, Territory.CentralAmerica,
                    Territory.EasternUnitedStates, Territory.Greenland, Territory.NorthwestTerritory,
                    Territory.Ontario, Territory.Quebec, Territory.WesternUnitedStates
                }
            },
            {
                Continent.SouthAmerica,
                new()
                {
                    Territory.Argentina, Territory.Brazil, Territory.Peru, Territory.Venezuela
                }
            },
            {
                Continent.Europe,
                new()
                {
                    Territory.GreatBritain, Territory.Iceland, Territory.NorthernEurope,
                    Territory.Scandinavia, Territory.SouthernEurope, Territory.Ukraine,
                    Territory.WesternEurope
                }
            },
            {
                Continent.Africa,
                new()
                {
                    Territory.Congo, Territory.EastAfrica, Territory.Egypt, Territory.Madagascar,
                    Territory.NorthAfrica, Territory.SouthAfrica
                }
            },
            {
                Continent.Asia,
                new()
                {
                    Territory.Afghanistan, Territory.China, Territory.India, Territory.Irkutsk,
                    Territory.Yakutsk, Territory.Mongolia, Territory.Siam, Territory.Japan,
                    Territory.MiddleEast, Territory.Siberia, Territory.Ural, Territory.Kamchatka
                }
            },
            {
                Continent.Australia,
                new()
                {
                    Territory.EasternAustralia, Territory.WesternAustralia,
                    Territory.Indonesia, Territory.NewGuinea
                }
            }
        };

        public static readonly Dictionary<Continent, int> Armies = new()
        {
            {Continent.Australia, 2},
            {Continent.SouthAmerica, 2},
            {Continent.Africa, 3},
            {Continent.Europe, 5},
            {Continent.NorthAmerica, 5},
            {Continent.Asia, 7}
        };
    }
}