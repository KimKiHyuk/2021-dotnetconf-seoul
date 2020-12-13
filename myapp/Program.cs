using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Spark.Sql;

namespace myapp
{
    class Program
    {
        static void Main(string[] args)
        {
            SparkSession spark = SparkSession.Builder().GetOrCreate();
        
            var df = spark.Read()
                .Option("header", true)
                .Csv("myapp/sample.csv");

    
            df = df.Filter("name IS NOT NULL AND age >= 20")
                .GroupBy("address")
                .Count()
                .OrderBy("count");

            df.Show();
        }
    }
}