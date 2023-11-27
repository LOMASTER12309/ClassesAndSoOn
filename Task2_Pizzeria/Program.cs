using System;
using System.Collections.Generic;
using System.IO;

namespace Task2_Pizzeria
{
    public class Vector
    {
        float x;
        float y;
        public float X => x;
        public float Y => y;
        public Vector()
        {
            x = 0;
            y = 0;
        }
        public Vector(float x, float y)
        {
            this.x = x;
            this.y = y;
        }
        public static int dist(Vector v1, Vector v2, float speed) //возращает расстояние в тактах времени с данной скоростью
        {
            return (int)Math.Ceiling((Math.Sqrt(Math.Pow(v2.X - v1.X, 2) + Math.Pow(v2.Y - v1.Y, 2)))/speed);
        }
    }
    public class Order
    {
        Vector destination = null; //пункт назначения
        string id = "000"; //номер заказа
        int time = 0; //время поступления заказа
        int DeliveryTime = 0; //время доставки
        Baker maker = null;
        public int TimeOfReceipt => time;
        public int WaitingTime
        {
            get
            {
                if (DeliveryTime - time > 0)
                    return (DeliveryTime - time);
                else return 0;
            }
        }

        public string ID => id;
        public Vector Location => destination;
        public Order(string count_id, int T, Vector v)
        {
            id = count_id;
            time = T;
            destination = v;
        }

        public Baker Maker
        {
            get
            {
                return maker;
            }
            set
            {
                maker = (Baker)value;
            }
        }
        public float urgency(int T) //срочность заказа как произведение дальности на квадрат ожидания
        {
            return (float)Math.Pow((T-time),2)*Vector.dist(destination, new Vector(0, 0), 1);
        }
        public float proximity(int T, Vector v) //близость заказа как деление ожидания на дальность от курьера
        {
            return (float)((T-time)/Math.Sqrt(Math.Pow(destination.X - v.X, 2) + Math.Pow(destination.Y - v.Y, 2)));
        }
        public void HandOverTheOrder(int T)
        {
            DeliveryTime = T;
        }
    }
    public class Сourier
    {
        public static List<Сourier> Couriers = new List<Сourier>(); //все курьеры
        string id = "00";
        int V = 0; //вместимость багажника
        float speed = 0;
        Queue<Order> route;//маршрут заказов
        int time = 0; //оставшееся время пути до следущего пункта
        int numOfDest = 0;
        int NumOfOrdDeliv = 0; //количество доставленных заказов

        public int Load => numOfDest - 1;
        public string ID => id;
        public float Speed => speed;
        public int MaxLoad => V;
        public Queue<Order> Route => route;
        public int NumOfOrdersDeliveered => NumOfOrdDeliv;
        public Сourier()
        {
            V = 3;
            speed = 10;
            route = new Queue<Order>();
        }
        public Сourier(string ID, int size, float sp): this()
        {
            id = ID;
            if (size > 0) V = size;
            if (sp > 0) speed = sp;
            route = new Queue<Order>(V);
        }
        public void AddOrder(Order ord)
        {
            if (numOfDest == 0) numOfDest++;
            route.Enqueue(ord);
            numOfDest++;
        }
        public void start()
        {
            if (route.Count == 0) return;
            time = Vector.dist(new Vector(0, 0), route.Peek().Location, speed);
            //Console.WriteLine($"Заказ {route.Peek().ID} будет идти {time} тактов");
        }
        public static void DistributeOrders(List<Order> orders, List<Сourier> FreeCouriers, int T) //распределить заказы
        {
            //определяем быстрейшего курьера, отдаём ему самый срочный заказ и ближайшие к нему, далее повторяем процесс
            while ((orders.Count > 0) && (FreeCouriers.Count > 0)) //пока не закончатся заказы или курьеры
            {
                int fastest = 0; //быстрейший курьер
                for (int i = 1; i < FreeCouriers.Count; i++)
                    if (FreeCouriers[fastest].Speed < FreeCouriers[i].Speed) fastest = i;
                Сourier curCourier = FreeCouriers[fastest];
                FreeCouriers.RemoveAt(fastest);
                int mostUrgentOrder = 0; // "срочнейший" заказ
                //Console.WriteLine($"Срочность заказа {orders[0].ID} составляет {orders[0].urgency(T)}");
                for (int i = 1; i < orders.Count; i++) 
                {
                    //Console.WriteLine($"Срочность заказа {orders[i].ID} составляет {orders[i].urgency(T)}");
                    if (orders[mostUrgentOrder].urgency(T) < orders[i].urgency(T)) mostUrgentOrder = i;
                }
                Order curOrder = orders[mostUrgentOrder]; //текущий обрабатываемый заказ
                curCourier.AddOrder(curOrder);
                Console.WriteLine($"Заказ {curOrder.ID} забран курьером {curCourier.ID}");
                orders.RemoveAt(mostUrgentOrder);
                while ((orders.Count > 0) && (curCourier.MaxLoad - curCourier.Load) > 0) //добавляем курьеру заказов
                {
                    int nearest = 0; //"ближайший" к предыдущему
                    for (int i = 1; i < orders.Count; i++)
                        if (orders[nearest].proximity(T, curOrder.Location) < orders[i].proximity(T, curOrder.Location))
                            nearest = i;
                    curOrder = orders[nearest];
                    curCourier.AddOrder(curOrder); //добавили заказ в список
                    Console.WriteLine($"Заказ {curOrder.ID} забран курьером {curCourier.ID}");
                    orders.RemoveAt(nearest);
                }
                curCourier.start();
            }
        }

