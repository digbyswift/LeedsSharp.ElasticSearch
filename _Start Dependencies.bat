@ECHO OFF

ECHO Starting Azure emulator ...
REM This will start the Azure storage emulator
"C:\Program Files (x86)\Microsoft SDKs\Azure\Storage Emulator\AzureStorageEmulator.exe" start


ECHO Starting ElasticSearch ...
docker start LeedsSharp
REM If this hangs, you may need to restart Docker for Windows! No idea why but it's happened and that's how you fix it.


ECHO Starting SearchIndexer function ...
REM You will need to install: npm i -g azure-functions-core-tools
REM if this is not already installed. This is purely to run the func from
REM the command line. See https://github.com/Azure/azure-functions-cli
REM for details.

REM You will also need to ensure that the SearchIndexer has a DEBUG configuration
REM built.
cd "LeedsSharp.ElasticSearch.IndexRunner\bin\Debug\net462"
func run IndexRunner
pause
