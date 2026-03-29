# Scriptium Backend
This repository represents backend application of [Scriptium](github.com/scriptium-project)

## Getting Started

### Prerequisites
- .NET 10 SDK
- EF Core .NET Tools (`dotnet tool install --global dotnet-ef`)
- Docker (Optional)

You should already have installed SDK for .NET 10 and EF Core .NET Tools.

### Installation

```bash
git clone https://github.com/kaanoz1/scriptium-backend.git
cd scriptium-backend
```

After that, you can run this project either locally or **Docker**.

### Configuration (For Local Run):
 	
You should create you own appsettings.json.

```bash
cp appsettings.Development.Example.json appsettings.Development.json # For Development
cp appsettings.Development.Example.json appsettings.json # For Production
```

if you use docker, you should pass this environment variables into the container. The program will stop you unless you pass necessary environment variables. You can get a hint from appsettings.json files to understand which variables are necessary.

#### Migrations:

Project automatically detects uncommitted migrations and apply them automatically. Check out `Program.cs`. if you want to do it manual. Run: 
```bash
dotnet ef database update
```

#### Build the project:

```bash
dotnet restore
dotnet build
dotnet run
```

Once running, the API is typically available at http://localhost:5000 (check your console output for the specific port).

You are good to go.

## 📄 License

This project is licensed under the [MIT License](LICENCE) on behalf of Scriptium.



