#!/bin/bash


main() {
    if [[ "$#" -eq 0 ]]; then
        print_help
    fi
    
    case "${1}" in
        -c|--create-cluster) createCluster "${@}";;
        -d|--destroy-cluster) deleteCluster "${@}";;
        -a|--add-step) addStep "${@}";;
        -h|--help) print_help
            exit 1 ;;
        *) echo "Unknown parameter passed: ${1}"; exit 1 ;;
    esac
}


function createCluster() {
    aws --profile default emr create-cluster \
    --name "Test cluster" \
    --log-uri "s3://aws-logs-417699346993-us-east-2/elasticmapreduce/" \
    --release-label emr-5.31.0 \
    --use-default-roles \
    --ec2-attributes KeyName=test-key \
    --applications Name=Spark \
    --instance-count 2 \
    --instance-type m4.large \
    --bootstrap-actions Path=s3://spark-app-vjal1251/install-worker.sh,Name="Install Microsoft.Spark.Worker",Args=["aws","s3://spark-app-vjal1251/Microsoft.Spark.Worker.netcoreapp3.1.linux-x64-1.0.0.tar.gz","/usr/local/bin"]

}
function addStep() {
    aws --profile default emr add-steps \
    --cluster-id $2 \
    --steps Type=spark,Name="Spark Program",Args=[--master,yarn,--packages,org.apache.spark:spark-avro_2.11:2.4.6,--class,org.apache.spark.deploy.dotnet.DotnetRunner,s3://spark-app-vjal1251/jars/microsoft-spark-2-4_2.11-1.0.0.jar,s3://spark-app-vjal1251/dll/dotnetconf.zip,emrapp,s3a://spark-data-vjal1251/topics/orders/partition=0,s3a://spark-data-vjal1251/result],ActionOnFailure=CONTINUE
}

function deleteCluster() {
    aws emr default terminate-clusters --cluster-id $2
}



print_help() {
  cat <<HELPMSG
Usage: helper.sh [OPTIONS]"
Options:
    -c, --create-cluster
    -d, --destroy-cluster <cluster-id>
    -a, --add-step <cluster-id>
    -h, --help
HELPMSG
}

main "${@}"