        public void MakeMove(List<Сourier> FreeCouriers, List<Order> CompletedOrders, int T)
        {
            int n = route.Count;
            if (time == 0) return;
            else time--;
            if (time == 0)
            {
                if (n == 0)
                {
                    numOfDest--;
                    Console.WriteLine($"Курьер {id} вернулся");
                    FreeCouriers.Add(this);
                    return;
                }
                while (time == 0) //сдаем все заказы по данным координатам, если их несколько 
                {
                    numOfDest--;
                    NumOfOrdDeliv++;
                    Order curOrder = route.Dequeue();
                    curOrder.HandOverTheOrder(T);
                    CompletedOrders.Add(curOrder);
                    Console.WriteLine($"Заказ {curOrder.ID} доставлен. Время исполнения {curOrder.WaitingTime}");
                    if (route.Count == 0) //если заказы закончились, отправляемся на базу
                    {
                        time = Vector.dist(curOrder.Location, new Vector(0, 0), speed);
                        //Console.WriteLine($"Курьер {id} вернётся через {time} тактов");
                    }
                    else
                    {
                        time = Vector.dist(curOrder.Location, route.Peek().Location, speed);
                        Console.WriteLine($"Курьер доставит заказ {route.Peek().ID} через {time} тактов");
                    }
                }
            }
        }
    }

    public class Baker
    {
        public static List<Baker> Bakers = new List<Baker>(); //все пекари
        public static Queue<Baker> WarehouseQueue = new Queue<Baker>();
        string id = "00";
        int efficiency = 0;
        int NumOfCompOrd = 0; //количество приготовленных заказов за день
        Order curOrder = null;
        int time = 0; //оставшееся время до готовности
        public string ID => id;
        public int Speed => efficiency;
        public int NumOfCompOrders => NumOfCompOrd;
        public Baker(string ID, int eff)
        {
            id = ID;
            efficiency = eff;
        }

        public Order CurrentOrder => curOrder;
        void GetOrder(Order order)
        {
            curOrder = order;
            curOrder.Maker = this;
            Console.WriteLine($"Пекарь {this.ID} взял заказ {order.ID}");
            time = efficiency;
        }
        public static void DistributeOrders(Queue<Order> orders, List<Baker> FreeBakers, ref int MaxQueue) //распределение заказов между пекарями
        {
            while ((orders.Count > 0) && (FreeBakers.Count > 0))
            {
                int theBest = 0; //сначала заказы берут самые опытные и быстрые пекари
                for (int i = 1; i < FreeBakers.Count; i++)
                    if (FreeBakers[theBest].Speed < FreeBakers[i].Speed) theBest = i;
                Baker curBaker = FreeBakers[theBest];
                Order curOrder = orders.Dequeue();
                curBaker.GetOrder(curOrder);
                FreeBakers.RemoveAt(theBest);
            }
            int queue = orders.Count;
            if (queue > MaxQueue) MaxQueue = queue;
        }

        public void MakeMove()
        {
            if (time > 0) time--;
            else return;
            if (time == 0)
            {
                Console.WriteLine($"Заказ {curOrder.ID} приготовлен. Пекарь {this.ID} ждёт места на складе");
                NumOfCompOrd++;
                WarehouseQueue.Enqueue(this);
            }
        }
        public static void UploadToWarehouse(List<Order> orders, int V, List<Baker> FreeBakers, ref int MaxQueue) //выгрузка заказов на склад, V - объем склада
        {
            while ((orders.Count < V) && (WarehouseQueue.Count > 0)) //пока есть место на складе и не пуста очередь на склад
            {
                Baker curBaker = WarehouseQueue.Dequeue();
                orders.Add(curBaker.curOrder);
                Console.WriteLine($"Заказ {curBaker.curOrder.ID} помещён на склад");
                curBaker.curOrder = null;
                FreeBakers.Add(curBaker);
            }
            int queue = WarehouseQueue.Count;
            if (queue > MaxQueue) MaxQueue = queue;
        }
    }

    class Program
    {
        static string CreateID(int id, int k)
        {
            string str = Convert.ToString(id);
            if (k == 0) while (str.Length < 3) str = "0" + str;
            else if (k > 0)
            {
                while (str.Length < 2) str = "0" + str;
                if (k == 1) str = "B" + str;
                else str = "K" + str;
            }
            return str;
        }
        static int N = 0; //пекари
        static int M = 0; //курьеры
        static int T = 0; //вместимость склада
        static int Count = 0; //количество сегодняшних заказов
        static int LimitTime = 0; //Время доставки, иначе пиццу отдаём бесплатно
        static Queue<Order> TodayOrders; 
        static Queue<Order> OrdersInQueue;
        static List<Order> OrdersInStock; //заказы на складе
        static List<Order> CompletedOrders;
        static List<Baker> FreeBakers;
        static List<Сourier> FreeСouriers;

