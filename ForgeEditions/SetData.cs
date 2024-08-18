using Parsers;

namespace Sets{
    public class SetData{
        public string setFilePath;
        public string setName;
        public string setScryfallCode;
        public string setForgeCode;
        public string setType;
        public string setRelease;
        public string plane;
        public string sortingString;
        public List<string> cardNames;
        public Dictionary<string, List<string>> cardNameDictionary;

        public SetData(string setFilePath){
            this.setFilePath = setFilePath;
            string fileContent = File.ReadAllText(setFilePath);

            //type check
            setType = fileContent.Split("Type=")[1].Split("\n")[0].ToLower();
            setForgeCode = fileContent.Split("Code=")[1].Split("\n")[0];
            setName = fileContent.Split("Name=")[1].Split("\n")[0];
            setRelease = fileContent.Split("Date=")[1].Split("\n")[0];
        }

        public SetData(string setFilePath, string setName, string setScryfallCode, string setForgeCode, string setType, string setRelease, string plane = ""){
            this.setFilePath = setFilePath;
            this.setName = setName;
            this.setScryfallCode = setScryfallCode;
            this.setForgeCode = setForgeCode;
            this.setType = setType;
            this.setRelease = setRelease;
            this.plane = plane;
            
        }
        public void PopulateCards(){
            string fileContent = File.ReadAllText(setFilePath);
            List<string> cardsBlock = new List<string>(fileContent.Split("]")[2].Split("\n"));
            for(int e = 0; e < cardsBlock.Count; e++){
                if(cardsBlock[e].Length < 3 || cardsBlock[e][0] == '['){
                    cardsBlock.RemoveAt(e);
                    e--;
                    continue;
                }

                string[] splitCard = cardsBlock[e].Split(" ");
                string[] splitCardAd = cardsBlock[e].Split("@");

                if(splitCardAd.Length > 1){
                    cardsBlock[e] = cardsBlock[e].Replace(" @" + splitCardAd[1], "");
                }
                cardsBlock[e] = cardsBlock[e].Replace(splitCard[0] + " " + splitCard[1] + " ", "");
            }
            SetCardList(cardsBlock);
            Console.WriteLine("[SetData.PopulateCards]Populated set " + setName + " with " + cardNames.Count + " cards");
            Console.WriteLine("[SetData.PopulateCards]Populated setDictionary " + setName + " with " + cardNameDictionary.Keys.Count + " keys");
        }
        void SetCardList(List<string> cards){
            cardNameDictionary = new Dictionary<string, List<string>>();
            cardNames = new List<string>();
            List<string> unhandledCards = new List<string>();
            for(int i = 0; i < cards.Count; i++){
                try{
                    cardNameDictionary[cards[i].Substring(0, 1).ToLower()].Add(cards[i]);
                    cardNames.Add(cards[i]);
                }catch{
                    try{
                        cardNameDictionary.Add(cards[i].Substring(0, 1).ToLower(), new List<string>(){cards[i]});
                        cardNames.Add(cards[i]);
                    }catch{
                        unhandledCards.Add(cards[i]);
                    }
                }
            }
            if(unhandledCards.Count != 0){
                string cardList = "";
                foreach(string unhandled in unhandledCards){
                    cardList += "\n\t[" + unhandled.Length.ToString() + "]" + unhandled;
                }
                Console.WriteLine(setName + "'s unhandled cards:" + cardList);
            }
        }
        public void SetSortingString(string[] sortingOrder, List<string> typeSortingOrder){
            this.sortingString = "";
            foreach(string sorting in sortingOrder){
                switch(sorting){
                    case "type":
                        sortingString += typeSortingOrder.IndexOf(setType) == -1 ? "99999" : typeSortingOrder.IndexOf(setType).ToString("000");
                        break;
                    case "release":
                        sortingString += setRelease;
                        break;
                    case "alphabetical":
                        sortingString += setName;
                        break;
                }
            }
        }
        public bool HasCard(string cardName){
            if(cardNameDictionary == null) PopulateCards();
            try{
                return cardNameDictionary[cardName.Substring(0, 1).ToLower()].Contains(cardName);
            }catch{
                return false;
            }
        }
        public List<string> GetAllCards(){
            if(cardNames == null) PopulateCards();
            List<string> final = new List<string>();
            foreach(string key in cardNameDictionary.Keys){
                final.AddRange(cardNameDictionary[key]);
            }
            return final;
        }
        public string[] GetBannedCards(string[] cards){
            if(cardNames == null) PopulateCards();
            List<string> banned = new List<string>(cardNames);
            for(int i = 0; i < cards.Length; i++){
                banned.Remove(cards[i]);
            }
            return banned.ToArray();
        }
    }

    public class SimplePlaneData{
        public string planeName;
        public List<string> sets = new List<string>();
        public List<string> extraCards = new List<string>();
        public List<string> bannedCards = new List<string>();
        public SimplePlaneData(string planeName){
            this.planeName = planeName;
        }
        public string GetPlaneString(){
            string toReturn = "Name:" + planeName;
            if(sets.Count != 0) toReturn += "|Sets:" + String.Join(", ", sets);
            if(extraCards.Count != 0) toReturn += "|Extra:" + String.Join(";", extraCards);
            if(bannedCards.Count != 0) toReturn += "|Banned:" + String.Join(";", bannedCards);
            return toReturn;
        }
    }
    public class PlaneData{
        public string planeName;
        public List<SetData> includedSets = new List<SetData>();
        public List<string> includedSetCodes = new List<string>();
        public List<string> includedCards = new List<string>();
        public List<string> cardsNotFound = new List<string>();
        public List<string> bannedCards = new List<string>();
        public PlaneData(string planeName){
            this.planeName = planeName;
        }
        public SetData ContainsCard(string card){
            for(int i = 0; i < includedSets.Count; i++){
                if(includedSets[i].HasCard(card)){
                    return includedSets[i];
                }
            }
            return null;
        }
        public string GetPlaneString(string namePrefix){
            SetBannedCards();
            string toReturn = "Name:" + namePrefix + planeName + "|Sets:";
            toReturn += string.Join(", ", includedSetCodes);
            if(bannedCards.Count != 0){
                toReturn += "|Banned:" + string.Join("; ", bannedCards);
            }
            return toReturn;
        }
        public bool AddCard(string card){
            SetData includedCardSet = ContainsCard(card);
            if(includedCardSet == null){
                Console.WriteLine("[PlaneData.AddCard(cardName)]Card is not in already added sets");
                SetData forgeSet = Forge.FindCardInSets(card);
                if(forgeSet != null){
                    Console.WriteLine("[PlaneData.AddCard(cardName)]Found a set which includes the card");
                    includedSets.Add(forgeSet);
                    includedSetCodes.Add(forgeSet.setForgeCode);
                }else{
                    Console.WriteLine("[PlaneData.AddCard(cardName)]" + card + " has not been found");
                    cardsNotFound.Add(card);
                    return false;
                }
            }
            Console.WriteLine("[PlaneData.AddCard(cardName)]Adding card " + card);
            includedCards.Add(card);
            return true;
        }
        public bool AddCard(string card, string set){
            SetData setData = Forge.GetSet(set);
            if(setData == null) return false;
            if(!includedSets.Contains(setData)){
                includedSets.Add(setData);
                includedSetCodes.Add(setData.setForgeCode);
            }
            includedCards.Add(card);
            return true;
        }
        public void SetBannedCards(){
            bannedCards.Clear();
            foreach(SetData set in includedSets){
                bannedCards.AddRange(set.GetBannedCards(includedCards.ToArray()));
            }
        }
    }

}