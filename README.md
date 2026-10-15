# Poprawa kolokwium — API klientów i samochodów

Projekt z poprawy kolokwium APBD: ASP.NET Core Web API z kontrolerem klientów, DTO samochodów i serwisem bazy danych opartym na SqlClient.

**Kategoria:** kolokwia APBD · **Przedmiot:** APBD · **Rok akademicki:** 2024/2025

## Technologie

C#, .NET 9, ASP.NET Core, Microsoft.Data.SqlClient.

## Gdzie jest kod

- [Plik projektu](./KolokwiumPoprawa/KolokwiumPoprawa.csproj): `KolokwiumPoprawa/KolokwiumPoprawa.csproj`.
- Kod aplikacji znajduje się w katalogu tego projektu.

## Uruchomienie

Wymagany jest odpowiedni .NET SDK dla projektu (.NET 9); jeśli repozytorium zawiera `global.json`, uwzględnij również wskazaną w nim wersję SDK.

Projekt wymaga dostępu do SQL Server i bazy zgodnej z modelami lub zapytaniami ćwiczenia. Skonfiguruj lokalne połączenie w ustawieniach aplikacji przed uruchomieniem.

Z katalogu głównego repozytorium:

```sh
dotnet restore "KolokwiumPoprawa/KolokwiumPoprawa.csproj"
dotnet run --project "KolokwiumPoprawa/KolokwiumPoprawa.csproj"
```

## Nawigacja

[Wszystkie projekty i materiały ze studiów](https://github.com/Michlean30495)
