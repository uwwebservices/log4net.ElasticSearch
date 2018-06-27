ApiKey=$1
Source=$2

echo "Pushing out the nuget package...."
nuget push ./src/log4net.ElasticSearch.NetCore/bin/Release/.*.nupkg -Verbosity detailed -ApiKey $ApiKey -Source $Source
echo "Finished pushing out the nuget package."
