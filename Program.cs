﻿using System;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using static spartanDungeon.Program;

namespace spartanDungeon
{
    class Program
    {
        public static string savePath = "";

        //게임 규칙 선언
        public enum ItemTypes { 무기, 방어구, 방패 };
        public static int[] myEquipment = new int[3];
        public enum Abilitys { 공격력, 방어력 };
        public static int[] myAddStat = new int[2];


        //게임 데이터 관련 변수 선언
        public static Character player;
        public static List<Item> myItem = new();
        public static List<Item> shop = new();
        public static int[] maxExp = { 0, 1, 2, 3, 4 };

        //아이템 정렬관련 변수 선언
        public static int sort = 0;
        public static bool order = true;

        /// <summary>게임 시작</summary>
        static void Main(string[] args)
        {
            GameDataSetting(0);
            DisplayGameIntro();
        }

        /// <summary>초기 세팅</summary>
        static void GameDataSetting(int set)
        {
            // data.xml 파일 읽기
            // 프로젝트 경로와, 솔루션 명을 활용하여 파일경로를 설정하고 해당 파일을 읽기
            string projectPath = Directory.GetParent(Environment.CurrentDirectory).Parent.FullName;
            string solutionName = Assembly.GetEntryAssembly().GetName().Name;

            // 게임 저장을 위한 경로 저장
            savePath = projectPath.Substring(0,projectPath.IndexOf(solutionName)+ solutionName.Length);

            // xml 파일 읽기
            // 게임 시작 0, 사망시 초기화 1
            XmlDocument xmlDoc = new XmlDocument();
            if(set == 0)
            {
                xmlDoc.Load($"{savePath}/data.xml");
            }
            else
            {
                xmlDoc.Load($"{savePath}/resetData.xml");
            }
           
            // xml 파싱
            // 아이템 정보 파싱
            XmlNodeList itemNode = xmlDoc.SelectNodes("Data/Items/Item");
            foreach (XmlNode item in itemNode)
            {
                XmlNodeList itemAbilitieNode = item.SelectNodes("ItemAbilitys/ItemAbility");
                List<ItemAbility> itemAbility = new List<ItemAbility>();
                foreach (XmlNode ability in itemAbilitieNode)
                {
                    itemAbility.Add(new ItemAbility(int.Parse(ability["Ability"].InnerText), int.Parse(ability["Stat"].InnerText)));
                }

                shop.Add(new Item(int.Parse(item["ItemId"].InnerText), item["ItemName"].InnerText, int.Parse(item["ItemType"].InnerText), itemAbility, item["Described"].InnerText, int.Parse(item["Price"].InnerText)));
            }

            // 캐릭터 정보 파싱
            XmlNode playerNode = xmlDoc.SelectSingleNode("Data/PlayerData");
            
            // 캐릭터 정보 세팅
            player = new Character(playerNode["Name"].InnerText, playerNode["Job"].InnerText, int.Parse(playerNode["Level"].InnerText), int.Parse(playerNode["Atk"].InnerText), int.Parse(playerNode["Def"].InnerText), int.Parse(playerNode["Hp"].InnerText), int.Parse(playerNode["Gold"].InnerText), int.Parse(playerNode["Exp"].InnerText));

            // 아이템 정보 세팅
            myItem = new();
            XmlNodeList myItemNode = playerNode.SelectNodes("MyItems/MyItem");
            foreach (XmlNode item in myItemNode)
            {
                // 장착 여부 검증
                bool equipment = bool.Parse(item["Equipment"].InnerText);
                // 장착
                if (equipment) myEquipment[int.Parse(item["ItemType"].InnerText)] = int.Parse(item["ItemId"].InnerText);
             
                XmlNodeList itemAbilitieNode = item.SelectNodes("ItemAbilitys/ItemAbility");
                List<ItemAbility> itemAbility = new List<ItemAbility>();
                foreach (XmlNode ability in itemAbilitieNode)
                {
                    itemAbility.Add(new ItemAbility(int.Parse(ability["Ability"].InnerText), int.Parse(ability["Stat"].InnerText)));
                }

                myItem.Add(new Item(int.Parse(item["ItemId"].InnerText), equipment, item["ItemName"].InnerText, int.Parse(item["ItemType"].InnerText), itemAbility, item["Described"].InnerText));
            
            }

            //장비 추가 스텟 적용
            AddStat();
        }

        /// <summary>게임 초기 화면 출력</summary>
        static void DisplayGameIntro()
        {
            Console.Clear();
            Console.WriteLine("스파르타 마을에 오신 여러분 환영합니다.");
            Console.WriteLine("이곳에서 전전으로 들어가기 전 활동을 할 수 있습니다.");
            Console.WriteLine();
            Console.WriteLine("1. 상태보기");
            Console.WriteLine("2. 인벤토리");
            Console.WriteLine("3. 상점");
            Console.WriteLine("4. 던전입장");
            Console.WriteLine("5. 휴식하기");
            Console.WriteLine();
            Console.WriteLine("원하시는 행동을 입력해주세요.");

            int input = CheckValidInput(1, 5);
            switch (input)
            {
                case 1:
                    DisplayMyInfo();
                    break;
                case 2:
                    DisplayInventory();
                    break;
                case 3:
                    DisplayShop();
                    break;
                case 4:
                    DisplayDungeon();
                    break;
                case 5:
                    DisplayRest("");
                    break;
            }
        }

