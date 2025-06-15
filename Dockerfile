
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["SWP301-BloodDonationSystem.sln", "./"]
COPY ["BloodDonationSupportSystem/BloodDonationSupportSystem.csproj", "BloodDonationSupportSystem/"]
COPY ["BusinessLayer/BusinessLayer.csproj", "BusinessLayer/"]
COPY ["DataAccessLayer/DataAccessLayer.csproj", "DataAccessLayer/"]
RUN dotnet restore "SWP301-BloodDonationSystem.sln"

COPY . .
WORKDIR "/src"
RUN dotnet build "SWP301-BloodDonationSystem.sln" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SWP301-BloodDonationSystem.sln" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BloodDonationSupportSystem.dll"]