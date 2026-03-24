# ใช้ .NET 8 SDK ในการ Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore
RUN dotnet publish -c Release -o /app

# ใช้ .NET 8 Runtime ในการรัน
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app .

# Render ใช้พอร์ต 10000 เป็นค่าเริ่มต้น
ENV ASPNETCORE_URLS=http://*:10000
EXPOSE 10000

ENTRYPOINT ["dotnet", "MoneyFlowAPI.dll"]