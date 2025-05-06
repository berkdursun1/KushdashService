FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /playerService
COPY published/ ./
ENTRYPOINT ["dotnet", "playerService.dll"]
