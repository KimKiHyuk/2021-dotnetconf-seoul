using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Spark.Sql;

namespace emrapp
{
    class Program
    {
        static void Main(string[] args)
        {
            SparkSession spark = SparkSession.Builder()
                .AppName("emrapp")
                .GetOrCreate();
            DataFrame df = spark.Read()
                .Format("avro")
                .Load(args[0]);
            
            df = df.Drop("address")
                .GroupBy("itemid")
                .Count();
            
            df.Show();
            df.Coalesce(1)
                .Write()
                .Format("csv")
                .Save($"{args[1]}/{DateTime.UtcNow.ToString("yyyy/MM/dd/hh-mm-ss")}");  
        }
    }
}
