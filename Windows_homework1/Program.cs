using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

//定义生产工厂接口
interface IProductionFactory
{
    void ProduceNeck();
    void ProduceWing();
}

//实现各个具体工厂
class WuhanFactory : IProductionFactory
{
    public void ProduceNeck()
    {
        Console.WriteLine("Wuhan  Factory produce duck neck.");
    }
    public void ProduceWing()
    {
        Console.WriteLine("Wuhan  Factory produce duck wing.");
    }
}

class NanjingFactory : IProductionFactory
{
    public void ProduceNeck()
    {
        Console.WriteLine("Nanjing  Factory produce duck neck.");
    }
    public void ProduceWing()
    {
        throw new NotImplementedException("Nanjing Factory has no access to produce duck wing!!!");
    }
}

class ChangshaFactory : IProductionFactory
{
    public void ProduceNeck()
    {
        throw new NotImplementedException("Changsha Factory has no access to produce duck neck!!!");
    }
    public void ProduceWing()
    {
        Console.WriteLine("Changsha Factory produce duck wing.");
    }
}

namespace homework1
{
    internal class Program
    {
        //定义委托
        public delegate void ProductionDelegate();

        //定义事件
        public event ProductionDelegate ExecuteOrder;

        static void Main(string[] args)
        {
            //实例化工厂对象
            IProductionFactory Wuhan_Factory = new WuhanFactory();
            IProductionFactory Nanjing_Factory = new NanjingFactory();
            IProductionFactory Changsha_Factory = new ChangshaFactory();

            //三个委托分别表示不同工厂的订单
            // 武汉工厂可以生产鸭脖和鸭翅
            ProductionDelegate wuhanProduction = Wuhan_Factory.ProduceNeck;
            wuhanProduction += Wuhan_Factory.ProduceWing;
            // 南京工厂只能生产鸭脖
            ProductionDelegate nanjingProduction = Nanjing_Factory.ProduceNeck;
            nanjingProduction += Nanjing_Factory.ProduceWing;
            // 长沙工厂只能生产鸭翅
            ProductionDelegate changshaProduction = Changsha_Factory.ProduceWing;

            Console.WriteLine("Wuhan Factory production:");
            try
            {
                wuhanProduction();
            }
            catch (NotImplementedException e)
            {
                Console.WriteLine(e.Message);
            }

            Console.WriteLine("Nanjing Factory production:");
            try
            {
                nanjingProduction();
            }

            catch (NotImplementedException e)
            {
                Console.WriteLine(e.Message);
            }

            Console.WriteLine("Changsha Factory production:");
            try
            {
                changshaProduction();
            }
            catch (NotImplementedException e)
            {
                Console.WriteLine(e.Message);
            }
            Console.ReadLine();
        }
    }
}