ApiKey=$1
Source=$2

echo "Pushing out the nuget package...."
dotnet nuget push log4net.ElasticSearch.NetStd/bin/Release/*.nupkg --api-key $ApiKey --source $Source
echo "Finished pushing out the nuget package."
