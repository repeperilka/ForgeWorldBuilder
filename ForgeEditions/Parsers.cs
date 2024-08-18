using Sets;

namespace Parsers{
    public static class Forge{
        public static string forgePath;
        public static List<SetData> allSets;
        public static Dictionary<string, SetData> setsByCode;
        public static Dictionary<string, List<SetData>> setsByType;
        /// <summary>
        /// Returns a full list of sets.
        /// </summary>
        /// <param name="forgePath"></param>
        /// <param name="sortingOrder"></param>
        /// <param name="typeSortingOrder"></param>
        /// <returns></returns>
        //"sortingOrder" tells the set the order of data to create it's "sortingString"
        //"typeSortingOrder" both acts as a filter for allowed set types, and replaces the setType data of the sets "sortingString"
        //by a number, allowing for custom set type sorting
        public static List<SetData> GetListOfSetsFromForge(string[] sortingOrder, List<string> typeSortingOrder){
            
            if(!Directory.Exists(forgePath + "/res/editions")){
                Console.WriteLine("[Forge.GetListOfSetsFromForge]editions directory not found");
                return null;
            }
            List<SetData> setsToReturn = new List<SetData>();
            string[] files = Directory.GetFiles(forgePath + "/res/editions");
            for(int i = 0; i < files.Length; i++){
                SetData newData = new SetData(files[i]);
                if(!typeSortingOrder.Contains(newData.setType.ToLower())){
                    continue;
                }
                newData.SetSortingString(sortingOrder, typeSortingOrder);
                setsToReturn.Add(newData);
            }
            setsToReturn.Sort(SortSets);
            return setsToReturn;
        }
        public static int SortSets(SetData a, SetData b){
            return string.Compare(a.sortingString, b.sortingString);
        }
    
        static void LoadAllSetsList(){
            allSets = GetListOfSetsFromForge(new string[0], new List<string>());
            Console.WriteLine("[Forge.LoadAllSetsList]Loaded " + allSets.Count + " sets");
        }
        static void LoadSetDictionary(){
            if(allSets == null) LoadAllSetsList();
            setsByCode = new Dictionary<string, SetData>();
            foreach(SetData set in allSets){
                setsByCode.Add(set.setForgeCode, set);
            }
            Console.WriteLine("[Forge.LoadSetDictionary]Loaded " + setsByCode.Keys.Count + " sets");
        }

        
        public static SetData GetSet(string setCode){
            if(setsByCode == null){
                LoadSetDictionary();
            }
            foreach(SetData set in allSets){
                if(set.setForgeCode.Equals(setCode)){
                    Console.WriteLine("[Forge.GetSet]Retrieving set " + setCode);
                    return set;
                }
            }
            Console.WriteLine("[Forge.GetSet]Set " + setCode + " not found.");
            return null;
        }
        public static SetData FindCardInSets(string cardName){
            if(allSets == null) LoadAllSetsList();
            for(int i = 0; i < allSets.Count; i++){
                if(allSets[i].cardNames == null) allSets[i].PopulateCards();
                if(allSets[i].HasCard(cardName)){
                    Console.WriteLine("[Forge.FindCardInSets]Card " + cardName + " found at set " + allSets[i].setName);
                    return allSets[i];
                }
            }
            Console.WriteLine("[Forge.FindCardInSets]Card " + cardName + " not found.");
            return null;
        }
    }
}