        /// <summary>케릭터 정보 화면 출력</summary>
        static void DisplayMyInfo()
        {
            AddStat();
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("상태보기");
            Console.ResetColor();
            Console.WriteLine("캐릭터의 정보르 표시합니다.");
            Console.WriteLine();
            Console.WriteLine($"Lv.{player.Level}");
            Console.WriteLine($"{player.Name}({player.Job})");
            Console.Write($"공격력 : {player.Atk} ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{(myAddStat[0] > 0 ? "(+" + myAddStat[0] + ")" : "")}");
            Console.ResetColor();
            Console.Write($"방어력 : {player.Def} ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{(myAddStat[1] > 0 ? "(+" + myAddStat[1] + ")" : "")}");
            Console.ResetColor();
            Console.WriteLine($"체력 : {player.Hp}");
            Console.WriteLine($"Gold : {player.Gold} G");
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("0. 나가기");
            Console.ResetColor();
            Console.WriteLine();
            Console.WriteLine("원하시는 행동을 입력해주세요.");

            int input = CheckValidInput(0, 0);
            switch (input)
            {
                case 0:
                    DisplayGameIntro();
                    break;
            }
        }

        /// <summary>착용한 장비의 스텟 계산 메소드</summary>
        static void AddStat()
        {
            myAddStat[0] = 0;
            myAddStat[1] = 0;
            foreach (Item item in myItem)
            {
                if (item.Equipment)
                {
                    foreach (ItemAbility itemAbility in item.ItemAbilitys)
                    {
                        switch (itemAbility.Ability)
                        {
                            case Abilitys.공격력:
                                myAddStat[0] += itemAbility.Stat;
                                break;
                            case Abilitys.방어력:
                                myAddStat[1] += itemAbility.Stat;
                                break;
                        }
                    }
                }
            }
        }

        /// <summary>인벤토리 화면 출력</summary>
        static void DisplayInventory()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("인벤토리");
            Console.ResetColor();
            Console.WriteLine("보유 중인 아이템을 관리할 수 있습니다.");
            Console.WriteLine();
            Console.WriteLine("[아이템 목록]");
            foreach (Item item in myItem)
            {

                // 한글 한글자당 2자리를 차지하여 공백을 먼저 계산 해야 함
                // 일이삼사오육칠팔구십, 일B삼D오F칠H구J, ABCDEFGHIJ 등 다양한 경우에서 모두 동일한 공백을 갖도록 계산
                string str = item.ItemName;
                int padLen = 0;
                for (int i = 0; i < str.Length; i++)
                {
                    // 한글 한글자당 3바이트 씩 사용
                    if (Encoding.Default.GetBytes(str.Substring(i, 1)).Length == 3)
                        padLen++;
                }

                padLen = 20 - padLen;

                if (item.Equipment) Console.ForegroundColor = ConsoleColor.Green;

                Console.WriteLine($" - {(item.Equipment ? "[E]" : "   ")}| {str.PadRight(padLen)} | {item.ItemAbilitys[0].Ability} +{item.ItemAbilitys[0].Stat.ToString().PadRight(2)} | {item.Described}");
                if (item.ItemAbilitys.Count > 1)
                {
                    for (int i = 1; i < item.ItemAbilitys.Count; i++)
                    {
                        string whiteSpace = "";
                        Console.WriteLine($"{whiteSpace.PadRight(28, ' ')} | {item.ItemAbilitys[i].Ability} +{item.ItemAbilitys[i].Stat.ToString().PadRight(2)} |");
                    }
                }

                if (item.Equipment) Console.ResetColor();
            }
            Console.WriteLine("1. 아이템 정렬");
            Console.WriteLine("2. 장착 관리");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("0. 나가기");
            Console.ResetColor();
            Console.WriteLine();
            Console.WriteLine("원하시는 행동을 입력해주세요.");

            int input = CheckValidInput(0, 2);
            switch (input)
            {
                case 1:
                    DisplayInventorySort();
                    break;
                case 2:
                    DisplayEquipment("");
                    break;
                case 0:
                    DisplayGameIntro();
                    break;
            }
        }

