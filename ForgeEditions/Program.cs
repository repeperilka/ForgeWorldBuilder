
using System.Diagnostics;
using ConsoleCommands;
using Parsers;
using Scryfall;
using Sets;
using WorldMode;


WorldBuilding.BuildWorld();
return;

List<ScryfallCard> cards = await ScryfallManager.SearchCards("t:dog");
foreach(ScryfallCard card in cards){
    Console.WriteLine(card.name);
}




