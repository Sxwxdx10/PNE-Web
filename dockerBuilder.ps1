dotnet publish --os linux -c Release
docker build -t lecanardnoir/pne-image -f Dockerfile .