        /// <summary>인벤토리 정렬 화면 출력</summary>
        static void DisplayInventorySort()
        {
       
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("인벤토리 - 정렬");
            Console.ResetColor();
            Console.WriteLine("보유 중인 아이템을 관리할 수 있습니다.");
            Console.WriteLine();
            Console.WriteLine("[아이템 목록]");
            foreach (Item item in myItem)
            {

                // 한글 한글자당 2자리를 차지하여 공백을 먼저 계산 해야 함
                // 일이삼사오육칠팔구십, 일B삼D오F칠H구J, ABCDEFGHIJ 등 다양한 경우에서 모두 동일한 공백을 갖도록 계산
                string str = item.ItemName;
                int padLen = 0;
                for (int i = 0; i < str.Length; i++)
                {
                    // 한글 한글자당 3바이트 씩 사용
                    if (Encoding.Default.GetBytes(str.Substring(i, 1)).Length == 3)
                        padLen++;
                }

                padLen = 20 - padLen;

                if (item.Equipment) Console.ForegroundColor = ConsoleColor.Green;

                Console.WriteLine($" - {(item.Equipment ? "[E]" : "   ")}| {str.PadRight(padLen)} | {item.ItemAbilitys[0].Ability} +{item.ItemAbilitys[0].Stat.ToString().PadRight(2)} | {item.Described}");
                if (item.ItemAbilitys.Count > 1)
                {
                    for (int i = 1; i < item.ItemAbilitys.Count; i++)
                    {
                        string whiteSpace = "";
                        Console.WriteLine($"{whiteSpace.PadRight(28, ' ')} | {item.ItemAbilitys[i].Ability} +{item.ItemAbilitys[i].Stat.ToString().PadRight(2)} |");
                    }
                }

                if (item.Equipment) Console.ResetColor();
            }
            Console.WriteLine("1. 이름");
            Console.WriteLine("2. 장착순");
            Console.WriteLine("3. 공격력");
            Console.WriteLine("4. 방어력");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("0. 나가기");
            Console.ResetColor();
            Console.WriteLine();
            Console.WriteLine("원하시는 행동을 입력해주세요.");

            int input = CheckValidInput(0, 4);
            switch (input)
            {
                case 1:
                case 2:
                case 3:
                case 4:
                    ChangeOrder(input);
                    break;

                case 0:
                    DisplayInventory();
                    break;
            }
        }

        /// <summary>인벤토리 정렬 메소드</summary>
        static void ChangeOrder(int type)
        {
            switch (type)
            {
                case 1:
                    if (sort == type)
                    {
                        if (order)
                        {
                            myItem = myItem.OrderByDescending(item => item.ItemName).ToList();
                            order = false;
                        }
                        else
                        {
                            myItem = myItem.OrderBy(item => item.ItemName).ToList();
                            order = true;
                        }
                    }
                    else
                    {
                        sort = type;
                        myItem = myItem.OrderBy(item => item.ItemName).ToList();
                        order = true;
                    }
                    break;
                case 2:
                    if (sort == type)
                    {
                        if (order)
                        {
                            myItem = myItem.OrderBy(item => item.Equipment).ToList();
                            order = false;
                        }
                        else
                        {
                            myItem = myItem.OrderByDescending(item => item.Equipment).ToList();
                            order = true;
                        }
                    }
                    else
                    {
                        sort = type;
                        myItem = myItem.OrderByDescending(item => item.Equipment).ToList();
                        order = true;
                    }
                    break;
                case 3:
                    if (sort == type)
                    {
                        if (order)
                        {
                            myItem = myItem.OrderBy(item => item.Stat).ToList();
                            order = false;
                        }
                        else
                        {
                            myItem = myItem.OrderByDescending(item => item.Stat).ToList();
                            order = true;
                        }
                    }
                    else
                    {
                        foreach (Item item in myItem)
                        {
                            item.Stat = -1;
                            foreach (ItemAbility itemAbility in item.ItemAbilitys)
                            {
                                if (itemAbility.Ability == Abilitys.공격력)
                                {
                                    item.Stat = itemAbility.Stat;
                                }
                            }
                        }
                        sort = type;
                        myItem = myItem.OrderByDescending(item => item.Stat).ToList();
                        order = true;
                    }
                    break;
                case 4:
                    if (sort == type)
                    {
                        if (order)
                        {
                            myItem = myItem.OrderBy(item => item.Stat).ToList();
                            order = false;
                        }
                        else
                        {
                            myItem = myItem.OrderByDescending(item => item.Stat).ToList();
                            order = true;
                        }
                    }
                    else
                    {
                        foreach (Item item in myItem)
                        {
                            item.Stat = -1;
                            foreach (ItemAbility itemAbility in item.ItemAbilitys)
                            {
                                if (itemAbility.Ability == Abilitys.방어력)
                                {
                                    item.Stat = itemAbility.Stat;
                                }
                            }
                        }
                        sort = type;
                        myItem = myItem.OrderByDescending(item => item.Stat).ToList();
                        order = true;
                    }
                    break;
                default:
                    break;
            }
            DisplayInventorySort();
        }

        /// <summary>인벤토리 장착 화면 출력</summary>
        static void DisplayEquipment(string msg)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("인벤토리 - 장착 관리");
            Console.ResetColor();

