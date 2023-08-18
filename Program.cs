using System.Text;

namespace spartanDungeon
{
    internal class Program
    {
        private static Character player;
        private static List<Item> myItem = new List<Item>();

        private enum Equipments { 무기, 방어구 };
        private static Equipments equipment;
        private static int[] myEquipment = new int[2];


        static void Main(string[] args)
        {
            GameDataSetting();
            DisplayGameIntro();
        }

        static void GameDataSetting()
        {
            // 캐릭터 정보 세팅
            player = new Character("Chad", "전사", 1, 10, 5, 100, 1500);

            // 아이템 정보 세팅
            List<ItemAbility> itemAbilities1 = new List<ItemAbility>();
            List<ItemAbility> itemAbilities2 = new List<ItemAbility>();

            itemAbilities1.Add(new ItemAbility(1, 5));
            itemAbilities2.Add(new ItemAbility(0, 2));
            itemAbilities2.Add(new ItemAbility(1, 2));
            myItem.Add(new Item(1, true, "무쇠갑옷", 1, itemAbilities1, "무쇠로 만들어져 튼튼한 갑옷입니다."));
            myItem.Add(new Item(2, false, "낡은 검", 0, itemAbilities2, "쉽게 볼 수 있는 낡은 검 입니다."));
            myItem.Add(new Item(3, false, "낡은 검", 0, itemAbilities2, "쉽게 볼 수 있는 낡은 검 입니다."));
        }

        static void DisplayGameIntro()
        {
            Console.Clear();

            Console.WriteLine("스파르타 마을에 오신 여러분 환영합니다.");
            Console.WriteLine("이곳에서 전전으로 들어가기 전 활동을 할 수 있습니다.");
            Console.WriteLine();
            Console.WriteLine("1. 상태보기");
            Console.WriteLine("2. 인벤토리");
            Console.WriteLine();
            Console.WriteLine("원하시는 행동을 입력해주세요.");

            int input = CheckValidInput(1, 2);
            switch (input)
            {
                case 1:
                    DisplayMyInfo();
                    break;

                case 2:
                    DisplayInventory();
                    break;
            }
        }

        static void DisplayMyInfo()
        {
            Console.Clear();

            Console.WriteLine("상태보기");
            Console.WriteLine("캐릭터의 정보르 표시합니다.");
            Console.WriteLine();
            Console.WriteLine($"Lv.{player.Level}");
            Console.WriteLine($"{player.Name}({player.Job})");
            Console.WriteLine($"공격력 :{player.Atk}");
            Console.WriteLine($"방어력 : {player.Def}");
            Console.WriteLine($"체력 : {player.Hp}");
            Console.WriteLine($"Gold : {player.Gold} G");
            Console.WriteLine();
            Console.WriteLine("0. 나가기");
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

        static void DisplayInventory()
        {
            Console.Clear();

            Console.WriteLine("인벤토리");
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
                    if ( Encoding.Default.GetBytes(str.Substring(i,1)).Length == 3 )
                        padLen++;
                }

                padLen = 20 - padLen;

                Console.WriteLine($" - {(item.Equipment ? "[E]" : "   ")}| {str.PadRight(padLen)} | {item.ItemAbilitys[0].Ability} + {item.ItemAbilitys[0].Stat} | {item.Described}");
                if (item.ItemAbilitys.Count > 1)
                {
                    for (int i = 1; i<item.ItemAbilitys.Count; i++)
                    {
                        string whiteSpace = "";
                        Console.WriteLine($"{whiteSpace.PadRight(28,' ')} | {item.ItemAbilitys[i].Ability} + {item.ItemAbilitys[i].Stat} |");
                    }
                }
            }
            Console.WriteLine();
            Console.WriteLine("1. 장착 관리");
            Console.WriteLine("0. 나가기");
            Console.WriteLine();
            Console.WriteLine("원하시는 행동을 입력해주세요.");

