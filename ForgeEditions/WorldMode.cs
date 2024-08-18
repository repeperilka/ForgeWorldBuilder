using ConsoleCommands;
using Sets;
using Parsers;
using System.Security.Cryptography.X509Certificates;

namespace WorldMode{
    /// <summary>
    /// Contains the different ways to create a world: BuildBySet()
    /// </summary>
    public static class WorldBuilding{
        public static void BuildWorld(){
            string currentDirectory = System.Environment.CurrentDirectory;
            if(!Directory.Exists(currentDirectory + "/Custom")){
                Directory.CreateDirectory(currentDirectory + "/Custom");
            }

            string forgeDirectory = ConsoleCommand.RequestForgeDirectory("Write the current Forge folder path:");
            Forge.forgePath = forgeDirectory;

            //skips custom
            WorldBuilding.BuildBySet(forgeDirectory);
            return;

            string worldBuildingMode = ConsoleCommand.RequestLabel(
                new string[]{"sets", "custom"}, 
                "What type of world do you want to create\n" +
                "sets: based on released sets\n" +
                "custom: based on the Custom folder"
                );

            switch(worldBuildingMode){
                case "sets":
                    WorldBuilding.BuildBySet(forgeDirectory);
                    break;
                case "custom":
                    WorldBuilding.BuildCustom(forgeDirectory);
                    break;
            }
        }
        /// <summary>
        /// Builds the world based on the sets released, filters set types and orders them based on either release date, type of set or alphabetically
        /// </summary>
        public static void BuildBySet(string forgeDirectory){
            string currentDirectory = System.Environment.CurrentDirectory;
            string editionsDirectory = forgeDirectory + "/res/editions";
            string questDirectory = forgeDirectory + "/res/quest/world";

            //Asks the user for the types of sets they want for the world (expansions, core, reprints, etc.)
            //The order specified by the user here is the order at which they'll be sorted if they choose to sort by "type"
            #region TypeSelection
            List<string> allTypes = new List<string>(){
                "expansion",
                "core",
                "commander",        
                "reprint",
                "promo",
                "funny",
                "collector_edition",        
                "online",
                "boxed_Set",        
                "multiplayer",     
                "draft",
                "duel_Deck",        
                "starter",
                "other"
            };
            string typeAskTitle = "Write the types of sets you want in your world (separated by spaces):\n";
            foreach(string type in allTypes){
                typeAskTitle += type.ToLower() + " ";
            }
            typeAskTitle.Substring(typeAskTitle.Length - 2, 1);
            string[] types = ConsoleCommand.RequestMultiLabel(allTypes.ToArray(), typeAskTitle);
            #endregion
            
            //asks the user for the sorting of the different worlds: by type, by release date or alphabetically
            #region OrderSelection
            string[] sortingOrders = new string[]{
                "type",
                "release",
                "alphabetical"
            };

            string sortingTitle = "Set the sorting order (separated by spaces):\n";
            foreach(string sortingmethod in sortingOrders){
                sortingTitle += sortingmethod + " ";
            }
            string[] sortingOrder = ConsoleCommand.RequestMultiLabel(sortingOrders, sortingTitle);
            #endregion


            string finalFile = "Name:Main world\n" +
            "Name:Random Standard\n" +
            "Name:Random Pioneer\n" +
            "Name:Random Modern\n" +
            "Name:Random Commander\n";
            int index = 0;
            List<SetData> allSets = Forge.GetListOfSetsFromForge(sortingOrder, new List<string>(types));
            foreach(SetData set in allSets){
                string final = "Name:[" + index.ToString("000") + "]" + set.setType.ToUpper() + ": " + set.setName + "|Sets:" + set.setForgeCode + "\n"; 
                finalFile += final;
                index++;
            }

            string yno = ConsoleCommand.RequestLabel(new string[]{"y", "n"}, "Replace worlds.txt?\ny:yes (the program will place the file in its place)\nn:no (the program will create the file in the program's folder)");
            if(yno == "y"){
                if(!File.Exists(forgeDirectory + "/res/quest/world/worlds_backup.txt")){
                    File.Move(forgeDirectory + "/res/quest/world/worlds.txt", forgeDirectory + "/res/quest/world/worlds_backup.txt");
                }
                File.WriteAllText(forgeDirectory + "/res/quest/world/worlds.txt", finalFile);
            }else if(yno == "n"){
                File.WriteAllText(currentDirectory + "/worlds.txt", finalFile);
            }

            Console.Clear();
            Console.WriteLine("Finished");
            Console.WriteLine("Press enter to close");
            Console.ReadLine();
        }

        public static void BuildCustom(string forgeDirectory){
            string currentDirectory = System.Environment.CurrentDirectory;
            string editionsDirectory = forgeDirectory + "/res/editions";
            string questDirectory = forgeDirectory + "/res/quest/world";

            string[] customWorlds = Directory.GetDirectories(currentDirectory + "\\Custom");
            for(int i = 0; i < customWorlds.Length; i++){
                customWorlds[i] = customWorlds[i].Replace(currentDirectory + "\\Custom\\", "");
            }
            string worldToLoad = ConsoleCommand.RequestLabel(customWorlds, "Select a world to load:\n" + string.Join("\n", customWorlds));
            string[] planeFiles = Directory.GetFiles(currentDirectory + "\\Custom\\" + worldToLoad);
            List<SimplePlaneData> planes = new List<SimplePlaneData>();
            for(int file = 0; file < planeFiles.Length; file++){
                string fileDir = planeFiles[file];

                string extension = fileDir.Substring(fileDir.Length - 6, 6);
                if(extension != ".plane") continue;

                string[] fileLines = File.ReadAllText(fileDir).Split('\n', StringSplitOptions.RemoveEmptyEntries);
                SimplePlaneData plane = new SimplePlaneData(fileLines[0].Replace("Name:", "").Replace("\r", ""));
                for(int line = 1; line < fileLines.Length; line++){
                    string[] lineSplit = fileLines[line].Replace("\r", "").Split('|');
                    switch(lineSplit.Length){
                        case 1:
                            plane.extraCards.Add(lineSplit[0]);
                            break;
                    }
                }
                planes.Add(plane);
            }
            string worldFile = "Name:Main world\n";
            foreach(SimplePlaneData plane in planes){
                worldFile += plane.GetPlaneString() + "\n";
            }
            File.WriteAllText(currentDirectory + "/worlds.txt", worldFile);
        }
    
    }
}