        static void InputData(string FileName)
        {
            string[] lines = File.ReadAllLines(FileName);
            string[] bakers = lines[0].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries); //времена изготовления пиццы
            string[] speeds = lines[1].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries); //скорости
            string[] trunks = lines[2].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries); //багажники
            string[] stock = lines[3].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries); //склад
            string[] limit = lines[4].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            T = int.Parse(stock[1]);
            LimitTime = int.Parse(limit[1]);
            M = Math.Min(speeds.Length, trunks.Length);
            N = bakers.Length;
            FreeBakers = new List<Baker>(N);
            for (int i = 1; i < N; i++)
            {
                Baker.Bakers.Add(new Baker(CreateID(i, 1), int.Parse(bakers[i])));
                FreeBakers.Add(Baker.Bakers[i-1]);
            }
            FreeСouriers = new List<Сourier>(M);
            for (int i = 1; i < M; i++)
            {
                Сourier.Couriers.Add(new Сourier(CreateID(i, 2), int.Parse(trunks[i]), float.Parse(speeds[i])));
                FreeСouriers.Add(Сourier.Couriers[i-1]);
            }
            Count = lines.Length - 6;
            TodayOrders = new Queue<Order>(Count);
            CompletedOrders = new List<Order>(Count);
            for (int i = 6; i < lines.Length; i++)
            {
                string[] order = lines[i].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                TodayOrders.Enqueue(new Order(CreateID(i-5, 0), int.Parse(order[0]), new Vector(float.Parse(order[1]), float.Parse(order[2]))));
            }
        }
        static void Main(string[] args)
        {
            InputData("test2.txt");
            Console.WriteLine("Мы имеем: ");
            for (int i =0; i < FreeBakers.Count; i++)
                Console.WriteLine($"Пекарь {FreeBakers[i].ID}, скорость {FreeBakers[i].Speed}");
            for (int i = 0; i < FreeСouriers.Count; i++)
                Console.WriteLine($"Курьер {FreeСouriers[i].ID}, скорость {FreeСouriers[i].Speed}, багажник {FreeСouriers[i].MaxLoad}");
            Console.WriteLine();
            int Time = 0; //время по тактам
            OrdersInQueue = new Queue<Order>(Count);
            OrdersInStock = new List<Order>(Count);
            int MaxQueueStock = 0; //максимальная очередь на склад
            int MaxQueueСashRegister = 0; //максимальная очередь на кассе
            CompletedOrders = new List<Order>(Count);
            while (CompletedOrders.Count < Count) //пока не будут выполнены все сегодняшние заказы
            {
                Time++;
                Console.WriteLine($"Такт {Time}:");
                while (TodayOrders.Count > 0)
                    if (TodayOrders.Peek().TimeOfReceipt == Time)
                    {
                        Order CurOrder = TodayOrders.Dequeue();
                        Console.WriteLine($"Поступил заказ {CurOrder.ID}");
                        OrdersInQueue.Enqueue(CurOrder);
                    }    
                    else break;
                Baker.DistributeOrders(OrdersInQueue, FreeBakers, ref MaxQueueСashRegister);
                Baker.UploadToWarehouse(OrdersInStock, T, FreeBakers, ref MaxQueueStock);
                Сourier.DistributeOrders(OrdersInStock, FreeСouriers, Time);
                foreach (var Bak in Baker.Bakers) Bak.MakeMove();
                foreach (var Cur in Сourier.Couriers) Cur.MakeMove(FreeСouriers, CompletedOrders, Time);
                Console.WriteLine();
            }
            Console.WriteLine("Рекомендации по итогам дня: ");
            int problems = 0;
            for (int i = 0; i < Count; i++)
                if (LimitTime < CompletedOrders[i].WaitingTime) problems++;
            if (problems == 0) //если всё хорошо
            {
                Console.WriteLine("- Увеличить количество заказов");
                for (int i = 0; i < Сourier.Couriers.Count; i++)
                    if (Сourier.Couriers[i].NumOfOrdersDeliveered == 0)
                        Console.WriteLine($"- Уволить курьера {Сourier.Couriers[i].ID}");
                for (int i = 0; i < Baker.Bakers.Count; i++)
                    if (Baker.Bakers[i].NumOfCompOrders == 0)
                        Console.WriteLine($"- Уволить пекаря {Baker.Bakers[i].ID}");
            }
            else
            {
                if (MaxQueueСashRegister > 0) Console.WriteLine("- Нанять пекаря");
                Console.WriteLine("- Нанять курьера");
            }
            if (MaxQueueStock > 0) Console.WriteLine("- Расширить склад");
        }
    }
}