            int input = CheckValidInput(0, 1);
            switch (input)
            {
                case 1:
                    DisplayEquipment("");
                    break;
                case 0:
                    DisplayGameIntro();
                    break;
            }
        }

        static void DisplayEquipment(string msg)
        {
            Console.Clear();

            Console.WriteLine("인벤토리 - 장착 관리");
            Console.WriteLine("보유 중인 아이템을 관리할 수 있습니다.");
            Console.WriteLine();
            Console.WriteLine("[아이템 목록]");
            int itemNumber = 1;
            int itemOver = myItem.Count > 10 ? 1 :0;
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

                //인벤토리의 아이템이 10개 이상 존재하면, itemNumber 여백 필요
                //itemOver 변수 사용
                string strItemNumber = itemNumber + "";
                Console.WriteLine($" - {strItemNumber.PadRight(1+itemOver)} {(item.Equipment ? "[E]" : "   ")}| {str.PadRight(padLen)} | {item.ItemAbilitys[0].Ability} + {item.ItemAbilitys[0].Stat} | {item.Described}");
                if (item.ItemAbilitys.Count > 1)
                {
                    for (int i = 1; i < item.ItemAbilitys.Count; i++)
                    {
                        string whiteSpace = "";
                        Console.WriteLine($"{whiteSpace.PadRight(30+itemOver, ' ')} | {item.ItemAbilitys[i].Ability} + {item.ItemAbilitys[i].Stat} |");
                    }
                }

                itemNumber++;
            }
            Console.WriteLine();
            Console.WriteLine("0. 나가기");
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

        static void ChangeEquipment(int index)
        {
            string msg = "";
            if(myItem[--index].Equipment)
            {
                myItem[index].Equipment = false;
                myEquipment[myItem[index].Type] = 0;
            }
            else
            {
                if(myEquipment[myItem[index].Type] == 0)
                {
                    myItem[index].Equipment = true;
                    myEquipment[myItem[index].Type] = myItem[index].ItemId;
                }
                else
                {
                    msg ="해당 타입의 장비는 이미 착용 중 입니다.";
                }
            }

            DisplayEquipment(msg);
        }

        static int CheckValidInput(int min, int max)
        {
            while (true)
            {
                string input = Console.ReadLine();

                bool parseSuccess = int.TryParse(input, out var ret);
                if (parseSuccess)
                {
                    if (ret >= min && ret <= max)
                        return ret;
                }

                Console.WriteLine("잘못된 입력입니다.");
            }
        }
    }


    public class Character
    {
        public string Name { get; }
        public string Job { get; }
        public int Level { get; }
        public int Atk { get; }
        public int Def { get; }
        public int Hp { get; }
        public int Gold { get; }

        public Character(string name, string job, int level, int atk, int def, int hp, int gold)
        {
            Name = name;
            Job = job;
            Level = level;
            Atk = atk;
            Def = def;
            Hp = hp;
            Gold = gold;
        }
    }

    public class Item
    {

        public int ItemId { get; }
        public bool Equipment { get; set; }
        public string ItemName { get; }
        public int Type { get; } // 0 : 무기, 1 : 방어구
        public List<ItemAbility> ItemAbilitys { get; }
        public string Described { get; }

        public Item(int itemId, bool equipment, string itemName, int type, List<ItemAbility> itemAbilitys, string described)
        {
            ItemId = itemId;
            Equipment = equipment;
            if(itemName.Length > 10)
            {
                ItemName = itemName.Substring(0, 10);
            }
            else
            {
                ItemName = itemName;
            }
            Type = type;
            ItemAbilitys = itemAbilitys;
            Described = described;
        }
    }

    public class ItemAbility
    {
        public enum Abilitys { 공격력, 방어력 };
        public Abilitys Ability { get; }
        public int Stat { get; }

        public ItemAbility(int ability, int stat)
        {
            Ability = (Abilitys)ability;
            Stat = stat;
        }
    }

}