            Console.WriteLine("보유 중인 아이템을 관리할 수 있습니다.");
            Console.WriteLine();
            Console.WriteLine("[아이템 목록]");
            int itemNumber = 1;
            int itemOver = myItem.Count > 10 ? 1 : 0;
            foreach (Item item in myItem)
            {

                // 한글 한글자당 2자리를 차지하여 공백을 먼저 계산 해야 함
                // 일이삼사오육칠팔구십, 일B삼D오F칠H구J, ABCDEFGHIJ 등 다양한 경우에서 모두 동일한 공백을 갖도록 계산
                string str = item.ItemName;
                int padLen = 0;
                for (int i = 0; i < str.Length; i++)
                {
                    // 한글 한글자당 3바이트 씩 사용
                    if (Encoding.Default.GetBytes(str.Substring(i, 1)).Length == 3)
                        padLen++;
                }

                padLen = 20 - padLen;
                if (item.Equipment) Console.ForegroundColor = ConsoleColor.Green;
                //인벤토리의 아이템이 10개 이상 존재하면, itemNumber 여백 필요
                //itemOver 변수 사용
                string strItemNumber = itemNumber++ + "";
                Console.WriteLine($" - {strItemNumber.PadRight(1 + itemOver)} {(item.Equipment ? "[E]" : "   ")}| {str.PadRight(padLen)} | {item.ItemAbilitys[0].Ability} +{item.ItemAbilitys[0].Stat.ToString().PadRight(2)} | {item.Described}");
                if (item.ItemAbilitys.Count > 1)
                {
                    for (int i = 1; i < item.ItemAbilitys.Count; i++)
                    {
                        string whiteSpace = "";
                        Console.WriteLine($"{whiteSpace.PadRight(30 + itemOver, ' ')} | {item.ItemAbilitys[i].Ability} +{item.ItemAbilitys[i].Stat.ToString().PadRight(2)} |");
                    }
                }
                if (item.Equipment) Console.ResetColor();
            }
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("0. 나가기");
            Console.ResetColor();
            Console.WriteLine();
            Console.WriteLine("원하시는 행동을 입력해주세요.");
            Console.WriteLine(msg);
            int input = CheckValidInput(0, myItem.Count());
            switch (input)
            {
                case 0:
                    DisplayInventory();
                    break;
                default:
                    ChangeEquipment(input);
                    break;
            }
        }

        /// <summary>장비 장착 변경 메소드</summary>
        static void ChangeEquipment(int index)
        {
            string msg = "";
            --index;

            switch (myItem[index].Type)
            {
                case ItemTypes.무기:
                case ItemTypes.방어구:
                case ItemTypes.방패:
                    if (myItem[index].Equipment)
                    {
                        myItem[index].Equipment = false;
                        myEquipment[(int)myItem[index].Type] = 0;
                    }
                    else
                    {
                        if (myEquipment[(int)myItem[index].Type] == 0)
                        {
                            myItem[index].Equipment = true;
                            myEquipment[(int)myItem[index].Type] = myItem[index].ItemId;
                        }
                        else
                        {
                            //착용중인 아이템 무조건 변경
                            Console.WriteLine(myEquipment[(int)myItem[index].Type]);
                            myItem[myItem.FindIndex(i => i.ItemId == myEquipment[(int)myItem[index].Type])].Equipment = false;
                            myItem[index].Equipment = true;
                            myEquipment[(int)myItem[index].Type] = myItem[index].ItemId;

                            //착용중에 변경 불가 시
                            //msg = $"{myItem[index].Type} 아이템은 이미 착용중인 입니다.";
                        }
                    }
                    break;

                default:
                    msg = "착용이 불가능한 아이템 입니다.";
                    break;

            }
            AddStat();
            DisplayEquipment(msg);
        }

        /// <summary>상점 화면 출력</summary>
        static void DisplayShop()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("상점");
            Console.ResetColor();
            Console.WriteLine("필요한 아이템을 얻을 수 있는 상점입니다.");
            Console.WriteLine();
            Console.WriteLine("[보유 골드]");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write($"{player.Gold}");
            Console.ResetColor();
            Console.Write(" G\n");
            Console.WriteLine();
            Console.WriteLine("[아이템 목록]");
            foreach (Item item in shop)
            {

                // 한글 한글자당 2자리를 차지하여 공백을 먼저 계산 해야 함
                // 일이삼사오육칠팔구십, 일B삼D오F칠H구J, ABCDEFGHIJ 등 다양한 경우에서 모두 동일한 공백을 갖도록 계산
                string strName = item.ItemName;
                string strDescribed = item.Described;
                int namePadLen = 0;
                int describedPadLen = 0;
                for (int i = 0; i < strName.Length; i++)
                {
                    // 한글 한글자당 3바이트 씩 사용
                    if (Encoding.Default.GetBytes(strName.Substring(i, 1)).Length == 3)
                        namePadLen++;
                }
                for (int i = 0; i < strDescribed.Length; i++)
                {
                    // 한글 한글자당 3바이트 씩 사용
                    if (Encoding.Default.GetBytes(strDescribed.Substring(i, 1)).Length == 3)
                        describedPadLen++;
                }

                namePadLen = 20 - namePadLen;
                describedPadLen = 60 - describedPadLen;
                
                //구매완료 텍스트 색 변경
                if (myItem.FindIndex(i => i.ItemId.Equals(item.ItemId)) > -1) Console.ForegroundColor = ConsoleColor.DarkGray;

                Console.Write($" - {strName.PadRight(namePadLen)} | {item.ItemAbilitys[0].Ability} +{item.ItemAbilitys[0].Stat.ToString().PadRight(2)} | {strDescribed.PadRight(describedPadLen)} | ");
                Console.WriteLine($"{(myItem.FindIndex(i => i.ItemId.Equals(item.ItemId) ) > -1 ? "구매완료" :item.Price + " G")}");
                if (item.ItemAbilitys.Count > 1)
                {
                    for (int i = 1; i < item.ItemAbilitys.Count; i++)
                    {
                        string whiteSpace = "";
                        Console.WriteLine($"{whiteSpace.PadRight(23, ' ')} | {item.ItemAbilitys[i].Ability} +{item.ItemAbilitys[i].Stat.ToString().PadRight(2)} |");
                    }
                }
                if (myItem.FindIndex(i => i.ItemId.Equals(item.ItemId)) > -1) Console.ResetColor();

            }
            Console.WriteLine("1. 아이템 구매");
            Console.WriteLine("2. 아이템 판매");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("0. 나가기");
            Console.ResetColor();
            Console.WriteLine();
            Console.WriteLine("원하시는 행동을 입력해주세요.");

            int input = CheckValidInput(0, 2);
            switch (input)
            {
                case 1:
                    DisplayPurchase("");
                    break;
                case 2:
                    DisplaySale();
                    break;
                case 0:
                    DisplayGameIntro();
                    break;
            }
        }

        /// <summary>상점 구매 화면 출력</summary>
        static void DisplayPurchase(string msg)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("상점 - 구매");
            Console.ResetColor();
            Console.WriteLine("필요한 아이템을 얻을 수 있는 상점입니다.");
            Console.WriteLine();
            Console.WriteLine("[보유 골드]");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write($"{player.Gold}");
            Console.ResetColor();
            Console.Write(" G\n");
            Console.WriteLine();
            Console.WriteLine("[아이템 목록]");
            int itemNumber = 1;
            int itemOver = shop.Count > 10 ? 1 : 0;
            foreach (Item item in shop)
            {

                // 한글 한글자당 2자리를 차지하여 공백을 먼저 계산 해야 함
                // 일이삼사오육칠팔구십, 일B삼D오F칠H구J, ABCDEFGHIJ 등 다양한 경우에서 모두 동일한 공백을 갖도록 계산
                string strName = item.ItemName;
                string strDescribed = item.Described;
                int namePadLen = 0;
                int describedPadLen = 0;
                for (int i = 0; i < strName.Length; i++)
                {
                    // 한글 한글자당 3바이트 씩 사용
                    if (Encoding.Default.GetBytes(strName.Substring(i, 1)).Length == 3)
                        namePadLen++;
                }
                for (int i = 0; i < strDescribed.Length; i++)
                {
                    // 한글 한글자당 3바이트 씩 사용
                    if (Encoding.Default.GetBytes(strDescribed.Substring(i, 1)).Length == 3)
                        describedPadLen++;
                }

                namePadLen = 20 - namePadLen;
                describedPadLen = 60 - describedPadLen;

                //인벤토리의 아이템이 10개 이상 존재하면, itemNumber 여백 필요
                //itemOver 변수 사용
                string strItemNumber = itemNumber++ + "";
                
                //구매완료 텍스트 색 변경
                if(myItem.FindIndex(i => i.ItemId.Equals(item.ItemId)) > -1) Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write($" - {strItemNumber.PadRight(1 + itemOver)} {strName.PadRight(namePadLen)} | {item.ItemAbilitys[0].Ability} +{item.ItemAbilitys[0].Stat.ToString().PadRight(2)} | {strDescribed.PadRight(describedPadLen)} | ");
                Console.WriteLine($"{(myItem.FindIndex(i => i.ItemId.Equals(item.ItemId)) > -1 ? "구매완료" : item.Price + " G")}");
                if (item.ItemAbilitys.Count > 1)
                {
                    for (int i = 1; i < item.ItemAbilitys.Count; i++)
                    {
                        string whiteSpace = "";
                        Console.WriteLine($"{whiteSpace.PadRight(25, ' ')} | {item.ItemAbilitys[i].Ability} +{item.ItemAbilitys[i].Stat.ToString().PadRight(2)} |");
                    }
                }
                if (myItem.FindIndex(i => i.ItemId.Equals(item.ItemId)) > -1) Console.ResetColor();
            }
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("0. 나가기");
            Console.ResetColor();
            Console.WriteLine();
            Console.WriteLine("원하시는 행동을 입력해주세요.");
            Console.WriteLine(msg);
            int input = CheckValidInput(0, shop.Count());
            switch (input)
            {
                case 0:
                    DisplayShop();
                    break;
                default:
                    Purchase(input);
                    break;
            }
        }

        static void Purchase(int index)
        {
            --index;
            string msg = "";
            if(myItem.FindIndex(i => i.ItemId.Equals(shop[index].ItemId)) > -1)
            {
                msg = "이미 구매한 아이템입니다.";
            }
            else
            {
                if(player.Gold < shop[index].Price)
                {
                    msg = "Gold 가 부족합니다.";
                }
                else
                {
                    player.Gold -= shop[index].Price;
                    myItem.Add(shop[index]);
                    msg = "구매를 완료했습니다.";

                }
            }
            DisplayPurchase(msg);
        }


        /// <summary>상점 판매 화면 출력</summary>
        static void DisplaySale()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("상점 - 판매");
            Console.ResetColor();
            Console.WriteLine("필요한 아이템을 얻을 수 있는 상점입니다.");
            Console.WriteLine();
            Console.WriteLine("[보유 골드]");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write($"{player.Gold}");
            Console.ResetColor();
            Console.Write(" G\n");
            Console.WriteLine();
            Console.WriteLine("[아이템 목록]");
            int itemNumber = 1;
            int itemOver = myItem.Count > 10 ? 1 : 0;
            foreach (Item item in myItem)
            {

                // 한글 한글자당 2자리를 차지하여 공백을 먼저 계산 해야 함
                // 일이삼사오육칠팔구십, 일B삼D오F칠H구J, ABCDEFGHIJ 등 다양한 경우에서 모두 동일한 공백을 갖도록 계산
                string strName = item.ItemName;
                string strDescribed = item.Described;
                int namePadLen = 0;
                int describedPadLen = 0;
                for (int i = 0; i < strName.Length; i++)
                {
                    // 한글 한글자당 3바이트 씩 사용
                    if (Encoding.Default.GetBytes(strName.Substring(i, 1)).Length == 3)
                        namePadLen++;
                }
                for (int i = 0; i < strDescribed.Length; i++)
                {
                    // 한글 한글자당 3바이트 씩 사용
                    if (Encoding.Default.GetBytes(strDescribed.Substring(i, 1)).Length == 3)
                        describedPadLen++;
                }

                namePadLen = 20 - namePadLen;
                describedPadLen = 60 - describedPadLen;

                //인벤토리의 아이템이 10개 이상 존재하면, itemNumber 여백 필요
                //itemOver 변수 사용
                string strItemNumber = itemNumber++ + "";

                Console.Write($" - {strItemNumber.PadRight(1 + itemOver)} {strName.PadRight(namePadLen)} | {item.ItemAbilitys[0].Ability} +{item.ItemAbilitys[0].Stat.ToString().PadRight(2)} | {strDescribed.PadRight(describedPadLen)} | ");

                //판매 금액 출력 
                //아이템의 가격의 85%
                Console.WriteLine($"{(int)(shop[shop.FindIndex(i => i.ItemId == item.ItemId)].Price * 0.85f)} G");
                if (item.ItemAbilitys.Count > 1)
                {
                    for (int i = 1; i < item.ItemAbilitys.Count; i++)
                    {
                        string whiteSpace = "";
                        Console.WriteLine($"{whiteSpace.PadRight(25, ' ')} | {item.ItemAbilitys[i].Ability} +{item.ItemAbilitys[i].Stat.ToString().PadRight(2)} |");
                    }
                }

            }
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("0. 나가기");
            Console.ResetColor();
            Console.WriteLine();
            Console.WriteLine("원하시는 행동을 입력해주세요.");
            int input = CheckValidInput(0, myItem.Count());
            switch (input)
            {
                case 0:
                    DisplayShop();
                    break;
                default:
                    Sale(input);
                    break;
            }
        }

        /// <summary>아이템 판매 메소드</summary>
        static void Sale(int index)
        {
            --index;
            if (myItem[index].Equipment) myEquipment[(int)myItem[index].Type] = 0;
                
            player.Gold += (int)(shop[shop.FindIndex(i => i.ItemId == myItem[index].ItemId)].Price * 0.85f);
            myItem.RemoveAt(index);
            AddStat();
            DisplaySale();
        }

        /// <summary>던전 정보 화면 출력</summary>
        static void DisplayDungeon()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("던전입장");
            Console.ResetColor();

            Console.WriteLine("이곳에서 던전으로 들어가기전 활동을 할 수 있습니다.");
            Console.WriteLine();
            Console.WriteLine("1. 쉬운 던전   | 방어력 5이상 권장");
            Console.WriteLine("2. 일반 던전   | 방어력 11이상 권장");
            Console.WriteLine("3. 어려운 던전 | 방어력 17이상 권장");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("0. 나가기");
            Console.ResetColor();
            Console.WriteLine();
            Console.WriteLine("원하시는 행동을 입력해주세요.");
            int input = CheckValidInput(0, 3);
            switch (input)
            {
                case 0:
                    DisplayGameIntro();
                    break;
                default:
                    DisplayDungeonResult(input);
                    break;
            }
        }

        static void DisplayDungeonResult(int level)
        {
            Console.Clear();
            Random rand = new Random();
            bool clear = true;
            bool die = false;
            int minDef = 0;
            int basicGold = 0;

            switch (level)
            {
                case 1:
                    minDef = 5;
                    basicGold = 1000;
                    break;
                case 2:
                    minDef = 11;
                    basicGold = 1700;
                    break;
                case 3:
                    minDef = 17;
                    basicGold = 2500;
                    break;
            }

            // 권장 방어력 미만인 경우 40%로 던전 실패 로직
            if (minDef > (player.Def + myAddStat[(int)Abilitys.방어력]))
            {       
                // 0 : 실패, 1 : 성공
                int ran = rand.Next(0, 101);
                float[] probs = { 60.0f, 40.0f };

                float cumulative = 0f;
                int target = -1;
                for (int i = 0; i < 2; i++)
                {
                    cumulative += probs[i];
                    if (ran <= cumulative)
                    {
                        target = 1 - i;
                        break;
                    }
                }
                clear = target > 0 ? true : false; 
            }

            // 클리어 체력 감소 20~35 사이 - 내방어력 + 권장방어력
            int subHp = rand.Next(20, 36) - (player.Def + myAddStat[(int)Abilitys.방어력]) + minDef;
            // 내방어력이 너무 높으면 체력 회복 되는걸 방지
            subHp = subHp < 0 ? 0 : subHp;

            // 클리어 보상 처리
            if (clear)
            {
                player.Hp -= subHp;
                float addRate = (rand.Next(player.Atk + myAddStat[(int)Abilitys.공격력], (player.Atk + myAddStat[(int)Abilitys.공격력]) * 2 + 1) / 100.0f);
                basicGold = (int)(basicGold + basicGold * addRate);
            }
            else
            {
                // 던전 클리어 실패시 체력 감소 절반
                player.Hp -= subHp / 2;
            }

            // 사망 체크
            die = player.Hp > 0 ? false : true;

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("던전");

            if (die)
            {
                Console.WriteLine(" - 사망");
            }
            else if (!die && !clear)
            {
                Console.WriteLine(" - 실패");
            } 
            else if (!die && clear)
            {
                Console.WriteLine(" - 클리어");
            }
            Console.ResetColor();
            
            if (die)
            {
                Console.WriteLine();
                Console.WriteLine("캐릭터가 사망하였습니다.");
                Console.WriteLine("모든 정보가 초기화 됩니다.");
                GameDataSetting(1);
            }
            else if (!die && !clear)
            {
                Console.WriteLine();
                Console.WriteLine("[체력 감소]");
                Console.WriteLine($"체력 {player.Hp + subHp / 2} -> {player.Hp}");

            }
            else if (!die && clear)
            {
                Console.WriteLine("축하합니다!!");
                if (level == 1) Console.Write("쉬운 던전을");
                if (level == 2) Console.Write("일반 던전을");
                if (level == 3) Console.Write("어려운 던전을");
                Console.WriteLine("클리어 하였습니다.");
                Console.WriteLine();

                Console.WriteLine("[탐험 결과]");
                Console.WriteLine($"체력 {player.Hp + subHp} -> {player.Hp}");
                Console.WriteLine($"Gold {player.Gold } G -> {player.Gold + basicGold} G");
                player.Gold += basicGold;
                
                //경험치 증가
                player.Exp++;
                if (player.Level < 5)
                {
                    // 경험치 조건 충족시 레업
                    if(player.Exp >= maxExp[player.Level])
                    {

                        player.Level++;
                        player.Exp = 0;

                        float addAtl = 0.5f * (player.Level - 1);
                        int addDef = (int)1 * (player.Level - 1);

                        Console.WriteLine();
                        Console.WriteLine("[레벨 업]");
                        Console.WriteLine($"Level {player.Level - 1} -> {player.Level}");
                        Console.WriteLine($"공격력 {player.Atk} -> {10 + (int)addAtl}");
                        Console.WriteLine($"방어력 {player.Def} -> {5+addDef}");

                        player.Atk = 10 + (int)addAtl;
                        player.Def = 5 + addDef;
                }
                }
            }

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("0. 나가기");
            Console.ResetColor();
            Console.WriteLine();
            Console.WriteLine("원하시는 행동을 입력해주세요.");
            int input = CheckValidInput(0, 0);
            switch (input)
            {
                case 0:
                    DisplayDungeon();
                    break;
            }
        }


        /// <summary>휴식 화면 출력</summary>
        static void DisplayRest(string msg)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("휴식하기");
            Console.ResetColor();

            Console.Write("500 G 를 내면 체력을 회복할 수 있습니다. (보유 골드 : ");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write($"{player.Gold}");
            Console.ResetColor();
            Console.Write(" G)");

            Console.WriteLine();
            Console.WriteLine("1. 휴식하기");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("0. 나가기");
            Console.ResetColor();
            Console.WriteLine();
            Console.WriteLine("원하시는 행동을 입력해주세요.");
            Console.WriteLine(msg);
            int input = CheckValidInput(0, 1);
            switch (input)
            {
                case 0:
                    DisplayGameIntro();
                    break;
                case 1:
                    Rest();
                    break;
            }
        }

        /// <summary>휴식 메소드</summary>
        static void Rest()
        {
            string msg;
            if (player.Gold < 500)
            {
                msg = "Gold 가 부족합니다.";
            }
            else
            {
                msg = "체력이 회복되었습니다.";
                player.Gold -= 500;
                player.Hp = 100;
            }
            DisplayRest(msg);
        }

        /// <summary>정보 저장 메소드</summary>
        static void Save()
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load($"{savePath}/data.xml");


            XmlNode playerNode = xmlDoc.SelectSingleNode("Data/PlayerData");

            // 캐릭터 정보 저장
            playerNode["Name"].InnerText = player.Name;
            playerNode["Job"].InnerText = player.Job;
            playerNode["Level"].InnerText = player.Level.ToString();
            playerNode["Atk"].InnerText = player.Atk.ToString();
            playerNode["Def"].InnerText = player.Def.ToString();
            playerNode["Hp"].InnerText = player.Hp.ToString();
            playerNode["Gold"].InnerText = player.Gold.ToString();
            playerNode["Exp"].InnerText = player.Exp.ToString();

            // 기존 아이템 정보 삭제
            playerNode.RemoveChild(playerNode["MyItems"]);

            // 아이템 정보 저장
            XmlNode myItems = xmlDoc.CreateElement("MyItems");
            playerNode.AppendChild(myItems);
            foreach (Item item in myItem)
            {
                XmlNode myItem = xmlDoc.CreateElement("MyItem");

                XmlNode itemId = xmlDoc.CreateElement("ItemId");
                XmlNode equipment = xmlDoc.CreateElement("Equipment");
                XmlNode itemName = xmlDoc.CreateElement("ItemName");
                XmlNode itemType = xmlDoc.CreateElement("ItemType");
                XmlNode described = xmlDoc.CreateElement("Described");

                itemId.InnerText = item.ItemId.ToString();
                equipment.InnerText = item.Equipment.ToString();
                itemName.InnerText = item.ItemName;
                itemType.InnerText = ((int)item.Type).ToString();
                described.InnerText = item.Described;

                XmlNode itemAbilitys = xmlDoc.CreateElement("ItemAbilitys");
                foreach (ItemAbility itemAbilityItem in item.ItemAbilitys)
                {
                    XmlNode itemAbility = xmlDoc.CreateElement("ItemAbility");
                    XmlNode ability = xmlDoc.CreateElement("Ability");
                    XmlNode stat = xmlDoc.CreateElement("Stat");

                    ability.InnerText = ((int)itemAbilityItem.Ability).ToString();
                    stat.InnerText = itemAbilityItem.Stat.ToString();

                    itemAbility.AppendChild(ability);
                    itemAbility.AppendChild(stat);
                    itemAbilitys.AppendChild(itemAbility);
                }

                myItem.AppendChild(itemId);
                myItem.AppendChild(equipment);
                myItem.AppendChild(itemName);
                myItem.AppendChild(itemType);
                myItem.AppendChild(itemAbilitys);
                myItem.AppendChild(described);

                myItems.AppendChild(myItem);
            }

            xmlDoc.Save($"{savePath}/data.xml");
        }

        /// <summary>입력 검증 메소드</summary>
        static int CheckValidInput(int min, int max)
        {
            while (true)
            {
                string input = Console.ReadLine();

                bool parseSuccess = int.TryParse(input, out var ret);
                if (parseSuccess)
                {
                    if (ret >= min && ret <= max)
                    {
                        if (ret == 0) Save();
                        return ret;
                    }

                }

                Console.WriteLine("잘못된 입력입니다.");
            }
        }

        public class Character
        {
            public string Name { get; }
            public string Job { get; }
            public int Level { get; set; }
            public int Atk { get; set; }
            public int Def { get; set; }
            public int Hp { get; set; }
            public int Gold { get; set; }
            public int Exp { get; set; }
            public int[] AddStat { get; set; }

            public Character(string name, string job, int level, int atk, int def, int hp, int gold, int exp)
            {
                Name = name;
                Job = job;
                Level = level;
                Atk = atk;
                Def = def;
                Hp = hp;
                Gold = gold;
                Exp = exp;
            }
        }

        public class Item
        {

            public int ItemId { get; }
            public bool Equipment { get; set; }
            public string ItemName { get; }
            public ItemTypes Type { get; }
            public List<ItemAbility> ItemAbilitys { get; }
            public string Described { get; }
            public int Stat { get; set; }
            public int Price { get; set; }

            public Item(int itemId, bool equipment, string itemName, int type, List<ItemAbility> itemAbilitys, string described)
            {
                ItemId = itemId;
                Equipment = equipment;
                ItemName = itemName.Length > 10 ? itemName.Substring(0, 10) : itemName;
                Type = (ItemTypes)type;
                ItemAbilitys = itemAbilitys;
                Described = described.Length > 30 ? described.Substring(0, 30) : described;
            }

            public Item(int itemId, string itemName, int type, List<ItemAbility> itemAbilitys, string described, int price)
            {
                ItemId = itemId;
                ItemName = itemName.Length > 10 ? itemName.Substring(0, 10) : itemName;
                Type = (ItemTypes)type;
                ItemAbilitys = itemAbilitys;
                Described = described.Length > 30 ? described.Substring(0, 30) : described;
                Price = price;
            }
        }

        public class ItemAbility
        {

            public Abilitys Ability { get; }
            public int Stat { get; }

            public ItemAbility(int ability, int stat)
            {
                Ability = (Abilitys)ability;
                Stat = stat;
            }
        }
    }
}