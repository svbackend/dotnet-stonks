## Installation

`cp .env .env.local`

`docker-compose up`

`cd Server && dotnet ef database update`

### Useful commands

`dotnet user-secrets init`

`dotnet user-secrets set "SecretKey" "RandomString3ddf15e36d5eeab329ba"`

`dotnet ef migrations add InitCreate`

`dotnet ef database update`