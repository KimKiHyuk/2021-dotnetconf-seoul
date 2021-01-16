# 2021-dotnetconf-seoul
**2021 닷넷 컨퍼런스 .NET for spark 맛보기**

### Reference
---
* https://hub.docker.com/r/3rdman/dotnet-spark
* https://3rdman.de/2020/10/net-for-apache-spark-interactive-notebook-docker-image/
* https://3rdman.de/2020/10/build-net-for-apache-spark-with-vs-code-in-a-browser/

## Getting started debugging

### 시작하기전에..

AWS 관련 셋팅을 진행해주세요.

* EXPORT aws_access_key_id=********
* EXPORT aws_secret_access_key=********
* aws configure
...


### debug example
---
1. 도커 컨테이너 실행
    ```
    docker run -d --name dotnet-spark \
    -p 4040:4040/tcp -p 5567:5567/tcp -p 6066:6066/tcp -p 7077:7077/tcp -p 8080:8080/tcp -p 8081:8081/tcp -p 8082:8082/tcp \
    keykim/demo-dotnetconf-seoul
    ```
1. 도커 extenstion을 통해 컨테이너에 접속
1. `dotnet new console -o <app name>`
1. `dotnet add package Microsoft.Spark --version 1.0.0`
1. `dotnet build`
1. 디버그 모드 활성화
    ```
    // avro, aws 등 패키지가 필요하면
    // --packages <org.apche.package>,<org.apache.package>... 를 추가
    spark-submit \
    --class org.apache.spark.deploy.dotnet.DotnetRunner \
    --master local \
    <app name>/bin/Debug/netcoreapp3.1/microsoft-spark-3-0_2.12-1.0.0.jar \
    debug  
    ```
1. F5

### Getting started myapp 
---
1. 도커 컨테이너 실행
    ```
    docker run -d --name dotnet-spark \
    -p 4040:4040/tcp -p 5567:5567/tcp -p 6066:6066/tcp -p 7077:7077/tcp -p 8080:8080/tcp -p 8081:8081/tcp -p 8082:8082/tcp \
    keykim/demo-dotnetconf-seoul
    ```
1. 도커 extenstion을 통해 컨테이너에 접속
1. git clone https://github.com/KimKiHyuk/2021-dotnetconf-seoul
1. cd myapp
1. dotnet build
1. spark-submit
    ```
    spark-submit --class org.apache.spark.deploy.dotnet.DotnetRunner --master local \
    bin/Debug/netcoreapp3.1/microsoft-spark-2-4_2.11-1.0.0.jar \
    dotnet bin/Debug/netcoreapp3.1/myapp.dll
    ```

### Getting started emrapp
---
1. 도커 컨테이너 실행
    ```
    docker run -d --name dotnet-spark \
    -p 4040:4040/tcp -p 5567:5567/tcp -p 6066:6066/tcp -p 7077:7077/tcp -p 8080:8080/tcp -p 8081:8081/tcp -p 8082:8082/tcp \
    keykim/demo-dotnetconf-seoul
    ```
2. 도커 extenstion을 통해 컨테이너에 접속
3. git clone https://github.com/KimKiHyuk/2021-dotnetconf-seoul
4. cd emrapp
5. dotnet build
6. spark-submit(local)
    ```
    spark-submit --conf spark.hadoop.fs.s3a.endpoint=s3.us-east-2.amazonaws.com \
    --conf spark.executor.extraJavaOptions=-Dcom.amazonaws.services.s3.enableV4=true \
    --conf spark.driver.extraJavaOptions=-Dcom.amazonaws.services.s3.enableV4=true \
    --packages org.apache.spark:spark-avro_2.11:2.4.6,org.apache.hadoop:hadoop-aws:2.7.7,com.amazonaws:aws-java-sdk:1.7.4 \
    --class org.apache.spark.deploy.dotnet.DotnetRunner --master local bin/Debug/netcoreapp3.1/microsoft-spark-2-4_2.11-1.0.0.jar \
    dotnet bin/Debug/netcoreapp3.1/emrapp.dll \
    s3a://<your-bucket>/<avro input directory> \
    s3a://<your-bucket>/<output directory>
    ```
6-1. spark-submit(aws emr)
  1. upload worker.sh (UDF 실행시 필요)
      ```
      wget https://github.com/dotnet/spark/blob/master/deployment/install-worker.sh
      aws s3 cp install-worker.sh <s3://<your-bucket> 
      ```
  2. download released worker (UDF 실행시 필요)
  
     https://github.com/dotnet/spark/releases/tag/v1.0.0 에서 다운로드
     ```
     aws s3 cp Microsoft.Spark.Worker.<zip file>  <s3://<your-bucket>
     ```
  3. 
     ```
     dotnet publish -c Release -f netcoreapp3.1 -r ubuntu.16.04-x64
     zip -r emrapp.zip bin/Release/netcoreapp3.1/ubuntu.16.04-x64/publish/
     aws s3 cp emrapp.zip <s3://<your-bucket>>
     ./deploy.sh
     ```
  
  4. 클러스터 생성
    
    스크립트안에 내용들을 본인 환경에 맞게 변경해주세요. (S3 위치 등)
    
    ./helper.sh -c
    stdout은 cluster-id입니다.
   
  5. 클러스터에 job 전달
  
    ./helper.sh -a <cluster-id>

