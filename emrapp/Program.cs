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
                .Load("s3a://spark-data-vjal1251/topics/orders/partition=0/");

            

            df.Show();
            // df.Coalesce(1)
            //     .Write()
            //     .Format("csv")
            //     .Save($"{args[1]}/{DateTime.UtcNow.ToString("yyyy/MM/dd/hh-mm-ss")}");  
        }
    }
}
