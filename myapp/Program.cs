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
            // create the spark session
            SparkSession spark = SparkSession.Builder().GetOrCreate();
            // create a dataframe from the json file
        
            var df = spark.Read()
                .Option("header", true)
                .Csv("2021-dotnetconf-seoul/myapp/sample.csv");

    
            df = df.Filter("name IS NOT NULL AND age >= 20")
                .GroupBy("address")
                .Count()
                .OrderBy("count");

            df.Show();
        }
    }
}