#! bin/bash
dotnet PNE-admin.dll&
dotnet PNE-api.dll&
wait -n
exit $?