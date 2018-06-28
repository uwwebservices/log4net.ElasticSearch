ApiKey=$1
Source=$2

echo "Pushing out the nuget package...."
pwd
dotnet nuget push log4net.ElasticSearch.NetCore/bin/Release/.*.nupkg --api-key $ApiKey --source $Source
echo "Finished pushing out the nuget package."
