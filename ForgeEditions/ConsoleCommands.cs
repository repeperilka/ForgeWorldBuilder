using System.Diagnostics;

namespace ConsoleCommands{
    public static class ConsoleCommand{
        public static string RequestLabel(string[] possibleLabels, string description){
            string finalLabel = "";
            string error = "";
            while(finalLabel == ""){
                Console.Clear();
                Console.WriteLine(error);
                Console.WriteLine(description);
                finalLabel = Console.ReadLine();
                error = "";
                for(int i = 0; i < possibleLabels.Length; i++){
                    if(finalLabel == possibleLabels[i]){
                        goto ReturnLabel;
                    }
                }
                error = "Command not found.";
                finalLabel = "";
            }

            ReturnLabel:
            return finalLabel;
        }

        public static string[] RequestMultiLabel(string[] possibleLabels, string description){
            string finalLabel = "";
            string[] labelSplit = new string[0];
            string error = "";
            while(finalLabel == ""){
                AskForLabel:
                Console.Clear();
                Console.WriteLine(error);
                Console.WriteLine(description);
                finalLabel = Console.ReadLine();
                error = "";
                labelSplit = finalLabel.Split(" ");
                foreach(string label in labelSplit){
                    foreach(string possible in possibleLabels){
                        if(possible == label) goto LabelFound;
                    }

                    error += "Command " + label + " not valid.";
                    finalLabel = "";
                    goto AskForLabel;

                    LabelFound:
                    continue;
                }
            }

            ReturnLabel:
            return labelSplit;
        }

        public static string RequestForgeDirectory(string description){
            string finalLabel = "";
            string error = "";
            while(finalLabel == ""){
                Console.Clear();
                Console.WriteLine(error);
                Console.WriteLine(description);
                finalLabel = Console.ReadLine();
                error = "";
                if(Directory.Exists(finalLabel)){
                    if(Directory.Exists(finalLabel + "/res/editions") && Directory.Exists(finalLabel + "/res/quest/world")){
                        return finalLabel;
                    }else{
                        error = "Editions folder not found.";
                    }
                }else{
                    error = "Forge directory not found.";
                }
                finalLabel = "";
            }

            ReturnLabel:
            return finalLabel;
        }

        public static string RequestString(string description){
            Console.Clear();
            Console.WriteLine(description);
            return Console.ReadLine();
        }
    }
}