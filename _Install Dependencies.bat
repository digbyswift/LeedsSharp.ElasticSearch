REM Install azure command line tools
npm i -g azure-functions-core-tools

REM Install docker image
docker pull docker.elastic.co/elasticsearch/elasticsearch:6.1.1
docker run --name LeedsSharp -p 9200:9200 -e "http.host=0.0.0.0" -e "transport.host=127.0.0.1" docker.elastic.co/elasticsearch/elasticsearch:6.1.1
